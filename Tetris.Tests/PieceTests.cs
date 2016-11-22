using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    internal class PieceTests
    {
        [TestMethod]
        public void CurrentBlocks()
        {
            Piece piece = new Piece(PieceType.I);
            PieceBlockManager pieceBlockManager = new PieceBlockManager();
            int[,] expected = {{0, 1, 0, 0}, {0, 1, 0, 0}, {0, 1, 0, 0}, {0, 1, 0, 0}};
            int[,] actual = piece.GetCurrentBlocks(pieceBlockManager);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BlocksAfterNextRotation()
        {
            Piece piece = new Piece(PieceType.I);
            PieceBlockManager pieceBlockManager = new PieceBlockManager();
            int[,] expected = {{0, 0, 0, 0}, {1, 1, 1, 1}, {0, 0, 0, 0}, {0, 0, 0, 0}};
            int[,] actual = piece.GetBlocksAfterNextRotation(pieceBlockManager);
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
