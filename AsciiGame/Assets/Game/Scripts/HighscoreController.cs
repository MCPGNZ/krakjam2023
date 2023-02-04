namespace Krakjam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Assets.Game.Scripts;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [Serializable]
    public sealed class HighscoreDatabase
    {
        [Serializable]
        public sealed class Entry
        {
            public string Name;
            public int Score;
        }

        public List<Entry> Entries = new List<Entry>();
    }

    public sealed class HighscoreController : MonoBehaviour
    {
        public enum State
        {
            Input,
            Show
        }

        public State CurrentState;
        public HighscoreDatabase Database;

        /* input state */
        public TMP_InputField InputField;
        public GameObject InputGameObject;

        /* highscore state */
        public TMP_Text HighscoreField;
        public GameObject HighscoreGameObject;
        public Button Return;

        private void Awake()
        {
            Return.onClick.AddListener(OnReturnClicked);

            InputField.onSubmit.AddListener(HighscoreSubmitted);
            InputField.Select();

            if (PlayerPrefs.HasKey(_HighscoreId))
            {
                var json = PlayerPrefs.GetString(_HighscoreId);
                Database = JsonUtility.FromJson<HighscoreDatabase>(json);

                Debug.Log($"Loaded highscores: {json}");
            }
        }
        private void OnDestroy()
        {
            var json = JsonUtility.ToJson(Database);
            PlayerPrefs.SetString(_HighscoreId, json);
            Debug.Log($"Saved highscores: {json}");
        }

        private void OnEnable()
        {
            CurrentState = State.Input;
            ShowInput();
        }
        private void Update()
        {
            switch (CurrentState)
            {
                case State.Input:
                    break;
                case State.Show:
                    {
                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            SceneReferences.LoadMenu();
                        }
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnReturnClicked()
        {
            SceneReferences.LoadMenu();
        }
        private void HighscoreSubmitted(string name)
        {
            Database.Entries.Add(new HighscoreDatabase.Entry { Name = name, Score = Game.Score });
            CurrentState = State.Show;
            ShowHighscore();

            Debug.Log($"Highscore submitted: {name} : {Game.Score}");
        }

        private void ShowInput()
        {
            InputGameObject.SetActive(true);
            HighscoreGameObject.SetActive(false);
        }
        private void ShowHighscore()
        {
            InputGameObject.SetActive(false);
            HighscoreGameObject.SetActive(true);

            var stringBuilder = new StringBuilder();
            var entries = Database.Entries.OrderByDescending(x => x.Score);
            foreach (var entry in entries)
            {
                stringBuilder.AppendLine($"{entry.Name} : {entry.Score}");
            }
            HighscoreField.SetText(stringBuilder.ToString());
        }

        [Sirenix.OdinInspector.Button]
        private void ClearDatabase()
        {
            Database.Entries.Clear();
        }

        private const string _HighscoreId = "Highscore";
    }
}