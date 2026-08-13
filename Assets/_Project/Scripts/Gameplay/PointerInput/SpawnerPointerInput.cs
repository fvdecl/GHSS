using UnityEngine;
using UnityEngine.EventSystems;
using GHSS.Core.Spawners;
using GHSS.Gameplay.Board;
using GHSS.Gameplay.Interaction;
using GHSS.Gameplay.Spawners;

namespace GHSS.Gameplay.PointerInput
{
    /// <summary>
    /// Input layer for one spawner - the only class here that references
    /// PointerEventData/EventSystems. A spawner has two independent gestures:
    /// a plain click activates it (<see cref="SpawnerActivationController"/>),
    /// a drag merges/moves it (<see cref="PieceDragController{TPiece}"/>).
    ///
    /// Click and drag are meant to be mutually exclusive at the EventSystem
    /// level, but that isn't trusted blindly here: <see cref="_isDragging"/> is
    /// tracked locally and checked in <see cref="OnPointerClick"/>, so a drag
    /// gesture can never activate the spawner on release regardless of how the
    /// active input module handles click suppression internally.
    ///
    /// The flag is cleared only in <see cref="OnPointerDown"/> (start of a
    /// fresh gesture), never in OnPointerUp/OnEndDrag - the relative order in
    /// which EndDrag, PointerClick and PointerUp fire for the same release
    /// isn't something to depend on, and getting that assumption wrong is
    /// exactly what made the first version of this guard not actually work.
    /// </summary>
    [RequireComponent(typeof(Spawner))]
    public sealed class SpawnerPointerInput : MonoBehaviour,
        IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Spawner _spawner;
        private Camera _camera;
        private SpawnerActivationController _activationController;
        private BoardFeedbackChannel _feedback;
        private PieceDragController<Spawner> _dragController;
        private bool _isDragging;

        public void Construct(
            SpawnerActivationController activationController,
            PieceDropResolver<Spawner> dropResolver,
            BoardMergeController<Spawner> mergeController,
            BoardFeedbackChannel feedback,
            Camera camera)
        {
            // Idempotent for the same reason as ItemPointerInput.Construct: a
            // drag-to-empty-cell relocation re-fires ObjectPlaced for the same,
            // already-wired spawner.
            if (_dragController != null) return;

            _activationController = activationController;
            _feedback = feedback;
            _camera = camera;
            _dragController = new PieceDragController<Spawner>(_spawner, dropResolver, mergeController, feedback);
        }

        private void Awake()
        {
            _spawner = GetComponent<Spawner>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // A drag just happened on this same press-release cycle - this is
            // not a tap, don't activate. Guards against the case reported on
            // device where OnPointerClick still fired after a drag+drop.
            if (_isDragging) return;

            if (!_activationController.TryActivate(_spawner, out _))
                _feedback.NotifyActionRejected("Нет свободного места на поле");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Start of a brand new gesture - only place this gets cleared.
            _isDragging = false;
            _dragController.Press();
        }

        public void OnPointerUp(PointerEventData eventData) => _dragController.Release();

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _dragController.BeginDrag();
        }

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
