namespace Krakjam
{
    using UnityEngine;
    [CreateAssetMenu(menuName = "Krakjam/OrbType")]
    public class OrbType : ScriptableObject
    {
        #region Public Variables
        public float SpeedChange => _SpeedChange;
        public int ResolutionChange => _ResolutionChange;
        #endregion Public Variables

        #region Inspector Variables
        [Range(-1.5f, 1.5f)]
        [SerializeField] private float _SpeedChange;
        [Range(-5, 5)]
        [SerializeField] private int _ResolutionChange;
        #endregion Inspector Variables
    }
}