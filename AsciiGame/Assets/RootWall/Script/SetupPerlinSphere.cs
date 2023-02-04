namespace Krakjam
{
    using UnityEngine;

    public sealed class SetupPerlinSphere : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Material _PerlinNoiseMovement;
        [SerializeField] private float _PerlinNoiseFrequency = 2.0f;
        [SerializeField] private float _PerlinNoiseAmplitude = 0.5f;
        #endregion Inspector Variables
        #region Unity Methods

        private void OnValidate()
        {
#if UNITY_EDITOR
            _PerlinNoiseMovement.SetFloat(_PerlinFrequencyId, _PerlinNoiseFrequency);
            _PerlinNoiseMovement.SetFloat(_PerlinAmplitudeId, _PerlinNoiseAmplitude);
#endif
        }

        #endregion Unity Methods

        #region Private Variables
        private static readonly int _PerlinFrequencyId = Shader.PropertyToID("_PerlinFrequency");
        private static readonly int _PerlinAmplitudeId = Shader.PropertyToID("_PerlinAmplitude");
        #endregion Private Variables
    }
}