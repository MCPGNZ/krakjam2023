namespace Krakjam
{
    using UnityEngine;
    [CreateAssetMenu(menuName = "Krakjam/OrbType")]
    public class OrbType : ScriptableObject
    {
        #region Public Variables
        public int ResolutionChangeRate => _ResolutionChangeRate;
        #endregion Public Variables

        #region Inspector Variables
        [Range(0, 100)]
        [SerializeField] private int _ResolutionChangeRate;
        #endregion Inspector Variables
    }
}