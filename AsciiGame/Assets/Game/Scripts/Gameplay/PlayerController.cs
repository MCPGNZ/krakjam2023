namespace Krakjam
{
    using System;
    using Assets.Game.Scripts;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class PlayerController : MonoBehaviour
    {
        #region Public Variables
        public AudioSource MonsterAudioSource;
        public AudioSource PlayerAudioSource;

        public Action OnDeath;
        public Action OnPickUpAction;

        public float Life;

        public bool IsDead;
        public float SpeedThreshold = 10.0f;
        public float MovementSpeedMultiplicator = 1.0f;

        public Camera Camera;

        /* movement parameters */
        public float RotationSensitivity = .5f;
        public float RotationSpeed = 1;

        /* jump parameters */
        public float DistanceToGround = 0.1f;
        public LayerMask GroundMask;

        /* ground rotation */
        public float GroundDetectionLength = 1.5f;

        [ShowInInspector]
        public bool IsGrounded => _Ground != null;

        [ShowInInspector]
        public float CurrentSpeed
        {
            get
            {
                if (_Rigidbody == null) { return 0.0f; }
                return _Rigidbody.velocity.magnitude * MovementSpeedMultiplicator;
            }
        }

        [ShowInInspector]
        public bool IsBelowThreshold => CurrentSpeed < SpeedThreshold;

        #endregion Public Variables

        #region Unity Methods
        private void Awake()
        {
            _Rigidbody = GetComponent<Rigidbody>();
            PlayerAudioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _SplitScreenChunks = GetComponentInChildren<SplitScreenToChunks>();
            Life = GameBalance.Life;
            Cursor.lockState = CursorLockMode.Locked;
        }
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (IsDead) { return; }

            /* update value */
            _Turn.x += _TurnInput.x * RotationSensitivity;
            _Turn.y += _TurnInput.y * RotationSensitivity;
            Camera.transform.localRotation = Quaternion.Euler(-_Turn.y, _Turn.x, 0.0f); ;

            UpdateLife();
            UpdateGround();
            if (!_DashedOnce) { _DashTimer -= Time.deltaTime; }
        }
        private void FixedUpdate()
        {
            var modifier = IsGrounded ? 1.0f : GameBalance.AirSpeed;
            _Rigidbody.AddForce(modifier * Camera.transform.forward * (GameBalance.MovementSpeed * _Direction.x * Time.fixedDeltaTime), ForceMode.Force);
            _Rigidbody.AddForce(modifier * Camera.transform.right * (GameBalance.MovementSpeed * _Direction.y * Time.fixedDeltaTime), ForceMode.Force);

            if (_PlayerMoving)
            {
                if (!PlayerAudioSource.isPlaying && IsGrounded)
                {
                    PlayerAudioSource.PlayOneShot(GameBalance.PlayerFootsteps);
                }
                _PlayerMoving = false;
            }
            _Direction = Vector2.zero;

            if (_DashTimer <= 0.0f)
            {
                _DashTimer = 2.0f;
                _DashedOnce = false;
            }

            if (_Jump)
            {
                PlayerAudioSource.PlayOneShot(GameBalance.PlayerJumpSound);
                _Rigidbody.AddForce(_GroundNormal * GameBalance.JumpStrength, ForceMode.Impulse);

                _Jump = false;
            }
            if (_Dash && !_DashedOnce)
            {
                PlayerAudioSource.PlayOneShot(GameBalance.PlayerDeathSound);
                if (!IsGrounded) { _Rigidbody.AddForce(Camera.transform.forward * GameBalance.DashStrength, ForceMode.Impulse); }
                _Dash = false;
                _DashedOnce = true;
            }

            if (IsGrounded)
            {
                _Rigidbody.AddForce(-_Rigidbody.velocity.normalized * _Rigidbody.velocity.sqrMagnitude *
                    GameBalance.Friction * Time.fixedDeltaTime);
            }
            else
            {
                _Rigidbody.AddForce(-_Rigidbody.velocity.normalized * _Rigidbody.velocity.sqrMagnitude *
                    GameBalance.MaxFriction * Time.fixedDeltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var rigidbody = other.attachedRigidbody;
            if (rigidbody == null) { return; }

            /* orb support */
            var orbController = rigidbody.GetComponent<OrbController>();
            if (orbController != null)
            {
                var orbType = orbController.OrbType;
                MovementSpeedMultiplicator += orbController.OrbType.SpeedChange;
                MovementSpeedMultiplicator = Math.Clamp(MovementSpeedMultiplicator, MIN_SPEED, MAX_SPEED);
                var previousXChunks = _SplitScreenChunks.ChunkSizeX;
                var previousYChunks = _SplitScreenChunks.ChunkSizeY;
                var resizeX = Mathf.Clamp(previousXChunks + orbType.ResolutionChange, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);
                var resizeY = Mathf.Clamp(previousXChunks + orbType.ResolutionChange, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);
                _SplitScreenChunks.Resize(resizeX, resizeY);
                Game.Score++;
                OnPickUpAction?.Invoke();
                orbController.Pickup();
                PlayerAudioSource.PlayOneShot(GameBalance.PlayerPickupOrb);
            }

            /* trap support */
            if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                PlayerAudioSource.PlayOneShot(GameBalance.PlayerHitSound);
                Debug.LogError("It's a trap");
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * DistanceToGround);

            FindClosestObstacle(out var closestRay, out var hitInfo, out var hitAngle);

            for (int i = 0; i < _RayCount; ++i)
            {
                var angle = (360.0f / _RayCount) * i;
                var quaternion = Quaternion.AngleAxis(angle, transform.forward);

                if (angle == hitAngle)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.gray;
                }

                Gizmos.DrawLine(transform.position, transform.position + quaternion * transform.up * GroundDetectionLength);
            }
        }
        #endregion Unity Methods

        #region Private Variables
        private SplitScreenToChunks _SplitScreenChunks;

        private Rigidbody _Rigidbody;

        private Vector2 _Direction;
        private Vector2 _Turn;
        private bool _Jump;
        private bool _Dash;
        private bool _DashedOnce = false;
        private bool _PlayerMoving = false;
        private float _DashTimer = 2.0f;

        private int _RayCount = 32;
        private Transform _Ground;
        private Vector3 _GroundNormal;

        private const float MIN_SPEED = 0.2f;
        private const float MAX_SPEED = 7.0f;

        private const int MAX_CHUNK_SIZE = 16;
        private const int MIN_CHUNK_SIZE = 8;

        private Vector2 _TurnInput;
        #endregion Private Variables

        #region Private Methods
        private void UpdateMovement()
        {
            _PlayerMoving = false;
            /* movement */
            if (Input.GetKey(KeyCode.W))
            {
                _PlayerMoving = true;
                _Direction += new Vector2(1.0f, 0.0f);
            }
            if (Input.GetKey(KeyCode.S))
            {
                _PlayerMoving = true;
                _Direction += new Vector2(-1.0f, 0.0f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                _PlayerMoving = true;
                _Direction += new Vector2(0.0f, -1.0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                _PlayerMoving = true;
                _Direction += new Vector2(0.0f, 1.0f);
            }

            if (IsGrounded)
            {
                _DashedOnce = false;
                /* jump */
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_Ground != null)
                    {
                        _Jump = true;
                    }
                }
            }
            /* Dash */
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (_Ground == null)
                {
                    _Dash = true;
                }
            }

            /* look */
            _Turn.x += Input.GetAxis("Mouse X") * RotationSensitivity;
            _Turn.y += Input.GetAxis("Mouse Y") * RotationSensitivity;
            Camera.transform.localRotation = Quaternion.Euler(-_Turn.y, _Turn.x, 0.0f);
        }
        private void UpdateLife()
        {
            float monterVolume = Mathf.Lerp(GameBalance.MaxVolume, 0.0f, Mathf.Pow(Normalization(Life, 0.0f, GameBalance.Life), 4.0f));

            MonsterAudioSource.volume = monterVolume;
            /* life */
            if (CurrentSpeed < SpeedThreshold)
            {
                Life -= Time.deltaTime;
                if (Life <= 0.0f)
                {
                    IsDead = true;
                    PlayerAudioSource.PlayOneShot(GameBalance.PlayerDeathSound);
                    OnDeath?.Invoke();
                    return;
                }
            }
            else
            {
                Life += Time.deltaTime;
                Life = Mathf.Min(Life, GameBalance.Life);
            }
        }

        private float Normalization(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }
        private void UpdateGround()
        {
            _Ground = null;
            _GroundNormal = Vector3.up;

            if (FindClosestObstacle(out var ray, out var hit, out float angle))
            {
                _Ground = hit.transform;
                _GroundNormal = hit.normal;
            }

            if (_Ground != null)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(transform.forward, _GroundNormal), Time.deltaTime * GameBalance.RotationToGroundNormal);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(transform.forward, Vector3.up), Time.deltaTime * GameBalance.RotationToGroundNormal);
            }
        }

        private bool FindClosestObstacle(out Ray closest, out RaycastHit hit, out float hitAngle)
        {
            closest = default;
            hit = default;
            hitAngle = -1.0f;

            var distance = float.MaxValue;

            for (int i = 0; i < _RayCount; ++i)
            {
                var angle = (360.0f / _RayCount) * i;
                var quaternion = Quaternion.AngleAxis(angle, transform.forward);

                var ray = new Ray(transform.position, quaternion * transform.up);
                if (Physics.Raycast(ray, out var hitInfo, GroundDetectionLength, GroundMask))
                {
                    if (hitInfo.distance < distance)
                    {
                        distance = hitInfo.distance;

                        closest = ray;
                        hit = hitInfo;
                        hitAngle = angle;
                    }
                }
            }

            return hitAngle != -1.0f;
        }
        #endregion Private Methods

        #region Input
        public void OnMovement(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _Direction = new Vector2(value.y, value.x);
            _PlayerMoving = true;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (IsGrounded)
            {
                _Jump = true;
                _DashedOnce = false;
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (IsGrounded == false)
            {
                _Dash = true;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            /* look */
            _TurnInput = context.ReadValue<Vector2>();
        }

        public void OnExit(InputAction.CallbackContext context)
        {
            SceneReferences.LoadGameplay();
        }
        #endregion Input
    }
}