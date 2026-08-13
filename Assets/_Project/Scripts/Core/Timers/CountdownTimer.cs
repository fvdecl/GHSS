using System;
using UnityEngine;
using GHSS.Core.Common;

namespace GHSS.Core.Timers
{
    /// <summary>
    /// A plain countdown: duration comes from data (<see cref="TimerConfig"/>),
    /// advancing is driven from the outside via <see cref="Tick"/> - this class
    /// has no Update() of its own. One of possibly several ITickable timer types.
    /// </summary>
    public sealed class CountdownTimer : ITickable, IReadOnlyCountdown
    {
        public float Duration { get; }
        public float Remaining { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action<float> Ticked;
        public event Action Completed;

        public CountdownTimer(float duration)
        {
            Duration = Mathf.Max(0f, duration);
            Remaining = Duration;
        }

        /// <summary>Starts the countdown, unless it's already running (protects
        /// against restarting an in-progress player-facing countdown).</summary>
        public void Start()
        {
            if (IsRunning) return;

            Restart();
        }

        /// <summary>Resets the countdown to full duration and (re)starts it,
        /// even if it was already running - for timers meant to restart on
        /// every trigger (e.g. an auto-hide delay retriggered by a new event).</summary>
        public void Restart()
        {
            Remaining = Duration;
            IsRunning = true;
            Ticked?.Invoke(Remaining);
        }

        public void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            Remaining = Mathf.Max(0f, Remaining - deltaTime);
            Ticked?.Invoke(Remaining);

            if (Remaining <= 0f)
            {
                IsRunning = false;
                Completed?.Invoke();
            }
        }
    }
}
