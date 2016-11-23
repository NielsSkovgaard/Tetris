using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceTests
    {
        private readonly PieceBlockManager _pieceBlockManager = new PieceBlockManager();

        [TestMethod]
        public void CurrentBlocks()
        {
            //Arrange
            Piece piece = new Piece(PieceType.I);
            int[,] expected = {{0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0}};

            //Act
            piece.UpdateCurrentBlocks(_pieceBlockManager);
            int[,] actual = piece.CurrentBlocks;

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LeftmostBlockIndex()
        {
            Piece piece = new Piece(PieceType.O);
            piece.UpdateCurrentBlocks(_pieceBlockManager);
            Assert.AreEqual(1, piece.LeftmostBlockIndex);
        }

        [TestMethod]
        public void RightmostBlockIndex()
        {
            Piece piece = new Piece(PieceType.S);
            piece.UpdateCurrentBlocks(_pieceBlockManager);
            Assert.AreEqual(2, piece.RightmostBlockIndex);
        }
    }
}
