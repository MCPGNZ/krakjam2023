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
        [SerializeField] private float _SpeedChange;
        [SerializeField] private int _ResolutionChange;
        #endregion Inspector Variables
    }
}