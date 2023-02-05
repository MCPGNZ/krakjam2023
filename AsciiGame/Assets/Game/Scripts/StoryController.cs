namespace Krakjam
{
    using UnityEngine;
    using TMPro;
    using System.Collections.Generic;
    using System;

    public sealed class StoryController : MonoBehaviour
    {
        #region Public Variables

        public bool ShowStory;
        public Action OnStartClick;
        public TextMeshProUGUI CurrentVisibleStoryBoard;

        public int CurrentIndex => _CurrentIndex;
        #endregion Public Variables

        #region Inspector Variables
        [SerializeField] private Canvas _MenuCanvas;
        [SerializeField] private Canvas _StoryboardCanvas;
        [SerializeField] private List<GameObject> _Frames;
        [SerializeField] private float _StoryBoardShowTime;
        [SerializeField] private List<string> _StoryBoards;
        #endregion Inspector Variables

        #region Unity Methods
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (ShowStory)
            {
                if (_FramesDisabled)
                {
                    _MenuCanvas.enabled = false;
                    _StoryboardCanvas.enabled = true;
                    foreach (var frame in _Frames)
                    {
                        frame.SetActive(false);
                    }
                }
                if (!_FramesDisabled)
                {
                    _FramesDisabled = true;
                }
                UpdateStoryboards();
            }
        }
        #endregion Unity Methods

        #region Private Methods
        private void Initialize()
        {
            if (_StoryBoards.Count == 0)
            {
                Debug.LogError("string for your story board");
                return;
            }
            _CurrentIndex = 0;
            _CurrentStoryBoardTimer = _StoryBoardShowTime;
            CurrentVisibleStoryBoard.text = _StoryBoards[_CurrentIndex];
        }

        private void UpdateStoryboards()
        {
            if (_StoryBoards.Count <= _CurrentIndex)
            {
                OnStartClick?.Invoke();
                return;
            }
            if (_CurrentStoryBoardTimer <= 0.0f)
            {
                _CurrentStoryBoardTimer = _StoryBoardShowTime;
                _CurrentIndex++;
                if (_CurrentIndex < _StoryBoards.Count)
                {
                    CurrentVisibleStoryBoard.text = _StoryBoards[_CurrentIndex];
                }
            }
            _CurrentStoryBoardTimer -= Time.deltaTime;
        }
        #endregion Private Methods

        #region Private Variables
        private float _CurrentStoryBoardTimer;
        private int _CurrentIndex;
        private bool _FramesDisabled = false;
        #endregion Private Variables
    }
}