using UnityEngine;
using GHSS.Core.Board;

namespace GHSS.Tests.EditMode.TestSupport
{
    /// <summary>
    /// Minimal IBoardObject stand-in - proof that BoardGrid genuinely doesn't
    /// care about the concrete piece type (no Item/Spawner/MonoBehaviour needed
    /// to exercise it).
    /// </summary>
    internal sealed class FakeBoardObject : IBoardObject
    {
        public Vector2Int? BoardPosition { get; private set; }

        public void SetBoardPosition(Vector2Int? position)
        {
            BoardPosition = position;
        }
    }
}
