using GHSS.Core.Common;

namespace GHSS.Core.Spawners
{
    /// <summary>
    /// Independent Bernoulli check for a spawner's bonus/unmergeable item.
    /// Deliberately separate from <see cref="WeightedItemRoller"/> (which picks
    /// among the regular SpawnTable outcomes) - the two never share state or
    /// logic, so tuning the bonus chance can never change the 90/10 or 50/50 odds.
    /// </summary>
    public static class BonusItemRoller
    {
        public static bool ShouldSpawn(float chance, IRandomSource random)
        {
            if (random == null) return false;

            return random.NextFloat01() < chance;
        }
    }
}
