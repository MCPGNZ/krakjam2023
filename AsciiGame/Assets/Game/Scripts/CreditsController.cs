namespace Krakjam
{
    using Assets.Game.Scripts;
    using UnityEngine;

    public sealed class CreditsController : MonoBehaviour
    {
        public void OnReturn()
        {
            SceneReferences.LoadMenu();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { OnReturn(); }
        }
    }
}