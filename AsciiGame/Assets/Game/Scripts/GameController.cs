namespace Krakjam
{
    using Assets.Game.Scripts;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public static class Game
    {
        public static int Score;
    }

    public sealed class GameController : MonoBehaviour
    {
        public void OnEnable()
        {
            BeginGame();
        }

        private void BeginGame()
        {
            Game.Score = 0;
        }

        [Button]
        private void EndGame()
        {
            SceneReferences.LoadHighscore();
        }
    }
}