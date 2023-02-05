namespace Assets.Game.Scripts
{
    using System.Threading.Tasks;
    using Krakjam;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class SceneReferences
    {
        public static void LoadMenu()
        {
            SceneManager.LoadScene("00. Menu");
        }
        public static void LoadGameplay()
        {
            SceneManager.LoadScene("01. Gameplay");
        }

        public static async void LoadHighscore(bool input)
        {
            if (input == false)
            {
                SceneManager.sceneLoaded += OnHighscoreLoaded;
            }

            SceneManager.LoadScene("02. Highscore");
        }

        private static void OnHighscoreLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= OnHighscoreLoaded;

            var controller = Object.FindObjectOfType<HighscoreController>();
            controller.SwitchToScores();
        }

        public static void LoadCredits()
        {
            SceneManager.LoadScene("03. Credits");
        }
    }
}