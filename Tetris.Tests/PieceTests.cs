using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void Blocks_PieceTypeI()
        {
            // Arrange
            Piece piece = new Piece(PieceType.I);
            int[,] expected = {{0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0}};

            // Act
            int[,] actual = piece.Blocks;

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
