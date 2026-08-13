using System;
using GHSS.Core.Board;
using GHSS.Core.Common;
using GHSS.Core.Items;

namespace GHSS.Gameplay.Items
{
    /// <summary>
    /// Executes a merge: validates it via <see cref="MergeRules"/>, spawns the
    /// next-level item through <see cref="ItemFactory"/> and consumes the two inputs.
    /// Knows nothing about the board - placing the result into a cell is the caller's job.
    /// </summary>
    public sealed class ItemMergeService : IMergeService<Item>
    {
        private readonly ItemChainConfig _chain;
        private readonly ItemFactory _factory;

        public ItemMergeService(ItemChainConfig chain, ItemFactory factory)
        {
            _chain = chain != null ? chain : throw new ArgumentNullException(nameof(chain));
            _factory = factory != null ? factory : throw new ArgumentNullException(nameof(factory));
        }

        public bool CanMerge(Item a, Item b)
        {
            if (a == null || b == null || a == b) return false;

            return MergeRules.CanMerge(a.Definition, b.Definition, _chain);
        }

        public bool TryMerge(Item a, Item b, out Item result)
        {
            if (!CanMerge(a, b) || !_chain.TryGetNextDefinition(a.Level, out var nextDefinition))
            {
                result = null;
                return false;
            }

            result = _factory.Create(nextDefinition, a.transform.position, a.transform.parent);

            UnityEngine.Object.Destroy(a.gameObject);
            UnityEngine.Object.Destroy(b.gameObject);

            return true;
        }
    }
}
