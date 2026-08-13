namespace GHSS.Core.Common
{
    public sealed class UnityRandomSource : IRandomSource
    {
        public float NextFloat01() => UnityEngine.Random.value;
    }
}
