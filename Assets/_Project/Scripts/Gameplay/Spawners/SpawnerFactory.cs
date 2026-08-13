using System;
using UnityEngine;
using GHSS.Core.Spawners;

namespace GHSS.Gameplay.Spawners
{
    /// <summary>
    /// The only place that instantiates a spawner prefab.
    /// </summary>
    public sealed class SpawnerFactory
    {
        public Spawner Create(SpawnerDefinition definition, Vector3 position, Transform parent = null)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (definition.Prefab == null)
                throw new InvalidOperationException($"Spawner definition for level {definition.Level} has no prefab assigned.");

            var spawner = UnityEngine.Object.Instantiate(definition.Prefab, position, Quaternion.identity, parent);
            spawner.Initialize(definition);
            return spawner;
        }
    }
}
