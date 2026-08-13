using NUnit.Framework;
using GHSS.Core.Spawners;
using GHSS.Tests.EditMode.TestSupport;

namespace GHSS.Tests.EditMode.Spawn
{
    /// <summary>
    /// WeightedItemRoller is what actually answers "which item level does this
    /// spawner produce". Using a deterministic FakeRandomSource instead of real
    /// randomness lets every boundary of the probability table be asserted
    /// exactly, with no statistical/flaky sampling involved.
    /// roll = randomValue * totalWeight, and an entry wins once roll falls
    /// below its cumulative weight - so for a table [90, 10] the boundary
    /// between outcomes sits at randomValue == 0.9.
    /// </summary>
    public class WeightedItemRollerTests
    {
        // Mirrors Spawner Level 1's default table: 90% -> item level 1, 10% -> item level 2.
        private static readonly SpawnWeight[] SpawnerLevel1Table =
        {
            new SpawnWeight(itemLevel: 1, weight: 90f),
            new SpawnWeight(itemLevel: 2, weight: 10f)
        };

        // Mirrors Spawner Level 2's default table: 50% / 50%.
        private static readonly SpawnWeight[] SpawnerLevel2Table =
        {
            new SpawnWeight(itemLevel: 1, weight: 50f),
            new SpawnWeight(itemLevel: 2, weight: 50f)
        };

        [TestCase(0f, 1)]
        [TestCase(0.5f, 1)]
        [TestCase(0.899f, 1)]
        [TestCase(0.9f, 2)]
        [TestCase(0.999f, 2)]
        public void SpawnerLevel1_90x10_RollsExpectedItemLevel(float randomValue, int expectedItemLevel)
        {
            var rolled = WeightedItemRoller.TryRoll(SpawnerLevel1Table, new FakeRandomSource(randomValue), out var itemLevel);

            Assert.IsTrue(rolled);
            Assert.AreEqual(expectedItemLevel, itemLevel);
        }

        [TestCase(0f, 1)]
        [TestCase(0.499f, 1)]
        [TestCase(0.5f, 2)]
        [TestCase(0.999f, 2)]
        public void SpawnerLevel2_50x50_RollsExpectedItemLevel(float randomValue, int expectedItemLevel)
        {
            var rolled = WeightedItemRoller.TryRoll(SpawnerLevel2Table, new FakeRandomSource(randomValue), out var itemLevel);

            Assert.IsTrue(rolled);
            Assert.AreEqual(expectedItemLevel, itemLevel);
        }

        [Test]
        public void RollAtVeryTopOfRange_FallsBackToLastEntry()
        {
            var rolled = WeightedItemRoller.TryRoll(SpawnerLevel1Table, new FakeRandomSource(1f), out var itemLevel);

            Assert.IsTrue(rolled);
            Assert.AreEqual(2, itemLevel);
        }

        [Test]
        public void EmptyTable_RollFails()
        {
            var rolled = WeightedItemRoller.TryRoll(System.Array.Empty<SpawnWeight>(), new FakeRandomSource(0f), out _);

            Assert.IsFalse(rolled);
        }

        [Test]
        public void AllZeroWeights_RollFails()
        {
            var table = new[] { new SpawnWeight(1, 0f), new SpawnWeight(2, 0f) };

            var rolled = WeightedItemRoller.TryRoll(table, new FakeRandomSource(0.5f), out _);

            Assert.IsFalse(rolled);
        }

        [Test]
        public void NullTable_RollFails()
        {
            Assert.IsFalse(WeightedItemRoller.TryRoll(null, new FakeRandomSource(0f), out _));
        }
    }
}
