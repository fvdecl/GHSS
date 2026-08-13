using UnityEngine;

namespace GHSS.Core.Timers
{
    /// <summary>
    /// Data for one timer-driven spawn: how long to count down (a reusable
    /// <see cref="TimerConfig"/>) and which spawner level appears when it
    /// completes. Keeping "how long" and "what appears" as separate assets lets
    /// several timers share a duration, or the same spawn level use a different
    /// duration, without touching C#.
    /// </summary>
    [CreateAssetMenu(fileName = "TimedSpawnerConfig", menuName = "GHSS/Timers/Timed Spawner Config")]
    public sealed class TimedSpawnerConfig : ScriptableObject
    {
        [SerializeField] private TimerConfig timer;
        [SerializeField, Min(1)] private int spawnerLevel = 1;

        public TimerConfig Timer => timer;
        public int SpawnerLevel => spawnerLevel;
    }
}
