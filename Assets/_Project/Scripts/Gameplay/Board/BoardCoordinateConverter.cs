using System;
using UnityEngine;
using GHSS.Core.Board;

namespace GHSS.Gameplay.Board
{
    /// <summary>
    /// Grid layer geometry: world position &lt;-&gt; cell coordinate, using
    /// BoardConfig's cell size/origin. BoardGrid itself stays purely index-based
    /// and knows nothing about world space - this is the only place that does.
    /// </summary>
    public sealed class BoardCoordinateConverter
    {
        private readonly BoardConfig _config;

        public BoardCoordinateConverter(BoardConfig config)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(
                _config.Origin.x + cell.x * _config.CellSize,
                _config.Origin.y + cell.y * _config.CellSize,
                0f);
        }

        public bool TryWorldToCell(Vector3 worldPosition, out Vector2Int cell)
        {
            var x = Mathf.RoundToInt((worldPosition.x - _config.Origin.x) / _config.CellSize);
            var y = Mathf.RoundToInt((worldPosition.y - _config.Origin.y) / _config.CellSize);

            cell = new Vector2Int(x, y);
            return x >= 0 && x < _config.Width && y >= 0 && y < _config.Height;
        }
    }
}
