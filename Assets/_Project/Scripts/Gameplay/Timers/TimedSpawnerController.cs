using System;
using GHSS.Core.Board;
using GHSS.Core.Common;
using GHSS.Core.Spawners;
using GHSS.Core.Timers;
using GHSS.Gameplay.Board;
using GHSS.Gameplay.Spawners;

namespace GHSS.Gameplay.Timers
{
    /// <summary>
    /// The glue between Timer, GameField and Spawner: owns the countdown, and on
    /// completion asks the board for a free cell and the spawner system to fill
    /// it with the spawner level configured in <see cref="TimedSpawnerConfig"/>.
    /// This is the only class that knows about all three systems - Timer,
    /// GameField (BoardGrid) and Spawner stay unaware of each other and of the UI.
    /// </summary>
    public sealed class TimedSpawnerController : ISpawnTimer
    {
        private readonly BoardGrid _board;
        private readonly BoardCoordinateConverter _coordinates;
        private readonly SpawnerChainConfig _spawnerChain;
        private readonly SpawnerFactory _spawnerFactory;
        private readonly IRandomSource _random;
        private readonly int _spawnerLevel;
        private readonly CountdownTimer _countdown;

        public IReadOnlyCountdown Countdown => _countdown;

        /// <summary>Raised when the countdown finished but the board had no free cell.</summary>
        public event Action SpawnFailed;

        public TimedSpawnerController(
            TimedSpawnerConfig config,
            TimerService timerService,
            BoardGrid board,
            BoardCoordinateConverter coordinates,
            SpawnerChainConfig spawnerChain,
            SpawnerFactory spawnerFactory,
            IRandomSource random)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (config.Timer == null) throw new ArgumentException("TimedSpawnerConfig has no TimerConfig assigned.", nameof(config));
            if (timerService == null) throw new ArgumentNullException(nameof(timerService));

            _board = board != null ? board : throw new ArgumentNullException(nameof(board));
            _coordinates = coordinates != null ? coordinates : throw new ArgumentNullException(nameof(coordinates));
            _spawnerChain = spawnerChain != null ? spawnerChain : throw new ArgumentNullException(nameof(spawnerChain));
            _spawnerFactory = spawnerFactory != null ? spawnerFactory : throw new ArgumentNullException(nameof(spawnerFactory));
            _random = random != null ? random : throw new ArgumentNullException(nameof(random));
            _spawnerLevel = config.SpawnerLevel;

            _countdown = new CountdownTimer(config.Timer.DurationSeconds);
            _countdown.Completed += OnCountdownCompleted;
            timerService.Register(_countdown);
        }

        public void StartCountdown() => _countdown.Start();

        private void OnCountdownCompleted()
        {
            if (!_board.TryGetRandomFreeCell(_random, out var cell) || !_spawnerChain.TryGetDefinition(_spawnerLevel, out var definition))
            {
                SpawnFailed?.Invoke();
                return;
            }

            var spawner = _spawnerFactory.Create(definition, _coordinates.CellToWorld(cell));
            _board.TryPlace(spawner, cell);
        }
    }
}
