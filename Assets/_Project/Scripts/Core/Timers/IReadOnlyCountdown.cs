using System;

namespace GHSS.Core.Timers
{
    /// <summary>
    /// Read-only view of a countdown, for observers (UI) that must not be able
    /// to start/tick it themselves.
    /// </summary>
    public interface IReadOnlyCountdown
    {
        float Duration { get; }
        float Remaining { get; }
        bool IsRunning { get; }

        event Action<float> Ticked;
        event Action Completed;
    }
}
