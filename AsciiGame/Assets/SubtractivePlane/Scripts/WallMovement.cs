namespace Krakjam
{
    using UnityEngine;

    public sealed class WallMovement : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Transform _PlayerTransform;
        [SerializeField] private float _WallSpeedMovement;
        #endregion Inspector Variables
        #region Unity Methods
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            var wallPlayerVector = _PlayerTransform.position - transform.position;
            var wallPlayerDir = wallPlayerVector.normalized;

            transform.position += wallPlayerDir * _WallSpeedMovement * Time.deltaTime;
        }
        #endregion Unity Methods
    }
}