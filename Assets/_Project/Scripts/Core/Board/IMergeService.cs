namespace GHSS.Core.Board
{
    /// <summary>
    /// Executes a merge for one board-piece family (items, spawners, ...).
    /// Implemented per family so <c>BoardMergeController&lt;TPiece&gt;</c> can stay
    /// generic and family-agnostic.
    /// </summary>
    public interface IMergeService<TPiece> where TPiece : class, IBoardObject
    {
        bool CanMerge(TPiece a, TPiece b);
        bool TryMerge(TPiece a, TPiece b, out TPiece result);
    }
}
