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
        public Action OnDeath;
        public Action OnPickUpAction;

        public float Life;

        public bool IsDead;
        public float SpeedThreshold = 10.0f;

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
                return _Rigidbody.velocity.magnitude;
            }
        }

        [ShowInInspector]
        public bool IsBelowThreshold => CurrentSpeed < SpeedThreshold;

        #endregion Public Variables

        #region Unity Methods
        private void Awake()
        {
            _Rigidbody = GetComponent<Rigidbody>();
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
        }
        private void FixedUpdate()
        {
            var modifier = IsGrounded ? 1.0f : GameBalance.AirSpeed;
            _Rigidbody.AddForce(modifier * Camera.transform.forward * (GameBalance.MovementSpeed * _Direction.x * Time.fixedDeltaTime), ForceMode.Force);
            _Rigidbody.AddForce(modifier * Camera.transform.right * (GameBalance.MovementSpeed * _Direction.y * Time.fixedDeltaTime), ForceMode.Force);

            if (_Jump)
            {
                _Rigidbody.AddForce(_GroundNormal * GameBalance.JumpStrength, ForceMode.Impulse);

                _Jump = false;
            }
            if (_Dash && !_DashedOnce)
            {
                Debug.Log("YEAH");
                _Rigidbody.AddForce(Camera.transform.forward * GameBalance.DashStrength, ForceMode.Impulse);
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
                //MovementSpeed += orbController.OrbType.SpeedChange;
                //MovementSpeed = Math.Clamp(MovementSpeed, MIN_SPEED, MAX_SPEED);
                var previousXChunks = _SplitScreenChunks.ChunkSizeX;
                var previousYChunks = _SplitScreenChunks.ChunkSizeY;
                var resizeX = Mathf.Clamp(previousXChunks + orbType.ResolutionChange, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);
                var resizeY = Mathf.Clamp(previousXChunks + orbType.ResolutionChange, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);
                _SplitScreenChunks.Resize(resizeX, resizeY);
                Game.Score++;
                OnPickUpAction?.Invoke();
                orbController.Pickup();
            }

            /* trap support */
            if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
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

        private int _RayCount = 32;
        private Transform _Ground;
        private Vector3 _GroundNormal;

        private const float MIN_SPEED = 100.0f;
        private const float MAX_SPEED = 1500.0f;

        private const int MAX_CHUNK_SIZE = 16;
        private const int MIN_CHUNK_SIZE = 8;

        private Vector2 _TurnInput;
        #endregion Private Variables

        #region Private Methods
        private void UpdateLife()
        {
            /* life */
            if (CurrentSpeed < SpeedThreshold)
            {
                Life -= Time.deltaTime;
                if (Life <= 0.0f)
                {
                    IsDead = true;
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