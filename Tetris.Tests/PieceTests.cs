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
            Block[] expected = { new Block(1, 0, 1), new Block(1, 1, 1), new Block(1, 2, 1), new Block(1, 3, 1) };

            // Act
            Block[] actual = piece.Blocks;

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
