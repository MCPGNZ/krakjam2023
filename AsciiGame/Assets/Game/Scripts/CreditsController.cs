namespace Krakjam
{
    using Assets.Game.Scripts;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class CreditsController : MonoBehaviour
    {
        public Button Return;

        public void OnReturn()
        {
            SceneReferences.LoadMenu();
        }
        private void Awake()
        {
            if (Return != null) { Return.onClick.AddListener(OnReturn); }
        }
    }
}