using System;
using UnityEngine;
using GHSS.Core.Board;

namespace GHSS.Gameplay.Board
{
    /// <summary>
    /// Board-aware merge for any mergeable piece family: validates through the
    /// family's <see cref="IMergeService{TPiece}"/>, then moves the board cells.
    /// Used for items and spawners alike - no per-family duplication. Knows
    /// nothing about pointers/drag or any other input scheme.
    /// </summary>
    public sealed class BoardMergeController<TPiece> where TPiece : MonoBehaviour, IBoardObject
    {
        private readonly BoardGrid _board;
        private readonly IMergeService<TPiece> _mergeService;

        public BoardMergeController(BoardGrid board, IMergeService<TPiece> mergeService)
        {
            _board = board != null ? board : throw new ArgumentNullException(nameof(board));
            _mergeService = mergeService != null ? mergeService : throw new ArgumentNullException(nameof(mergeService));
        }

        public bool CanMerge(TPiece source, TPiece target) => _mergeService.CanMerge(source, target);

        public bool TryMergeOnBoard(TPiece source, TPiece target)
        {
            if (source == null || target == null || source == target) return false;
            if (source.BoardPosition == null || target.BoardPosition == null) return false;
            if (!_mergeService.CanMerge(source, target)) return false;

            var sourceCoord = source.BoardPosition.Value;
            var targetCoord = target.BoardPosition.Value;
            var targetWorldPosition = target.transform.position;

            _board.Remove(source);
            _board.Remove(target);

            if (!_mergeService.TryMerge(source, target, out var result))
            {
                _board.TryPlace(source, sourceCoord);
                _board.TryPlace(target, targetCoord);
                return false;
            }

            result.transform.position = targetWorldPosition;
            _board.TryPlace(result, targetCoord);
            return true;
        }
    }
}
