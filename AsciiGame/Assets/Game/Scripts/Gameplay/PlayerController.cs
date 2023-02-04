namespace Krakjam
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public float MovementSpeed = 1000.0f;

        public float RotationSensitivity = .5f;
        public float RotationSpeed = 1;

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

        private void Awake()
        {
            _Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
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

        private Rigidbody _Rigidbody;

        private Vector2 _Direction;
        private Vector2 _Turn;
        private bool _Jump;
        private Vector3 _JumpNormal;
    }
}