using System;
using System.Collections.Generic;
using UnityEngine;
using GHSS.Core.Common;

namespace GHSS.Core.Board
{
    /// <summary>
    /// Logical model of the play field: a 2D array of <see cref="BoardCell"/>.
    /// Knows nothing about visuals, input or specific object types (items, spawners, ...).
    /// </summary>
    public sealed class BoardGrid : IReadOnlyBoard
    {
        private readonly BoardCell[,] _cells;

        public int Width { get; }
        public int Height { get; }
        public int TotalCellCount => Width * Height;
        public int FreeCellCount { get; private set; }

        public event Action<IBoardObject, Vector2Int> ObjectPlaced;
        public event Action<IBoardObject, Vector2Int> ObjectRemoved;

        public BoardGrid(BoardConfig config) : this(config.Width, config.Height)
        {
        }

        public BoardGrid(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            _cells = new BoardCell[width, height];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                _cells[x, y] = new BoardCell(new Vector2Int(x, y));

            FreeCellCount = TotalCellCount;
        }

        public bool IsInside(Vector2Int coord) =>
            coord.x >= 0 && coord.x < Width && coord.y >= 0 && coord.y < Height;

        public BoardCell GetCell(Vector2Int coord)
        {
            if (!IsInside(coord))
                throw new ArgumentOutOfRangeException(nameof(coord), coord, "Coordinate is outside the board.");

            return _cells[coord.x, coord.y];
        }

        public bool IsFree(Vector2Int coord) => IsInside(coord) && GetCell(coord).IsFree;

        public IBoardObject GetObjectAt(Vector2Int coord) =>
            IsInside(coord) ? GetCell(coord).Occupant : null;

        public bool TryPlace(IBoardObject boardObject, Vector2Int coord)
        {
            if (boardObject == null) throw new ArgumentNullException(nameof(boardObject));
            if (!IsInside(coord)) return false;

            var cell = GetCell(coord);
            if (!cell.IsFree) return false;

            cell.Occupant = boardObject;
            boardObject.SetBoardPosition(coord);
            FreeCellCount--;
            ObjectPlaced?.Invoke(boardObject, coord);
            return true;
        }

        public bool Remove(IBoardObject boardObject)
        {
            if (boardObject == null) throw new ArgumentNullException(nameof(boardObject));

            var position = boardObject.BoardPosition;
            if (position == null || !IsInside(position.Value)) return false;

            var cell = GetCell(position.Value);
            if (cell.Occupant != boardObject) return false;

            var removedCoord = cell.Coordinate;
            cell.Occupant = null;
            boardObject.SetBoardPosition(null);
            FreeCellCount++;
            ObjectRemoved?.Invoke(boardObject, removedCoord);
            return true;
        }

        public IEnumerable<Vector2Int> GetFreeCells()
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                if (_cells[x, y].IsFree)
                    yield return _cells[x, y].Coordinate;
        }

        /// <summary>
        /// A uniformly random free cell, not just the first one a scan happens
        /// to find - otherwise every spawn would pile up in the same corner
        /// while the rest of the board stays empty. Single pass, no allocation
        /// (reservoir sampling), so it's no more expensive than GetFreeCells().
        /// </summary>
        public bool TryGetRandomFreeCell(IRandomSource random, out Vector2Int cell)
        {
            cell = default;
            var found = false;
            var seen = 0;

            foreach (var candidate in GetFreeCells())
            {
                seen++;
                if (random.NextFloat01() < 1f / seen)
                {
                    cell = candidate;
                    found = true;
                }
            }

            return found;
        }
    }
}
