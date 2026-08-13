namespace GHSS.Core.Common
{
    /// <summary>
    /// Abstraction over randomness so weighted-roll logic (e.g. spawn tables) can
    /// be unit-tested with a deterministic fake instead of UnityEngine.Random.
    /// </summary>
    public interface IRandomSource
    {
        float NextFloat01();
    }
}
