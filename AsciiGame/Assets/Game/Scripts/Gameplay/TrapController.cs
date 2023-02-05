namespace Krakjam
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class TrapController : MonoBehaviour
    {
        public PlayerController Player;

        public float Movement = 1.0f;
        public float Distance = 1.0f;

        public Transform Trap;

        [ReadOnly, ShowInInspector]
        public bool Activated
        {
            get
            {
                if (Player == null || Trap == null) { return false; }
                return Vector3.Distance(Player.transform.position, Trap.position) < Distance;
            }
        }

        private void Awake()
        {
            Player = FindObjectOfType<PlayerController>();
        }
        private void Update()
        {
            var length = Vector3.Distance(Player.transform.position, Trap.position);
            if (length < Distance)
            {
                _Progress += Time.deltaTime;
                _Progress = Mathf.Min(1.0f, _Progress);
            }

            Trap.position = transform.position + Vector3.up * _Progress * Movement;
        }
        private void OnValidate()
        {
            Player = FindObjectOfType<PlayerController>();
        }

        private void OnDrawGizmos()
        {
            if (Trap == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(Trap.position, Trap.position + Vector3.up * Movement);
            Gizmos.DrawWireSphere(Trap.position, Distance);
        }

        private float _Progress;
    }
}