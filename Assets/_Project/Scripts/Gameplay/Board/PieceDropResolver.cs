using System;
using System.Collections.Generic;
using UnityEngine;
using GHSS.Core.Board;

namespace GHSS.Gameplay.Board
{
    public enum PieceDropOutcome
    {
        Moved,
        Merged,
        MergeRejected,
        CellOccupied,
        OutOfBounds
    }

    /// <summary>
    /// Gameplay-logic layer for dropping a board piece: combines Grid
    /// (BoardGrid + BoardCoordinateConverter) and Merge (BoardMergeController)
    /// to decide merge vs move vs reject. Generic over the piece family (Item,
    /// Spawner, ...) - both share the exact same drop rules, only the merge
    /// service backing BoardMergeController differs. Knows nothing about input,
    /// pointers or drag visuals - callable directly from tests.
    /// </summary>
    public sealed class PieceDropResolver<TPiece> where TPiece : MonoBehaviour, IBoardObject
    {
        private readonly BoardGrid _board;
        private readonly BoardCoordinateConverter _coordinates;
        private readonly BoardMergeController<TPiece> _mergeController;
        private readonly List<Collider2D> _hitsBuffer = new();
        private readonly ContactFilter2D _filter = ContactFilter2D.noFilter;

        public PieceDropResolver(BoardGrid board, BoardCoordinateConverter coordinates, BoardMergeController<TPiece> mergeController)
        {
            _board = board != null ? board : throw new ArgumentNullException(nameof(board));
            _coordinates = coordinates != null ? coordinates : throw new ArgumentNullException(nameof(coordinates));
            _mergeController = mergeController != null ? mergeController : throw new ArgumentNullException(nameof(mergeController));
        }

        public PieceDropOutcome TryResolveDrop(TPiece piece, Vector3 worldPosition)
        {
            var target = FindPieceAt(worldPosition, piece);
            if (target != null)
                return _mergeController.TryMergeOnBoard(piece, target) ? PieceDropOutcome.Merged : PieceDropOutcome.MergeRejected;

            if (!_coordinates.TryWorldToCell(worldPosition, out var cell))
                return PieceDropOutcome.OutOfBounds;

            var droppedOnOwnCell = piece.BoardPosition == cell;
            if (!droppedOnOwnCell && !_board.IsFree(cell))
                return PieceDropOutcome.CellOccupied;

            if (!droppedOnOwnCell)
            {
                _board.Remove(piece);
                _board.TryPlace(piece, cell);
            }

            piece.transform.position = _coordinates.CellToWorld(cell);
            return PieceDropOutcome.Moved;
        }

        /// <summary>Which piece (if any) of this same family sits at a world point. Public so drag-visual hover feedback can reuse the exact same lookup rule.</summary>
        public TPiece FindPieceAt(Vector3 worldPosition, TPiece exclude)
        {
            _hitsBuffer.Clear();
            Physics2D.OverlapPoint(worldPosition, _filter, _hitsBuffer);

            foreach (var hit in _hitsBuffer)
            {
                var candidate = hit.GetComponentInParent<TPiece>();
                if (candidate != null && candidate != exclude)
                    return candidate;
            }

            return null;
        }
    }
}
