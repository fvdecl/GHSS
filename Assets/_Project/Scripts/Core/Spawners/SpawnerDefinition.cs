using System;
using System.Collections.Generic;
using UnityEngine;
using GHSS.Core.Common;
using GHSS.Core.Items;

namespace GHSS.Core.Spawners
{
    /// <summary>
    /// Per-level spawner data: level, visual, prefab and the spawn probability
    /// table (item level -> weight). The spawner never sees an item prefab -
    /// only item levels; resolving a level to a prefab is <see cref="GHSS.Core.Items.ItemChainConfig"/>'s job.
    ///
    /// <see cref="UnmergeableItem"/>/<see cref="UnmergeableItemChance"/> are a
    /// second, independent bonus roll - unrelated to <see cref="SpawnTable"/>,
    /// so tuning the bonus chance can never change the normal 90/10 or 50/50
    /// odds. Leaving <see cref="UnmergeableItem"/> unassigned disables the
    /// bonus entirely for that spawner level.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnerDefinition", menuName = "GHSS/Spawners/Spawner Definition")]
    public sealed class SpawnerDefinition : ScriptableObject, ILeveled
    {
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField] private Sprite icon;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Spawner prefab;
        [SerializeField] private SpawnWeight[] spawnTable = Array.Empty<SpawnWeight>();

        [Header("Bonus unmergeable item (independent of Spawn Table)")]
        [SerializeField] private ItemDefinition unmergeableItem;
        [SerializeField, Range(0f, 1f)] private float unmergeableItemChance;

        public int Level => level;
        public Sprite Icon => icon;
        public Color Color => color;
        public Spawner Prefab => prefab;
        public IReadOnlyList<SpawnWeight> SpawnTable => spawnTable;
        public ItemDefinition UnmergeableItem => unmergeableItem;
        public float UnmergeableItemChance => unmergeableItemChance;
    }
}
