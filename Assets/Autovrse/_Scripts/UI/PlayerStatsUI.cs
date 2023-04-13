
using UnityEngine;
using TMPro;
namespace Autovrse
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _healthValueText;
        private void OnEnable()
        {
            GameEvents.OnPlayerHealthChanged += OnPlayerHealthChanged;
        }
        private void OnDisable()
        {
            GameEvents.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        }

        private void OnPlayerHealthChanged(float newHealthValue)
        {
            _healthValueText.text = newHealthValue.ToString();
        }
    }
}
