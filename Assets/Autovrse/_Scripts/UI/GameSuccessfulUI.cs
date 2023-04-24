using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Autovrse
{

    public class GameSuccessfulUI : MonoBehaviour
    {
        private Canvas _canvas;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Button _nextLevelBtn;
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
            _restartBtn.interactable = false;
            _nextLevelBtn.interactable = false;
        }
        private void OnEnable()
        {
            GameEvents.OnPlayerSuccessful += OnPlayerSuccessful;

            _restartBtn.onClick.AddListener(RestartGame);
            _nextLevelBtn.onClick.AddListener(NextLevel);
        }
        private void OnDisable()
        {
            GameEvents.OnPlayerSuccessful -= OnPlayerSuccessful;
            _restartBtn.onClick.RemoveListener(RestartGame);
            _nextLevelBtn.onClick.RemoveListener(NextLevel);
        }

        private void OnPlayerSuccessful()
        {

            ToggleUI(true);

        }
        private void ToggleUI(bool isEnabled)
        {
            GameEvents.NotifyOnUIStateChanged();
            _canvas.enabled = isEnabled;
            _restartBtn.interactable = isEnabled;
            _nextLevelBtn.interactable = isEnabled;
        }

        void RestartGame()
        {
            ToggleUI(false);
            GameEvents.NotifyOnGameRestart();
        }

        void NextLevel()
        {
            ToggleUI(false);
            GameEvents.NotifyOnGameNextLevel();
            GameEvents.NotifyOnGameRestart();
        }

    }
}
