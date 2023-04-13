using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Autovrse
{

    public class GameOverUI : MonoBehaviour
    {
        private Canvas _canvas;
        [SerializeField] private Button _restartBtn;
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
            _restartBtn.interactable = false;
        }
        private void OnEnable()
        {
            GameEvents.OnPlayerDie += OnPlayerDie;
            _restartBtn.onClick.AddListener(RestartGame);
        }
        private void OnDisable()
        {
            GameEvents.OnPlayerDie -= OnPlayerDie;
            _restartBtn.onClick.RemoveListener(RestartGame);
        }

        private void OnPlayerDie()
        {

            ToggleUI(true);

        }
        private void ToggleUI(bool isEnabled)
        {
            GameEvents.OnUIStateChanged();
            _canvas.enabled = isEnabled;
            _restartBtn.interactable = isEnabled;
        }

        void RestartGame()
        {
            ToggleUI(false);
            GameEvents.NotifyOnGameRestart();
        }

    }
}
