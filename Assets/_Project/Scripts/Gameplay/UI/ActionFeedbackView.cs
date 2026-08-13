using System;
using UnityEngine;
using UnityEngine.UI;
using GHSS.Core.Timers;
using GHSS.Gameplay.Board;
using GHSS.Gameplay.Timers;

namespace GHSS.Gameplay.UI
{
    /// <summary>
    /// Pure presentation: shows a short-lived message when
    /// <see cref="BoardFeedbackChannel"/> reports a rejected action. Contains no
    /// rule about what is/isn't valid - it only displays the reason text it's given.
    /// The auto-hide delay reuses <see cref="CountdownTimer"/>/<see cref="TimerService"/>
    /// instead of its own Update() - one central per-frame driver for every timer
    /// in the project, this one included.
    /// </summary>
    public sealed class ActionFeedbackView : MonoBehaviour
    {
        [SerializeField] private Text messageText;
        [SerializeField] private float visibleSeconds = 1.5f;

        private BoardFeedbackChannel _feedback;
        private CountdownTimer _hideTimer;

        public void Construct(BoardFeedbackChannel feedback, TimerService timerService)
        {
            if (feedback == null) throw new ArgumentNullException(nameof(feedback));
            if (timerService == null) throw new ArgumentNullException(nameof(timerService));

            if (_feedback != null)
                _feedback.ActionRejected -= OnActionRejected;

            _feedback = feedback;
            _feedback.ActionRejected += OnActionRejected;

            _hideTimer = new CountdownTimer(visibleSeconds);
            _hideTimer.Completed += () => SetVisible(false);
            timerService.Register(_hideTimer);

            SetVisible(false);
        }

        private void OnDestroy()
        {
            if (_feedback != null)
                _feedback.ActionRejected -= OnActionRejected;
        }

        private void OnActionRejected(string reason)
        {
            if (messageText != null)
                messageText.text = reason;

            SetVisible(true);
            _hideTimer.Restart();
        }

        private void SetVisible(bool visible)
        {
            if (messageText != null)
                messageText.gameObject.SetActive(visible);
        }
    }
}
