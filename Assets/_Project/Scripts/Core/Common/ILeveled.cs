namespace GHSS.Core.Common
{
    /// <summary>
    /// Anything that has a level within some chain (item definitions, spawner
    /// definitions, ...). Lets level-chain/merge logic stay generic.
    /// </summary>
    public interface ILeveled
    {
        int Level { get; }
    }
}
