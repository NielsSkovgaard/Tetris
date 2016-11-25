using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceTests
    {
        private readonly PieceBlockManager _pieceBlockManager = new PieceBlockManager();

        [TestMethod]
        public void Blocks_PieceTypeI()
        {
            // Arrange
            Piece piece = new Piece(PieceType.I, _pieceBlockManager);
            int[,] expected = {{0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0}};

            // Act
            int[,] actual = piece.Blocks;

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
