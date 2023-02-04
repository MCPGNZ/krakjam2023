namespace Krakjam
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        /* death conditions */
        public Action OnDeath;
        public float Life = 100.0f;
        public bool IsDead;
        public float SpeedThreshold = 10.0f;

        /* movement parameters */
        public float MovementSpeed = 1000.0f;

        public float RotationSensitivity = .5f;
        public float RotationSpeed = 1;

        /* jump parameters */
        public float DistanceToGround = 0.1f;
        public float JumpStrength = 100.0f;
        public LayerMask GroundMask;

        [ShowInInspector]
        public bool IsGrounded
        {
            get
            {
                var ray = new Ray(transform.position, -transform.up);
                return Physics.Raycast(ray, out var info, DistanceToGround, GroundMask);
            }
        }

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

        private float _InitialLife;
        private Rigidbody _Rigidbody;

        private Vector2 _Direction;
        private Vector2 _Turn;
        private bool _Jump;
        private Vector3 _JumpNormal;

        private void Awake()
        {
            _Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
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
                    var ray = new Ray(transform.position, -transform.up);
                    bool grounded = Physics.Raycast(ray, out var info, DistanceToGround, GroundMask);
                    if (grounded)
                    {
                        _Jump = true;
                        _JumpNormal = info.normal;
                    }
                }
            }

            /* look */
            _Turn.x += Input.GetAxis("Mouse X") * RotationSensitivity;
            _Turn.y += Input.GetAxis("Mouse Y") * RotationSensitivity;
            transform.localRotation = Quaternion.Euler(-_Turn.y, _Turn.x, 0);

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
        private void FixedUpdate()
        {
            _Rigidbody.AddForce(transform.forward * (MovementSpeed * _Direction.x * Time.fixedDeltaTime), ForceMode.Force);
            _Rigidbody.AddForce(transform.right * (MovementSpeed * _Direction.y * Time.fixedDeltaTime), ForceMode.Force);
            _Direction = Vector2.zero;

            if (_Jump)
            {
                _Rigidbody.AddForce(_JumpNormal * JumpStrength, ForceMode.Impulse);
                _Jump = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var rigidbody = other.attachedRigidbody;
            if (rigidbody == null) { return; }

            var orbController = rigidbody.GetComponent<OrbController>();
            if (orbController != null)
            {
                Debug.Log("Pickup Orb");
                orbController.Pickup();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * DistanceToGround);

            if (IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + _JumpNormal * 2.0f);
            }
        }
    }
}