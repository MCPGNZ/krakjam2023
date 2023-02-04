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
            _ScoreMesh.SetText($"Score: {Game.Score}");
        }
        #endregion Public Methods

        #region Unity Methods
        private void OnEnable()
        {
            _ScoreMesh.SetText($"Score: {Game.Score}");
        }
        #endregion Unity Methods
    }
}