using System;
using UnityEngine;
using UnityEngine.UI;
using GHSS.Core.Board;

namespace GHSS.Gameplay.UI
{
    /// <summary>
    /// Pure presentation of board occupancy ("Free cells: X/Y"). Depends only on
    /// the read-only <see cref="IReadOnlyBoard"/> - cannot place or remove
    /// anything, only observe. Updates from board events, no polling.
    /// </summary>
    public sealed class BoardStateView : MonoBehaviour
    {
        [SerializeField] private Text stateText;

        private IReadOnlyBoard _board;

        public void Construct(IReadOnlyBoard board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));

            if (_board != null)
            {
                _board.ObjectPlaced -= OnBoardChanged;
                _board.ObjectRemoved -= OnBoardChanged;
            }

            _board = board;
            _board.ObjectPlaced += OnBoardChanged;
            _board.ObjectRemoved += OnBoardChanged;

            Refresh();
        }

        private void OnDestroy()
        {
            if (_board == null) return;

            _board.ObjectPlaced -= OnBoardChanged;
            _board.ObjectRemoved -= OnBoardChanged;
        }

        private void OnBoardChanged(IBoardObject boardObject, Vector2Int coord) => Refresh();

        private void Refresh()
        {
            if (stateText != null)
                stateText.text = $"Free cells: {_board.FreeCellCount}/{_board.TotalCellCount}";
        }
    }
}
