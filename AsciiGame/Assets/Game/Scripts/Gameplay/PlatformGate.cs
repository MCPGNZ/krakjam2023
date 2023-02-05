namespace Krakjam
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(BoxCollider))]
    public class PlatformGate : MonoBehaviour
    {
        public Action Triggered;
        public Color Color = Color.white;

        private void OnDrawGizmos()
        {
            var boxCollider = GetComponent<BoxCollider>();

            Gizmos.color = Color;
            Gizmos.DrawWireCube(transform.position + boxCollider.center, Vector3.Scale(transform.lossyScale, boxCollider.size));
            Gizmos.color = Color * new Vector4(1.0f, 1.0f, 1.0f, 0.2f);
            Gizmos.DrawCube(transform.position + boxCollider.center, Vector3.Scale(transform.lossyScale, boxCollider.size));
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() == null)
            {
                return;
            }
            Triggered.Invoke();
        }
    }
}