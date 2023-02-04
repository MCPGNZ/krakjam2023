namespace Krakjam
{
    using Assets.Game.Scripts;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class CreditsController : MonoBehaviour
    {
        public Button Return;

        private void Awake()
        {
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { OnReturnClicked(); }
        }

        private void OnReturnClicked()
        {
            SceneReferences.LoadMenu();
        }
    }
}