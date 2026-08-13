using NUnit.Framework;
using GHSS.Core.Common;
using GHSS.Core.Items;
using GHSS.Tests.EditMode.TestSupport;

namespace GHSS.Tests.EditMode.Merge
{
    /// <summary>
    /// MergeRules is a static, pure function of (definition, definition, chain) -
    /// no scene, no prefab, no MonoBehaviour needed to exercise every rule from
    /// the task: equal-level merges up the chain, Level 4 can't merge, mismatched
    /// levels can't merge.
    /// </summary>
    public class MergeRulesTests
    {
        private ItemDefinition _level1;
        private ItemDefinition _level2;
        private ItemDefinition _level3;
        private ItemDefinition _level4;
        private ItemChainConfig _chain;

        [SetUp]
        public void SetUp()
        {
            _level1 = TestConfigFactory.ItemDefinition(1);
            _level2 = TestConfigFactory.ItemDefinition(2);
            _level3 = TestConfigFactory.ItemDefinition(3);
            _level4 = TestConfigFactory.ItemDefinition(4);
            _chain = TestConfigFactory.ItemChain(_level1, _level2, _level3, _level4);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_chain);
            UnityEngine.Object.DestroyImmediate(_level1);
            UnityEngine.Object.DestroyImmediate(_level2);
            UnityEngine.Object.DestroyImmediate(_level3);
            UnityEngine.Object.DestroyImmediate(_level4);
        }

        [Test]
        public void Level1PlusLevel1_CanMerge()
        {
            Assert.IsTrue(MergeRules.CanMerge(_level1, _level1, _chain));
        }

        [Test]
        public void Level2PlusLevel2_CanMerge()
        {
            Assert.IsTrue(MergeRules.CanMerge(_level2, _level2, _chain));
        }

        [Test]
        public void Level3PlusLevel3_CanMerge()
        {
            Assert.IsTrue(MergeRules.CanMerge(_level3, _level3, _chain));
        }

        [Test]
        public void Level4PlusLevel4_CannotMerge_NoNextLevelInChain()
        {
            Assert.IsFalse(MergeRules.CanMerge(_level4, _level4, _chain));
        }

        [Test]
        public void Level1PlusLevel2_CannotMerge_DifferentLevels()
        {
            Assert.IsFalse(MergeRules.CanMerge(_level1, _level2, _chain));
        }

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        public void MergeResult_IsTheNextLevelDefinitionInTheChain(int sourceLevel, int expectedResultLevel)
        {
            _chain.TryGetDefinition(sourceLevel, out var source);

            var hasNext = _chain.TryGetNextDefinition(source.Level, out var next);

            Assert.IsTrue(hasNext);
            Assert.AreEqual(expectedResultLevel, next.Level);
        }

        [Test]
        public void DefinitionNotInChain_CannotMerge_EvenWithMatchingLevelNumber()
        {
            var foreignLevel1 = TestConfigFactory.ItemDefinition(1);

            var canMerge = MergeRules.CanMerge(_level1, foreignLevel1, _chain);

            Assert.IsFalse(canMerge, "A definition from a different chain must not merge just because its Level matches.");
            UnityEngine.Object.DestroyImmediate(foreignLevel1);
        }

        [Test]
        public void NullChain_CannotMerge()
        {
            Assert.IsFalse(MergeRules.CanMerge(_level1, _level1, null));
        }

        [Test]
        public void TwoItemsOutsideTheChain_CannotMergeWithEachOther()
        {
            // Mirrors the real "unmergeable bonus item" setup: two board pieces
            // referencing the very same definition, which was deliberately never
            // added to any chain. No IsMergeable flag, no type check anywhere -
            // absence from the chain is the only thing that makes it non-mergeable,
            // and that must hold even for two instances of the same such item.
            var special = TestConfigFactory.ItemDefinition(1);

            var canMerge = MergeRules.CanMerge(special, special, _chain);

            Assert.IsFalse(canMerge, "Two pieces referencing a definition outside the chain must not merge with each other either.");
            UnityEngine.Object.DestroyImmediate(special);
        }

        [Test]
        public void SpecialItem_CannotMerge_WithRegularItemOfDifferentLevel()
        {
            var special = TestConfigFactory.ItemDefinition(1);

            var canMerge = MergeRules.CanMerge(special, _level2, _chain);

            Assert.IsFalse(canMerge);
            UnityEngine.Object.DestroyImmediate(special);
        }

        [Test]
        public void SpecialItem_IsNeverPartOfTheChain_AndNeverProducedAsAMergeResult()
        {
            var special = TestConfigFactory.ItemDefinition(1);

            Assert.IsFalse(_chain.Contains(special), "An item deliberately kept out of the chain must never register as a chain member.");

            for (var level = 1; level <= 4; level++)
            {
                _chain.TryGetNextDefinition(level, out var next);
                Assert.AreNotSame(special, next, $"The special item must never be the merge result for level {level}.");
            }

            UnityEngine.Object.DestroyImmediate(special);
        }
    }
}
