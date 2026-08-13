using NUnit.Framework;
using GHSS.Core.Spawners;
using GHSS.Tests.EditMode.TestSupport;

namespace GHSS.Tests.EditMode.Spawn
{
    /// <summary>
    /// Task 3, scenario 13: changing the configured chance in the Inspector
    /// must actually change the outcome, with no code change. Since the real
    /// value lives on SpawnerDefinition and flows straight into
    /// BonusItemRoller.ShouldSpawn, testing the roller with different chance
    /// arguments is exactly equivalent to testing "what happens when a
    /// designer edits the slider" - no debug/gameplay hack needed.
    /// </summary>
    public class BonusItemRollerTests
    {
        [TestCase(0.3f, 0.29f, true)]
        [TestCase(0.3f, 0.30f, false)]
        [TestCase(0.3f, 0.31f, false)]
        public void ShouldSpawn_RespectsConfiguredChance(float chance, float randomValue, bool expected)
        {
            var result = BonusItemRoller.ShouldSpawn(chance, new FakeRandomSource(randomValue));

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void LoweringChanceFrom30To20Percent_CanFlipAnOtherwiseSuccessfulRoll()
        {
            var random = new FakeRandomSource(0.25f);

            Assert.IsTrue(BonusItemRoller.ShouldSpawn(0.3f, random), "0.25 must succeed at a 30% chance.");
            Assert.IsFalse(BonusItemRoller.ShouldSpawn(0.2f, random), "The same 0.25 roll must fail once the chance is lowered to 20%.");
        }

        [Test]
        public void RaisingChanceFrom30To50Percent_CanFlipAnOtherwiseFailedRoll()
        {
            var random = new FakeRandomSource(0.4f);

            Assert.IsFalse(BonusItemRoller.ShouldSpawn(0.3f, random), "0.4 must fail at a 30% chance.");
            Assert.IsTrue(BonusItemRoller.ShouldSpawn(0.5f, random), "The same 0.4 roll must succeed once the chance is raised to 50%.");
        }

        [Test]
        public void ZeroChance_NeverSpawns()
        {
            Assert.IsFalse(BonusItemRoller.ShouldSpawn(0f, new FakeRandomSource(0f)));
        }

        [Test]
        public void FullChance_AlwaysSpawns()
        {
            Assert.IsTrue(BonusItemRoller.ShouldSpawn(1f, new FakeRandomSource(0f)));
            Assert.IsTrue(BonusItemRoller.ShouldSpawn(1f, new FakeRandomSource(0.999f)));
        }

        [Test]
        public void NullRandomSource_NeverSpawns()
        {
            Assert.IsFalse(BonusItemRoller.ShouldSpawn(1f, null));
        }
    }
}
