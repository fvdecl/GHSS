using UnityEngine;
using GHSS.Core.Common;

namespace GHSS.Core.Spawners
{
    /// <summary>
    /// The spawner chain (Level 1..N). Adding Level 3 later is: create a new
    /// <see cref="SpawnerDefinition"/> asset with level = 3 and its own spawn
    /// table, drop it into this array. No code change.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnerChainConfig", menuName = "GHSS/Spawners/Spawner Chain Config")]
    public sealed class SpawnerChainConfig : LevelChainConfig<SpawnerDefinition>
    {
    }
}
