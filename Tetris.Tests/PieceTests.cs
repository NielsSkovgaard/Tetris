using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void CurrentBlocks()
        {
            Piece piece = new Piece(PieceType.I);
            int[,] expected = { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 } };
            int[,] actual = piece.CurrentBlocks;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NextBlocks()
        {
            Piece piece = new Piece(PieceType.I);
            int[,] expected = { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
            int[,] actual = piece.NextBlocks;
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
