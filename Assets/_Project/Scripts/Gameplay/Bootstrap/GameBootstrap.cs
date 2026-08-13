using UnityEngine;
using GHSS.Core.Board;
using GHSS.Core.Common;
using GHSS.Core.Game;
using GHSS.Core.Items;
using GHSS.Core.Spawners;
using GHSS.Gameplay.Board;
using GHSS.Gameplay.Items;
using GHSS.Gameplay.PointerInput;
using GHSS.Gameplay.Spawners;
using GHSS.Gameplay.Timers;
using GHSS.Gameplay.UI;

namespace GHSS.Gameplay.Bootstrap
{
    /// <summary>
    /// Composition root: the only class in the project that references every
    /// system at once. It makes no gameplay decisions of its own - it only
    /// constructs services and wires their dependencies, plus places the
    /// starting Level 1 spawner. All actual behavior lives in the services it
    /// builds; this class contains no "if" about game rules, only "new" and
    /// "Construct" calls, which is why it isn't a God Object despite touching
    /// everything.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private Camera boardCamera;
        [SerializeField] private Vector2Int initialSpawnerCell;

        [SerializeField] private SpawnTimerView spawnTimerView;
        [SerializeField] private BoardStateView boardStateView;
        [SerializeField] private ActionFeedbackView actionFeedbackView;

        // Held explicitly (not just left as locals) so their lifetime doesn't
        // depend on being reachable through some other object's fields -
        // ownership of "the board" and "the thing wiring input" belongs here.
        private BoardGrid _board;
        private BoardInputBinder _inputBinder;

        private void Start()
        {
            // Nothing outside this class ever needs a scene reference to it,
            // so it doesn't need to be a manually-wired object either - just
            // one more component on the same GameObject.
            var timerService = gameObject.AddComponent<TimerService>();

            _board = new BoardGrid(gameConfig.Board);
            var coordinates = new BoardCoordinateConverter(gameConfig.Board);
            var feedback = new BoardFeedbackChannel();
            IRandomSource random = new UnityRandomSource();

            var itemFactory = new ItemFactory();
            var itemMergeService = new ItemMergeService(gameConfig.Items, itemFactory);
            var itemMergeController = new BoardMergeController<Item>(_board, itemMergeService);
            var itemDropResolver = new PieceDropResolver<Item>(_board, coordinates, itemMergeController);

            var spawnerFactory = new SpawnerFactory();
            var spawnerMergeService = new SpawnerMergeService(gameConfig.Spawners, spawnerFactory);
            var spawnerMergeController = new BoardMergeController<Spawner>(_board, spawnerMergeService);
            var spawnerDropResolver = new PieceDropResolver<Spawner>(_board, coordinates, spawnerMergeController);
            var activationController = new SpawnerActivationController(_board, coordinates, gameConfig.Items, itemFactory, random);

            var timedSpawner = new TimedSpawnerController(
                gameConfig.TimedSpawner, timerService, _board, coordinates, gameConfig.Spawners, spawnerFactory, random);
            timedSpawner.SpawnFailed += () => feedback.NotifyActionRejected("Нет места для нового Spawner");

            // Must exist before the first TryPlace (including the initial spawner
            // below) so it never misses wiring input on a piece.
            _inputBinder = new BoardInputBinder(
                _board, itemDropResolver, itemMergeController, spawnerDropResolver, spawnerMergeController,
                activationController, feedback, boardCamera);

            spawnTimerView.Construct(timedSpawner);
            boardStateView.Construct(_board);
            actionFeedbackView.Construct(feedback, timerService);

            PlaceInitialSpawner(coordinates, spawnerFactory);
        }

        private void PlaceInitialSpawner(BoardCoordinateConverter coordinates, SpawnerFactory spawnerFactory)
        {
            if (!gameConfig.Spawners.TryGetDefinition(1, out var level1)) return;

            var worldPosition = coordinates.CellToWorld(initialSpawnerCell);
            var spawner = spawnerFactory.Create(level1, worldPosition);
            _board.TryPlace(spawner, initialSpawnerCell);
        }
    }
}
