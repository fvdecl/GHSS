using UnityEngine;

namespace GHSS.Core.Board
{
    /// <summary>
    /// Contract for anything that can occupy a cell on a <see cref="BoardGrid"/>.
    /// The grid depends only on this interface, never on a concrete item/spawner type.
    /// </summary>
    public interface IBoardObject
    {
        Vector2Int? BoardPosition { get; }

        void SetBoardPosition(Vector2Int? position);
    }
}
