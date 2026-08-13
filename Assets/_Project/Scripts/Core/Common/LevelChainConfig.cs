using System;
using UnityEngine;

namespace GHSS.Core.Common
{
    /// <summary>
    /// Generic ordered chain of level definitions, shared by any mergeable
    /// board-piece family (items, spawners, ...). The number of levels is however
    /// many entries the designer puts in <see cref="levels"/> - adding a level
    /// never requires a code change.
    /// </summary>
    public abstract class LevelChainConfig<TDefinition> : ScriptableObject
        where TDefinition : UnityEngine.Object, ILeveled
    {
        [SerializeField] private TDefinition[] levels = Array.Empty<TDefinition>();

        public int LevelCount => levels.Length;

        public bool TryGetDefinition(int level, out TDefinition definition)
        {
            foreach (var candidate in levels)
            {
                if (candidate != null && candidate.Level == level)
                {
                    definition = candidate;
                    return true;
                }
            }

            definition = null;
            return false;
        }

        public bool TryGetNextDefinition(int level, out TDefinition next) =>
            TryGetDefinition(level + 1, out next);

        public bool IsMaxLevel(int level) => !TryGetDefinition(level + 1, out _);

        public bool Contains(TDefinition definition)
        {
            if (definition == null) return false;

            return TryGetDefinition(definition.Level, out var found) && found == definition;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Array.Sort(levels, (a, b) =>
            {
                var levelA = a != null ? a.Level : int.MaxValue;
                var levelB = b != null ? b.Level : int.MaxValue;
                return levelA.CompareTo(levelB);
            });
        }
#endif
    }
}
