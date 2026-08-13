using UnityEngine;
using GHSS.Core.Board;
using GHSS.Core.Items;
using GHSS.Core.Spawners;
using GHSS.Core.Timers;

namespace GHSS.Core.Game
{
    /// <summary>
    /// Single entry point for wiring: references every balance config asset in
    /// one place. Holds no gameplay data of its own - only references, so nothing
    /// here can drift out of sync with the configs it points to. Gameplay
    /// services still depend on the individual config they need (BoardGrid on
    /// BoardConfig, ItemFactory on ItemChainConfig, ...), never on GameConfig -
    /// this asset exists for the composition root and for the designer to have
    /// one place that shows the whole game, not as a service dependency.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GHSS/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [SerializeField] private BoardConfig board;
        [SerializeField] private ItemChainConfig items;
        [SerializeField] private SpawnerChainConfig spawners;
        [SerializeField] private TimedSpawnerConfig timedSpawner;

        public BoardConfig Board => board;
        public ItemChainConfig Items => items;
        public SpawnerChainConfig Spawners => spawners;
        public TimedSpawnerConfig TimedSpawner => timedSpawner;
    }
}
