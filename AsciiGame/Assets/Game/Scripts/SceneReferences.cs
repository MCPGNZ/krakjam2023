namespace Assets.Game.Scripts
{
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
        public static void LoadHighscore()
        {
            SceneManager.LoadScene("02. Highscore");
        }
        public static void LoadCredits()
        {
            SceneManager.LoadScene("03. Credits");
        }
    }
}