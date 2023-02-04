namespace Krakjam
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public sealed class PlayerController : MonoBehaviour
    {
        #region Public Variables
        public Action OnDeath;
        public Action OnPickUpAction;

        public float Life = 100.0f;
        public float InitialLife => _InitialLife;

        public bool IsDead;
        public float SpeedThreshold = 10.0f;

        public Camera Camera;

        /* movement parameters */
        public float MovementSpeed = 1000.0f;

        public float RotationSensitivity = .5f;
        public float RotationSpeed = 1;

        /* dash parameters */
        public float Dashstrength = 100.0f;

        /* jump parameters */
        public float DistanceToGround = 0.1f;
        public float JumpStrength = 100.0f;
        public LayerMask GroundMask;

        /* ground rotation */
        public float RotationToGroundNormalSpeed = 1.0f;
        public float GroundDetectionLength = 1.5f;
        public float GroundForce = 1.0f;

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
            _InitialLife = Life;
            Cursor.lockState = CursorLockMode.Locked;
        }
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (IsDead) { return; }

            UpdateMovement();
            UpdateLife();
            UpdateGround();
        }
        private void FixedUpdate()
        {
            _Rigidbody.AddForce(Camera.transform.forward * (MovementSpeed * _Direction.x * Time.fixedDeltaTime), ForceMode.Force);
            _Rigidbody.AddForce(Camera.transform.right * (MovementSpeed * _Direction.y * Time.fixedDeltaTime), ForceMode.Force);
            _Direction = Vector2.zero;

            if (_Jump)
            {
                _Rigidbody.AddForce(_GroundNormal * JumpStrength, ForceMode.Impulse);

                _Jump = false;
            }
            if (_Dash && !_DashedOnce)
            {
                Debug.Log("YEAH");
                _Rigidbody.AddForce(Camera.transform.forward * Dashstrength, ForceMode.Impulse);
                _Dash = false;
                _DashedOnce = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var rigidbody = other.attachedRigidbody;
            if (rigidbody == null) { return; }

            var orbController = rigidbody.GetComponent<OrbController>();
            if (orbController != null)
            {
                var orbType = orbController.OrbType;
                MovementSpeed += orbController.OrbType.SpeedChange;
                MovementSpeed = Math.Clamp(MovementSpeed, MIN_SPEED, MAX_SPEED);
                var previousXChunks = _SplitScreenChunks.ChunkSizeX;
                var previousYChunks = _SplitScreenChunks.ChunkSizeY;
                var resizeX = Mathf.Clamp(previousXChunks + orbType.ResolutionChange, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);
                var resizeY = Mathf.Clamp(previousXChunks + orbType.ResolutionChange, MIN_CHUNK_SIZE, MAX_CHUNK_SIZE);
                _SplitScreenChunks.Resize(resizeX, resizeY);
                Game.Score++;
                OnPickUpAction?.Invoke();
                orbController.Pickup();
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

        private float _InitialLife;
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

        #endregion Private Variables

        #region Private Methods
        private void UpdateMovement()
        {
            if (IsGrounded)
            {
                /* movement */
                if (Input.GetKey(KeyCode.W)) { _Direction += new Vector2(1.0f, 0.0f); }
                if (Input.GetKey(KeyCode.S)) { _Direction += new Vector2(-1.0f, 0.0f); }
                if (Input.GetKey(KeyCode.A)) { _Direction += new Vector2(0.0f, -1.0f); }
                if (Input.GetKey(KeyCode.D)) { _Direction += new Vector2(0.0f, 1.0f); }
                /* jump */
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_Ground != null)
                    {
                        _Jump = true;
                    }
                }
                _DashedOnce = false;
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
            Camera.transform.localRotation = Quaternion.Euler(-_Turn.y, _Turn.x, 0.0f); ;
        }
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
                Life = Mathf.Min(Life, _InitialLife);
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
                    Quaternion.LookRotation(transform.forward, _GroundNormal), Time.deltaTime * RotationToGroundNormalSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(transform.forward, Vector3.up), Time.deltaTime * RotationToGroundNormalSpeed);
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
    }
}