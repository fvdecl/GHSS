namespace GHSS.Core.Timers
{
    /// <summary>
    /// Everything the UI is allowed to know about the spawn countdown: how to
    /// read it and how to start it. Nothing about board or spawner types leaks
    /// through this interface.
    /// </summary>
    public interface ISpawnTimer
    {
        IReadOnlyCountdown Countdown { get; }

        void StartCountdown();
    }
}
