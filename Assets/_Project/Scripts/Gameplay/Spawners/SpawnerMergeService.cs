using System;
using GHSS.Core.Board;
using GHSS.Core.Common;
using GHSS.Core.Spawners;

namespace GHSS.Gameplay.Spawners
{
    /// <summary>
    /// Spawner Level 1 + Spawner Level 1 -&gt; Spawner Level 2 (and so on, if a
    /// designer adds more levels). Same shape as ItemMergeService, reusing the
    /// same generic <see cref="MergeRules"/>.
    /// </summary>
    public sealed class SpawnerMergeService : IMergeService<Spawner>
    {
        private readonly SpawnerChainConfig _chain;
        private readonly SpawnerFactory _factory;

        public SpawnerMergeService(SpawnerChainConfig chain, SpawnerFactory factory)
        {
            _chain = chain != null ? chain : throw new ArgumentNullException(nameof(chain));
            _factory = factory != null ? factory : throw new ArgumentNullException(nameof(factory));
        }

        public bool CanMerge(Spawner a, Spawner b)
        {
            if (a == null || b == null || a == b) return false;

            return MergeRules.CanMerge(a.Definition, b.Definition, _chain);
        }

        public bool TryMerge(Spawner a, Spawner b, out Spawner result)
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
