namespace Krakjam
{
    using Assets.Game.Scripts;
    using UnityEngine;

    public sealed class WallMovement : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Transform _PlayerTransform;
        [SerializeField] private float _WallSpeedMovement;
        #endregion Inspector Variables
        #region Unity Methods

        // Update is called once per frame
        private void Update()
        {
            var wallPlayerVector = _PlayerTransform.position - transform.position;
            var wallPlayerDir = wallPlayerVector.normalized;

            transform.position += wallPlayerDir * _WallSpeedMovement * Time.deltaTime;

            if (Vector3.Dot(wallPlayerDir, transform.up) <= 0.0f)
            {
                EndGame();
            }
        }
        #endregion Unity Methods

        #region Private Methods
        private void EndGame()
        {
            SceneReferences.LoadHighscore();
        }
        #endregion Private Methods
    }
}