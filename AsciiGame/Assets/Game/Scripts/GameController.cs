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
        public PlayerController Player;

        public void OnEnable()
        {
            Player.OnDeath += OnPlayerDeath;
            BeginGame();
        }
        private void OnDisable()
        {
            Player.OnDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            EndGame();
        }

        private void BeginGame()
        {
            Game.Score = 0;
        }

        [Button]
        private void EndGame()
        {
            SceneReferences.LoadHighscore(true);
        }
    }
}