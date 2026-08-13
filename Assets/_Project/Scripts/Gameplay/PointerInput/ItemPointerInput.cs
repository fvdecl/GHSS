using UnityEngine;
using UnityEngine.EventSystems;
using GHSS.Core.Items;
using GHSS.Gameplay.Board;
using GHSS.Gameplay.Interaction;

namespace GHSS.Gameplay.PointerInput
{
    /// <summary>
    /// Input layer for one item - the only class in the whole item-interaction
    /// stack that references PointerEventData/EventSystems. It converts
    /// screen-space pointer events to plain world-space calls on
    /// <see cref="PieceDragController{TPiece}"/> (drag &amp; drop layer), which
    /// never sees PointerEventData itself.
    ///
    /// To move from mouse to touch: EventSystem already dispatches these same
    /// interfaces for touch input (via a touch-capable input module), so in most
    /// cases nothing changes at all. If a fully custom touch scheme is ever
    /// needed, only this component gets replaced - PieceDragController,
    /// PieceDropResolver, BoardGrid and the merge system stay untouched.
    /// </summary>
    [RequireComponent(typeof(Item))]
    public sealed class ItemPointerInput : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Item _item;
        private Camera _camera;
        private PieceDragController<Item> _dragController;

        public void Construct(
            PieceDropResolver<Item> dropResolver,
            BoardMergeController<Item> mergeController,
            BoardFeedbackChannel feedback,
            Camera camera)
        {
            // Idempotent: TryPlace fires ObjectPlaced on every successful
            // placement, including relocating an already-wired piece to a new
            // cell (drag-to-empty-cell). Re-building the drag controller mid-drag
            // would drop its in-progress state (e.g. the sorting order captured
            // by Press()), so a second Construct on an already-wired instance is a no-op.
            if (_dragController != null) return;

            _camera = camera;
            _dragController = new PieceDragController<Item>(_item, dropResolver, mergeController, feedback);
        }

        private void Awake()
        {
            _item = GetComponent<Item>();
        }

        public void OnPointerDown(PointerEventData eventData) => _dragController.Press();

        public void OnPointerUp(PointerEventData eventData) => _dragController.Release();

        public void OnBeginDrag(PointerEventData eventData) => _dragController.BeginDrag();

        public void OnDrag(PointerEventData eventData) => _dragController.Drag(ScreenToWorld(eventData.position));

        public void OnEndDrag(PointerEventData eventData) => _dragController.EndDrag(ScreenToWorld(eventData.position));

        private Vector3 ScreenToWorld(Vector2 screenPosition)
        {
            var world = _camera.ScreenToWorldPoint(screenPosition);
            world.z = transform.position.z;
            return world;
        }
    }
}
