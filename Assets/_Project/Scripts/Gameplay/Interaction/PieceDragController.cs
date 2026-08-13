using System;
using UnityEngine;
using GHSS.Core.Board;
using GHSS.Gameplay.Board;

namespace GHSS.Gameplay.Interaction
{
    /// <summary>
    /// Drag &amp; drop layer for one board piece: owns the visual side of a drag
    /// (follow the pointer, scale/tint feedback, snap back on failure) and
    /// delegates the actual outcome to <see cref="PieceDropResolver{TPiece}"/>
    /// (gameplay logic). Generic over the piece family - used for both Item and
    /// Spawner drag-merge, identical mechanics either way. Framework-agnostic:
    /// driven by plain method calls, never sees PointerEventData.
    /// </summary>
    public sealed class PieceDragController<TPiece> where TPiece : MonoBehaviour, IBoardObject, IVisualPiece
    {
        private const float DragScaleMultiplier = 1.1f;
        private const int PressedSortingOrderBonus = 10;

        private readonly TPiece _piece;
        private readonly Transform _transform;
        private readonly SpriteRenderer _visual;
        private readonly PieceDropResolver<TPiece> _dropResolver;
        private readonly BoardMergeController<TPiece> _mergeController;
        private readonly BoardFeedbackChannel _feedback;
        private readonly int _originSortingOrder;
        private readonly Color _originColor;

        private Vector3 _originPosition;
        private Vector3 _originScale;
        private TPiece _hoveredTarget;

        public PieceDragController(
            TPiece piece,
            PieceDropResolver<TPiece> dropResolver,
            BoardMergeController<TPiece> mergeController,
            BoardFeedbackChannel feedback)
        {
            _piece = piece != null ? piece : throw new ArgumentNullException(nameof(piece));
            _transform = piece.transform;
            _visual = piece.Visual;
            _dropResolver = dropResolver != null ? dropResolver : throw new ArgumentNullException(nameof(dropResolver));
            _mergeController = mergeController != null ? mergeController : throw new ArgumentNullException(nameof(mergeController));
            _feedback = feedback != null ? feedback : throw new ArgumentNullException(nameof(feedback));

            _originSortingOrder = _visual != null ? _visual.sortingOrder : 0;
            // Captured once, right after Initialize() already applied the
            // definition's own color - hover tint must return to this, not to
            // a hardcoded white, or every level would visually collapse to
            // white after its first drag.
            _originColor = _visual != null ? _visual.color : Color.white;
        }

        public void Press()
        {
            if (_visual != null)
                _visual.sortingOrder = _originSortingOrder + PressedSortingOrderBonus;
        }

        public void Release()
        {
            if (_visual != null)
                _visual.sortingOrder = _originSortingOrder;
        }

        public void BeginDrag()
        {
            _originPosition = _transform.position;
            _originScale = _transform.localScale;
            _transform.localScale = _originScale * DragScaleMultiplier;
        }

        public void Drag(Vector3 worldPosition)
        {
            _transform.position = worldPosition;
            UpdateHoverTint(worldPosition);
        }

        public void EndDrag(Vector3 worldPosition)
        {
            ClearHoverTint();
            _transform.localScale = _originScale;

            switch (_dropResolver.TryResolveDrop(_piece, worldPosition))
            {
                case PieceDropOutcome.Moved:
                case PieceDropOutcome.Merged:
                    return;

                case PieceDropOutcome.MergeRejected:
                    _feedback.NotifyActionRejected("Нельзя объединить эти объекты");
                    break;

                case PieceDropOutcome.CellOccupied:
                    _feedback.NotifyActionRejected("Клетка занята");
                    break;

                case PieceDropOutcome.OutOfBounds:
                    break;
            }

            _transform.position = _originPosition;
        }

        private void UpdateHoverTint(Vector3 worldPosition)
        {
            var target = _dropResolver.FindPieceAt(worldPosition, _piece);
            if (target == _hoveredTarget) return;

            _hoveredTarget = target;
            if (_visual == null) return;

            _visual.color = target != null
                ? (_mergeController.CanMerge(_piece, target) ? Color.green : Color.red)
                : _originColor;
        }

        private void ClearHoverTint()
        {
            _hoveredTarget = null;
            if (_visual != null)
                _visual.color = _originColor;
        }
    }
}
