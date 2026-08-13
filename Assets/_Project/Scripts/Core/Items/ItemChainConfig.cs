using UnityEngine;
using GHSS.Core.Common;

namespace GHSS.Core.Items
{
    /// <summary>
    /// The single item chain (Level 1..N). Number of levels comes from how many
    /// definitions the designer adds here - nothing in code assumes exactly 4.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemChainConfig", menuName = "GHSS/Items/Item Chain Config")]
    public sealed class ItemChainConfig : LevelChainConfig<ItemDefinition>
    {
    }
}
