using System;
using UnityEngine;
using GHSS.Core.Board;
using GHSS.Core.Items;
using GHSS.Core.Spawners;
using GHSS.Gameplay.Board;
using GHSS.Gameplay.Spawners;

namespace GHSS.Gameplay.PointerInput
{
    /// <summary>
    /// Wires the input component of every item/spawner the moment it's placed
    /// on the board - regardless of how it got there (initial placement, merge
    /// result, spawner activation, timer spawn). Reacts to BoardGrid's own
    /// ObjectPlaced event instead of every creation path (ItemFactory,
    /// SpawnerFactory, the merge services) remembering to wire input itself,
    /// which would otherwise force a dependency cycle (factories are used by
    /// the merge/drag system, so the drag system can't also be a dependency
    /// of the factories).
    /// </summary>
    public sealed class BoardInputBinder
    {
        private readonly PieceDropResolver<Item> _itemDropResolver;
        private readonly BoardMergeController<Item> _itemMergeController;
        private readonly PieceDropResolver<Spawner> _spawnerDropResolver;
        private readonly BoardMergeController<Spawner> _spawnerMergeController;
        private readonly SpawnerActivationController _spawnerActivation;
        private readonly BoardFeedbackChannel _feedback;
        private readonly Camera _camera;

        public BoardInputBinder(
            IReadOnlyBoard board,
            PieceDropResolver<Item> itemDropResolver,
            BoardMergeController<Item> itemMergeController,
            PieceDropResolver<Spawner> spawnerDropResolver,
            BoardMergeController<Spawner> spawnerMergeController,
            SpawnerActivationController spawnerActivation,
            BoardFeedbackChannel feedback,
            Camera camera)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));

            _itemDropResolver = itemDropResolver ?? throw new ArgumentNullException(nameof(itemDropResolver));
            _itemMergeController = itemMergeController ?? throw new ArgumentNullException(nameof(itemMergeController));
            _spawnerDropResolver = spawnerDropResolver ?? throw new ArgumentNullException(nameof(spawnerDropResolver));
            _spawnerMergeController = spawnerMergeController ?? throw new ArgumentNullException(nameof(spawnerMergeController));
            _spawnerActivation = spawnerActivation ?? throw new ArgumentNullException(nameof(spawnerActivation));
            _feedback = feedback ?? throw new ArgumentNullException(nameof(feedback));
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));

            board.ObjectPlaced += OnObjectPlaced;
        }

        private void OnObjectPlaced(IBoardObject boardObject, Vector2Int coord)
        {
            switch (boardObject)
            {
                case Item item:
                    item.GetComponent<ItemPointerInput>()
                        ?.Construct(_itemDropResolver, _itemMergeController, _feedback, _camera);
                    break;

                case Spawner spawner:
                    spawner.GetComponent<SpawnerPointerInput>()
                        ?.Construct(_spawnerActivation, _spawnerDropResolver, _spawnerMergeController, _feedback, _camera);
                    break;
            }
        }
    }
}
