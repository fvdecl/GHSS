using System;
using UnityEngine;
using UnityEngine.UI;
using GHSS.Core.Timers;

namespace GHSS.Gameplay.UI
{
    /// <summary>
    /// Pure presentation: button + remaining-time label. Knows nothing about the
    /// board, spawners or probabilities - only <see cref="ISpawnTimer"/>. Contains
    /// no game rules, just wiring between widgets and the timer's own events.
    /// </summary>
    public sealed class SpawnTimerView : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Text remainingTimeText;

        private ISpawnTimer _spawnTimer;

        public void Construct(ISpawnTimer spawnTimer)
        {
            if (spawnTimer == null) throw new ArgumentNullException(nameof(spawnTimer));

            if (_spawnTimer != null)
            {
                _spawnTimer.Countdown.Ticked -= OnTicked;
                _spawnTimer.Countdown.Completed -= OnCompleted;
            }

            _spawnTimer = spawnTimer;
            _spawnTimer.Countdown.Ticked += OnTicked;
            _spawnTimer.Countdown.Completed += OnCompleted;

            RefreshText(_spawnTimer.Countdown.Remaining);
            startButton.interactable = !_spawnTimer.Countdown.IsRunning;
        }

        private void OnEnable()
        {
            startButton.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(HandleButtonClicked);
        }

        private void OnDestroy()
        {
            if (_spawnTimer == null) return;

            _spawnTimer.Countdown.Ticked -= OnTicked;
            _spawnTimer.Countdown.Completed -= OnCompleted;
        }

        private void HandleButtonClicked()
        {
            startButton.interactable = false;
            _spawnTimer.StartCountdown();
        }

        private void OnTicked(float remaining) => RefreshText(remaining);

        private void OnCompleted() => startButton.interactable = true;

        private void RefreshText(float remaining)
        {
            if (remainingTimeText != null)
                remainingTimeText.text = Mathf.CeilToInt(remaining).ToString();
        }
    }
}
