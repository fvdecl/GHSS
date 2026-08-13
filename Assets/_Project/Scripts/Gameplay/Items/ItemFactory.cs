using System;
using UnityEngine;
using GHSS.Core.Items;

namespace GHSS.Gameplay.Items
{
    /// <summary>
    /// The only place that actually instantiates an item prefab. Everything else
    /// works with <see cref="Item"/>/<see cref="ItemDefinition"/> and never calls
    /// Instantiate directly.
    /// </summary>
    public sealed class ItemFactory
    {
        public Item Create(ItemDefinition definition, Vector3 position, Transform parent = null)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (definition.Prefab == null)
                throw new InvalidOperationException($"Item definition for level {definition.Level} has no prefab assigned.");

            var item = UnityEngine.Object.Instantiate(definition.Prefab, position, Quaternion.identity, parent);
            item.Initialize(definition);
            return item;
        }
    }
}
