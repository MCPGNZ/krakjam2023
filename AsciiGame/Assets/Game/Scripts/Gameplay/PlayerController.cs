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
        public WallMovement Monster;
        public Canvas StartInfoCanvas;

        public AudioSource MonsterAudioSource;
        public AudioSource PlayerAudioSource;

        public Action OnDeath;
        public Action OnPickUpAction;

        public float Life;

        public bool IsDead;
        public float SpeedThreshold = 10.0f;
        public float MovementSpeedMultiplicator = 1.0f;

        public Camera Camera;

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
                return _Rigidbody.velocity.magnitude + _DefaultSpeed * MovementSpeedMultiplicator;
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

            _PreviousChunkX = _SplitScreenChunks.ChunkSizeX;
            _PreviousChunkY = _SplitScreenChunks.ChunkSizeY;
        }
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            Camera.transform.localRotation = Quaternion.Euler(0.0f, _AnimationRotationFactor, 0.0f);
            if (!_IsAnimationEnd)
            {
                _AnimationStartTimer -= Time.deltaTime;

                if (_AnimationStartTimer <= 0.0f)
                {
                    _AnimationRotationFactor += GameBalance.AnimationSpeed * Time.deltaTime;

                    if (_AnimationRotationFactor >= _AnimationRotationThreshold)
                    {
                        _IsAnimationEnd = true;
                    }
                }
                return;
            }
            if (_InfoDisplayTImer >= 0.0f)
            {
                _InfoDisplayTImer -= Time.deltaTime;
                StartInfoCanvas.enabled = true;
            }
            else
            {
                if (StartInfoCanvas.enabled)
                {
                    Monster.WallSpeedMovement = GameBalance.MonsterSpeed;
                    StartInfoCanvas.enabled = false;
                }
            }
            if (transform.position.y <= GameBalance.DeathHeighOffset)
            {
                IsDead = true;
                PlayerAudioSource.PlayOneShot(GameBalance.PlayerDeathSound);
                OnDeath?.Invoke();
                return;
            }
            if (IsDead) { return; }

            /* update value */
            _Turn.x += _TurnInput.x * GameBalance.RotationSensitivity;
            _Turn.y += _TurnInput.y * GameBalance.RotationSensitivity;
            Camera.transform.localRotation = Quaternion.Euler(-_Turn.y, _Turn.x + _AnimationRotationFactor, 0.0f); ;

            UpdateLife();
            UpdateGround();
            if (!_DashedOnce) { _DashTimer -= Time.deltaTime; }
        }
        private void FixedUpdate()
        {
            if (!_IsAnimationEnd)
            {
                return;
            }

            var modifier = IsGrounded ? 1.0f : GameBalance.AirSpeed;
            _Rigidbody.AddForce(modifier * Camera.transform.forward * (GameBalance.MovementSpeed * _Direction.x * Time.fixedDeltaTime), ForceMode.Force);
            _Rigidbody.AddForce(modifier * Camera.transform.right * (GameBalance.MovementSpeed * _Direction.y * Time.fixedDeltaTime), ForceMode.Force);

            if (_DashTimer <= 0.0f)
            {
                _DashTimer = 2.0f;
                _DashedOnce = false;
            }

            if (_Jump)
            {
                _Rigidbody.AddForce(_GroundNormal * GameBalance.JumpStrength, ForceMode.Impulse);

                _Jump = false;
            }
            if (_Dash && !_DashedOnce)
            {
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
        private float _DashTimer = 2.0f;

        [SerializeField] private int _RayCount = 128;
        private Transform _Ground;
        private Vector3 _GroundNormal;

        private const float MIN_SPEED = 0.2f;
        private const float MAX_SPEED = 1000.0f;

        private Vector2 _TurnInput;

        private float _AnimationStartTimer = 2.0f;
        private float _InfoDisplayTImer = 1.0f;
        private float _AnimationRotationFactor;
        private const float _AnimationRotationThreshold = 180.0f;

        private int _PreviousChunkX;
        private int _PreviousChunkY;

        private float _DefaultSpeed = 1.0f;

        private bool _IsAnimationEnd = false;
        #endregion Private Variables

        #region Private Methods
        private void UpdateLife()
        {
            float monterVolume = Mathf.Lerp(GameBalance.MaxVolume, 0.0f, Mathf.Pow(Normalization(Life, 0.0f, GameBalance.Life), GameBalance.VolumeChangeRateStrength));

            MonsterAudioSource.volume = monterVolume;
            /* life */

            var t = Mathf.Pow(Normalization(Life, 0.0f, GameBalance.Life), GameBalance.ResolutionChangeStrength);

            var resizeX = (int)Mathf.Round(Mathf.Lerp(_PreviousChunkX + 8.0f, _PreviousChunkX - 4.0f, t));
            var resizeY = (int)Mathf.Round(Mathf.Lerp(_PreviousChunkY + 8.0f, _PreviousChunkY - 4.0f, t));
            _SplitScreenChunks.Resize(Mathf.Max(resizeX, 8), Mathf.Max(resizeY, 8));

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
                Life += Time.deltaTime * GameBalance.LifeTimeChangeSpeed;
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