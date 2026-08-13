using System;

namespace GHSS.Gameplay.Board
{
    /// <summary>
    /// Single shared channel for "the player tried to do something and it was
    /// rejected" (invalid merge, board full, ...). Input adapters and controllers
    /// report into it; UI subscribes to this one object instead of discovering
    /// every board piece individually.
    /// </summary>
    public sealed class BoardFeedbackChannel
    {
        public event Action<string> ActionRejected;

        public void NotifyActionRejected(string reason) => ActionRejected?.Invoke(reason);
    }
}
