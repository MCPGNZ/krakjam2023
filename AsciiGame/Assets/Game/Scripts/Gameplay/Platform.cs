namespace Krakjam
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public sealed class Platform : MonoBehaviour
    {
        public Action Started;
        public Action Finished;

        public PlatformGate EntryGate;
        public PlatformGate ExitGate;

        public float Lenght;

        public Vector3 BeginPoint => transform.position;
        public Vector3 EndPoint => transform.position + Vector3.forward * Lenght;

        [ReadOnly]
        public bool PlayerInside;

        public void Awake()
        {
            EntryGate.Triggered += OnEntry;
            ExitGate.Triggered += OnExit;
        }

        private void OnValidate()
        {
            if (EntryGate != null)
            {
                EntryGate.Color = Color.green;
            }
            if (ExitGate != null)
            {
                ExitGate.Color = Color.red;
            }
        }

        private void OnEntry()
        {
            Started?.Invoke();
            PlayerInside = true;
        }
        private void OnExit()
        {
            PlayerInside = false;
            Finished?.Invoke();
        }

        private void OnDrawGizmos()
        {
            float size = 5.0f;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position + Vector3.forward * Lenght / 2.0f, new Vector3(size, size, Lenght));
            Gizmos.color = Color.magenta * new Vector4(1.0f, 1.0f, 1.0f, 0.2f);
            Gizmos.DrawCube(transform.position + Vector3.forward * Lenght / 2.0f, new Vector3(size, size, Lenght));
        }
    }
}