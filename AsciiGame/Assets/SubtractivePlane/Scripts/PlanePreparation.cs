namespace Krakjam
{
    using UnityEngine;

    [ExecuteAlways]
    public sealed class PlanePreparation : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Material _PlaneSubtractiveMaterial;

        private void Update()
        {
            _PlaneSubtractiveMaterial.SetVector(_PlanePositionId, transform.position);
            _PlaneSubtractiveMaterial.SetVector(_PlaneNormalId, -transform.up);
        }
        #endregion Inspector Variables

        #region Private Varibles
        private static readonly int _PlanePositionId = Shader.PropertyToID("_PlanePosition");
        private static readonly int _PlaneNormalId = Shader.PropertyToID("_PlaneNormal");
        #endregion Private Varibles
    }
}