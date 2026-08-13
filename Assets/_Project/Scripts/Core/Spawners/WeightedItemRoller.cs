using System.Collections.Generic;
using UnityEngine;
using GHSS.Core.Common;

namespace GHSS.Core.Spawners
{
    /// <summary>
    /// Rolls one item level out of a weighted table. Pure function of
    /// (table, random source) - no percentages are hardcoded anywhere here,
    /// the table is the only source of truth for probabilities.
    /// </summary>
    public static class WeightedItemRoller
    {
        public static bool TryRoll(IReadOnlyList<SpawnWeight> table, IRandomSource random, out int itemLevel)
        {
            itemLevel = 0;
            if (table == null || table.Count == 0 || random == null) return false;

            var total = 0f;
            for (var i = 0; i < table.Count; i++)
                total += Mathf.Max(0f, table[i].Weight);

            if (total <= 0f) return false;

            var roll = random.NextFloat01() * total;
            var cumulative = 0f;

            for (var i = 0; i < table.Count; i++)
            {
                cumulative += Mathf.Max(0f, table[i].Weight);
                if (roll < cumulative)
                {
                    itemLevel = table[i].ItemLevel;
                    return true;
                }
            }

            itemLevel = table[table.Count - 1].ItemLevel;
            return true;
        }
    }
}
