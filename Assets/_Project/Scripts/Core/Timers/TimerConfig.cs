using UnityEngine;

namespace GHSS.Core.Timers
{
    [CreateAssetMenu(fileName = "TimerConfig", menuName = "GHSS/Timers/Timer Config")]
    public sealed class TimerConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float durationSeconds = 30f;

        public float DurationSeconds => durationSeconds;
    }
}
