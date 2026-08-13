using System.Linq;
using NUnit.Framework;
using UnityEngine;
using GHSS.Core.Board;
using GHSS.Tests.EditMode.TestSupport;

namespace GHSS.Tests.EditMode.Grid
{
    /// <summary>
    /// BoardGrid is plain C# (index-based, no world positions) - a FakeBoardObject
    /// is enough to exercise it, no Item/Spawner/prefab required.
    /// </summary>
    public class BoardGridTests
    {
        private BoardGrid _grid;

        [SetUp]
        public void SetUp()
        {
            _grid = new BoardGrid(width: 3, height: 2);
        }

        [Test]
        public void FreeCell_CanBeOccupied()
        {
            var obj = new FakeBoardObject();
            var coord = new Vector2Int(1, 1);

            var placed = _grid.TryPlace(obj, coord);

            Assert.IsTrue(placed);
            Assert.AreEqual(coord, obj.BoardPosition);
            Assert.AreSame(obj, _grid.GetObjectAt(coord));
            Assert.IsFalse(_grid.IsFree(coord));
        }

        [Test]
        public void OccupiedCell_CannotBeOccupiedAgain()
        {
            var first = new FakeBoardObject();
            var second = new FakeBoardObject();
            var coord = new Vector2Int(0, 0);
            _grid.TryPlace(first, coord);

            var placedSecond = _grid.TryPlace(second, coord);

            Assert.IsFalse(placedSecond);
            Assert.IsNull(second.BoardPosition, "A rejected placement must not touch the object's position.");
            Assert.AreSame(first, _grid.GetObjectAt(coord), "The original occupant must not be displaced.");
        }

        [Test]
        public void GetFreeCells_ReturnsExactlyTheUnoccupiedCells()
        {
            _grid.TryPlace(new FakeBoardObject(), new Vector2Int(0, 0));
            _grid.TryPlace(new FakeBoardObject(), new Vector2Int(2, 1));

            var freeCells = _grid.GetFreeCells().ToList();

            Assert.AreEqual(_grid.Width * _grid.Height - 2, freeCells.Count);
            Assert.AreEqual(freeCells.Count, _grid.FreeCellCount);
            CollectionAssert.DoesNotContain(freeCells, new Vector2Int(0, 0));
            CollectionAssert.DoesNotContain(freeCells, new Vector2Int(2, 1));
        }

        [TestCase(0, 0, true)]
        [TestCase(2, 1, true)]
        [TestCase(-1, 0, false)]
        [TestCase(0, -1, false)]
        [TestCase(3, 0, false)]
        [TestCase(0, 2, false)]
        public void IsInside_ValidatesCoordinatesAgainstBoardBounds(int x, int y, bool expectedInside)
        {
            Assert.AreEqual(expectedInside, _grid.IsInside(new Vector2Int(x, y)));
        }

        [Test]
        public void PlacingOutsideBounds_Fails()
        {
            var obj = new FakeBoardObject();

            var placed = _grid.TryPlace(obj, new Vector2Int(99, 99));

            Assert.IsFalse(placed);
            Assert.IsNull(obj.BoardPosition);
        }

        [Test]
        public void Remove_FreesTheCellAndClearsTheObjectsPosition()
        {
            var obj = new FakeBoardObject();
            var coord = new Vector2Int(1, 0);
            _grid.TryPlace(obj, coord);

            var removed = _grid.Remove(obj);

            Assert.IsTrue(removed);
            Assert.IsNull(obj.BoardPosition);
            Assert.IsTrue(_grid.IsFree(coord));
        }
    }
}
