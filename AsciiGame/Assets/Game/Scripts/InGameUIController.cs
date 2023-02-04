namespace Krakjam
{
    using TMPro;
    using UnityEngine;

    public sealed class InGameUIController : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private TextMeshProUGUI _ScoreMesh;

        #endregion Inspector Variables

        #region Public Methods
        public void UpdateScoreUI()
        {
        }
        #endregion Public Methods

        #region Unity Methods
        private void OnEnable()
        {
        }
        #endregion Unity Methods
    }
}