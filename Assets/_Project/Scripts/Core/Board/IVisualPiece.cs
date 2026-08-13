using UnityEngine;

namespace GHSS.Core.Board
{
    /// <summary>
    /// A board piece that exposes its own visual for drag feedback (tint,
    /// sorting order) without the drag layer needing per-family knowledge.
    /// </summary>
    public interface IVisualPiece
    {
        SpriteRenderer Visual { get; }
    }
}
