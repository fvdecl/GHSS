using UnityEngine;
using GHSS.Core.Common;

namespace GHSS.Core.Items
{
    /// <summary>
    /// Per-level data: which level this is, how it looks, and which prefab represents it.
    /// One asset per level, all referenced from an <see cref="ItemChainConfig"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "GHSS/Items/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject, ILeveled
    {
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField] private Sprite icon;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Item prefab;

        public int Level => level;
        public Sprite Icon => icon;
        public Color Color => color;
        public Item Prefab => prefab;
    }
}
