
using UnityEngine;
using TMPro;
using System.Collections;

namespace Autovrse
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerValueText;
        [SerializeField] private TextMeshProUGUI _storyText;
        private Coroutine _timerCoroutine = null;
        [SerializeField] private GameObject _timeInfo;
        [SerializeField] private float _timeInMinute = 3;
        private void Start()
        {
            _timeInfo.SetActive(false);
        }
        private void OnEnable()
        {
            GameEvents.OnGameStart += StartTimer;
            GameEvents.OnGameRestart += StartTimer;
            GameEvents.OnPlayerSuccessful += StopTimer;
            GameEvents.OnPlayerUnSuccessful += StopTimer;
        }
        private void OnDisable()
        {
            GameEvents.OnGameStart -= StartTimer;
            GameEvents.OnGameRestart -= StartTimer;
            GameEvents.OnPlayerSuccessful -= StopTimer;
            GameEvents.OnPlayerUnSuccessful -= StopTimer;
        }

        private void StartTimer()
        {
            _timeInfo.SetActive(true);
            _storyText.enabled = false;
            if (_timerCoroutine == null)
                _timerCoroutine = StartCoroutine(TimerSequence());

        }

        private void StopTimer()
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
            _timerValueText.text = string.Empty;
        }

        IEnumerator TimerSequence()
        {
            // Time in seconds
            float timer = _timeInMinute * 60;
            float minutes, seconds;
            while (timer > 0)
            {
                minutes = Mathf.FloorToInt(timer / 60);
                seconds = Mathf.FloorToInt(timer % 60);
                timer -= Time.deltaTime;
                _timerValueText.text = minutes + ":" + seconds;
                yield return null;
            }
            GameEvents.NotifyOnPlayerUnSuccessful();
        }
    }
}
