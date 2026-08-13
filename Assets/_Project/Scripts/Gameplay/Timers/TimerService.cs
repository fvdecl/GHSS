using System.Collections.Generic;
using UnityEngine;
using GHSS.Core.Common;

namespace GHSS.Gameplay.Timers
{
    /// <summary>
    /// The only Update() any timer needs. Any number of ITickable instances -
    /// countdowns today, other timer types later - register here instead of
    /// running their own Update.
    /// </summary>
    public sealed class TimerService : MonoBehaviour
    {
        private readonly List<ITickable> _tickables = new();

        public void Register(ITickable tickable)
        {
            if (tickable != null && !_tickables.Contains(tickable))
                _tickables.Add(tickable);
        }

        public void Unregister(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            // Backwards so a Tick() that registers/unregisters a tickable (its own
            // Completed handler, say) can never shift a not-yet-processed index
            // out from under this loop.
            for (var i = _tickables.Count - 1; i >= 0; i--)
                _tickables[i].Tick(deltaTime);
        }
    }
}
