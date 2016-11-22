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
        public void BlocksAfterNextRotation()
        {
            Piece piece = new Piece(PieceType.I);
            int[,] expected = { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
            int[,] actual = piece.BlocksAfterNextRotation;
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
