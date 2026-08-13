namespace GHSS.Core.Common
{
    /// <summary>
    /// Anything that advances with time (countdown, stopwatch, repeating trigger, ...).
    /// Lets a single central driver tick many objects instead of each one running
    /// its own Update().
    /// </summary>
    public interface ITickable
    {
        void Tick(float deltaTime);
    }
}
