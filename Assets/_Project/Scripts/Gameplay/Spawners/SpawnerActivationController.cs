using System;
using GHSS.Core.Board;
using GHSS.Core.Common;
using GHSS.Core.Items;
using GHSS.Core.Spawners;
using GHSS.Gameplay.Board;
using GHSS.Gameplay.Items;

namespace GHSS.Gameplay.Spawners
{
    /// <summary>
    /// "Activating" a spawner: roll an item level from its spawn table, find a
    /// free cell and place a new item there. Never touches existing occupants -
    /// if the roll or the definition lookup or the free-cell search fails, it's
    /// simply a no-op, nothing is created and nothing is destroyed.
    ///
    /// Separately, and independently of the regular roll, it may also place a
    /// bonus "unmergeable" item (<see cref="SpawnerDefinition.UnmergeableItem"/>)
    /// - a plain <see cref="Item"/> whose definition is deliberately absent from
    /// the item chain, so <see cref="GHSS.Core.Common.MergeRules"/> already
    /// refuses to merge it with anything (chain-membership check), with no
    /// changes needed anywhere in the merge pipeline.
    /// </summary>
    public sealed class SpawnerActivationController
    {
        private readonly BoardGrid _board;
        private readonly BoardCoordinateConverter _coordinates;
        private readonly ItemChainConfig _itemChain;
        private readonly ItemFactory _itemFactory;
        private readonly IRandomSource _random;

        public SpawnerActivationController(
            BoardGrid board,
            BoardCoordinateConverter coordinates,
            ItemChainConfig itemChain,
            ItemFactory itemFactory,
            IRandomSource random)
        {
            _board = board != null ? board : throw new ArgumentNullException(nameof(board));
            _coordinates = coordinates != null ? coordinates : throw new ArgumentNullException(nameof(coordinates));
            _itemChain = itemChain != null ? itemChain : throw new ArgumentNullException(nameof(itemChain));
            _itemFactory = itemFactory != null ? itemFactory : throw new ArgumentNullException(nameof(itemFactory));
            _random = random != null ? random : throw new ArgumentNullException(nameof(random));
        }

        public bool TryActivate(Spawner spawner, out Item spawnedItem)
        {
            spawnedItem = null;
            if (spawner == null) return false;

            var regularItemSpawned = TrySpawnRegularItem(spawner, out spawnedItem);
            TrySpawnBonusItem(spawner);

            return regularItemSpawned;
        }

        /// <summary>The existing 90/10 / 50/50 roll, untouched - reads only <see cref="SpawnerDefinition.SpawnTable"/>.</summary>
        private bool TrySpawnRegularItem(Spawner spawner, out Item spawnedItem)
        {
            spawnedItem = null;

            if (!WeightedItemRoller.TryRoll(spawner.Definition.SpawnTable, _random, out var itemLevel)) return false;
            if (!_itemChain.TryGetDefinition(itemLevel, out var itemDefinition)) return false;
            if (!_board.TryGetRandomFreeCell(_random, out var freeCell)) return false;

            spawnedItem = _itemFactory.Create(itemDefinition, _coordinates.CellToWorld(freeCell), spawner.transform.parent);
            _board.TryPlace(spawnedItem, freeCell);
            return true;
        }

        /// <summary>Independent bonus roll - never reads SpawnTable, never affects its odds.
        /// A missing free cell (or a missing UnmergeableItem, i.e. the bonus being
        /// disabled for this spawner level) is a silent no-op, same as the regular roll.</summary>
        private void TrySpawnBonusItem(Spawner spawner)
        {
            var bonusItemDefinition = spawner.Definition.UnmergeableItem;
            if (bonusItemDefinition == null) return;
            if (!BonusItemRoller.ShouldSpawn(spawner.Definition.UnmergeableItemChance, _random)) return;
            if (!_board.TryGetRandomFreeCell(_random, out var freeCell)) return;

            var bonusItem = _itemFactory.Create(bonusItemDefinition, _coordinates.CellToWorld(freeCell), spawner.transform.parent);
            _board.TryPlace(bonusItem, freeCell);
        }
    }
}
