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
        public StoryController StoryBoardController;
        public Button NewGame;
        public Button Credits;
        public Button Highscore;
        public Button Exit;

        private void Awake()
        {
            NewGame.onClick.AddListener(OnStartClicked);
            Credits.onClick.AddListener(OnCreditsClicked);
            Highscore.onClick.AddListener(OnHighscoreClicked);
            Exit.onClick.AddListener(OnExitClicked);
        }
        public void OnEnable()
        {
            MusicController.PlayAmbient();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { OnStartClicked(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { OnCreditsClicked(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { OnHighscoreClicked(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { OnExitClicked(); }
        }

        private void OnStartClicked()
        {
            StoryBoardController.DisableInput();
            StoryBoardController.OnStartClick += StartGame;
            StoryBoardController.ShowStory = true;
        }
        private void OnHighscoreClicked()
        {
            SceneReferences.LoadHighscore(false);
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

        private void StartGame()
        {
            StoryBoardController.EnableInput();
            SceneReferences.LoadGameplay();
        }
    }
}