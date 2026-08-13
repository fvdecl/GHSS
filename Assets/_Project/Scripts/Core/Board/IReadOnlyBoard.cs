using System;
using UnityEngine;

namespace GHSS.Core.Board
{
    /// <summary>
    /// Read-only, event-driven view of the board for observers (UI) that must
    /// not be able to place/remove anything themselves - only <see cref="BoardGrid"/>
    /// can mutate occupancy.
    /// </summary>
    public interface IReadOnlyBoard
    {
        int Width { get; }
        int Height { get; }
        int TotalCellCount { get; }
        int FreeCellCount { get; }

        event Action<IBoardObject, Vector2Int> ObjectPlaced;
        event Action<IBoardObject, Vector2Int> ObjectRemoved;
    }
}
