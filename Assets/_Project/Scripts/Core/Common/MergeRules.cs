namespace GHSS.Core.Common
{
    /// <summary>
    /// Merge eligibility, generic over any leveled definition type. Backs both
    /// item merging and spawner merging with one rule: equal level, same chain,
    /// and the chain must actually have a next level (rules out the max level).
    ///
    /// "Same chain" is membership in the <see cref="LevelChainConfig{TDefinition}"/>
    /// array, nothing else - so a definition that is deliberately never added to
    /// any chain (e.g. a bonus item that should never merge) is permanently
    /// ineligible here, for both operands, with no per-type or per-level check
    /// anywhere in the merge pipeline.
    /// </summary>
    public static class MergeRules
    {
        public static bool CanMerge<TDefinition>(TDefinition a, TDefinition b, LevelChainConfig<TDefinition> chain)
            where TDefinition : UnityEngine.Object, ILeveled
        {
            if (chain == null || a == null || b == null) return false;
            if (a.Level != b.Level) return false;
            if (!chain.Contains(a) || !chain.Contains(b)) return false;

            return chain.TryGetNextDefinition(a.Level, out _);
        }
    }
}
