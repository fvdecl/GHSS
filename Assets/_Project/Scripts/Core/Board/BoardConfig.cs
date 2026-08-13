using UnityEngine;

namespace GHSS.Core.Board
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "GHSS/Board/Board Config")]
    public sealed class BoardConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int width = 7;
        [SerializeField, Min(1)] private int height = 9;
        [SerializeField, Min(0.01f)] private float cellSize = 1f;
        [SerializeField] private Vector2 origin = Vector2.zero;

        public int Width => width;
        public int Height => height;

        /// <summary>World-space size of one cell, and the world position of cell (0,0).
        /// Used only to convert between world positions and cell coordinates
        /// (see BoardCoordinateConverter) - BoardGrid itself stays index-based.</summary>
        public float CellSize => cellSize;
        public Vector2 Origin => origin;
    }
}
