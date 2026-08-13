using UnityEngine;

namespace GHSS.Core.Board
{
    /// <summary>
    /// A single slot of the board. Knows only its coordinate and current occupant.
    /// Mutated exclusively through <see cref="BoardGrid"/> to keep occupancy consistent.
    /// </summary>
    public sealed class BoardCell
    {
        public Vector2Int Coordinate { get; }
        public IBoardObject Occupant { get; internal set; }
        public bool IsFree => Occupant == null;

        public BoardCell(Vector2Int coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
