namespace Krakjam
{
    using UnityEngine;
    [CreateAssetMenu(menuName = "Krakjam/OrbType")]
    public class OrbType : ScriptableObject
    {
        #region Public Variables
        public float SpeedChange => _SpeedChange;
        #endregion Public Variables

        #region Inspector Variables
        [Range(-1000.0f, 1000.0f)]
        [SerializeField] private float _SpeedChange;
        #endregion Inspector Variables
    }
}