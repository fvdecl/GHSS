using GHSS.Core.Common;

namespace GHSS.Tests.EditMode.TestSupport
{
    /// <summary>
    /// Deterministic IRandomSource stand-in - returns whatever value the test
    /// sets, instead of a real (flaky-to-assert-on) random stream. This is the
    /// exact reason IRandomSource exists as an abstraction over UnityEngine.Random.
    /// </summary>
    internal sealed class FakeRandomSource : IRandomSource
    {
        private readonly float _value;

        public FakeRandomSource(float value)
        {
            _value = value;
        }

        public float NextFloat01() => _value;
    }
}
