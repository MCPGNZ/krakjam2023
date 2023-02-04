namespace Krakjam
{
    using UnityEngine;

    public sealed class OrbController : MonoBehaviour
    {
        public float Height = 0.5f;
        public float Speed = 0.5f;
        public AnimationCurve MovementCurve;

        public void Pickup()
        {
            Destroy(gameObject);
        }

        private void OnEnable()
        {
            _InitialPosition = transform.position;
            _Time = Random.Range(0.0f, 1.0f);
        }
        private void Update()
        {
            transform.position = _InitialPosition + Vector3.up * (MovementCurve.Evaluate(_Time * Speed) * Height);
            _Time += Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * Height);
        }

        private Vector3 _InitialPosition;
        private float _Time;
    }
}