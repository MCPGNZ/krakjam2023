namespace Krakjam
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using Assets.Game.Scripts;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public sealed class MenuController : MonoBehaviour
    {
        public Button NewGame;
        public Button Credits;
        public Button Exit;

        private void Awake()
        {
            NewGame.onClick.AddListener(OnStartClicked);
            Credits.onClick.AddListener(OnCreditsClicked);
            Exit.onClick.AddListener(OnExitClicked);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { OnStartClicked(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { OnCreditsClicked(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { OnExitClicked(); }
        }

        private void OnStartClicked()
        {
            SceneReferences.LoadGameplay();
        }
        private void OnCreditsClicked()
        {
            SceneReferences.LoadCredits();
        }
        private void OnExitClicked()
        {
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }
    }
}