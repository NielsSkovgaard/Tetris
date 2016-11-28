using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tetris.Models;

namespace Tetris.Tests.Models
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void Blocks_PieceTypeI()
        {
            // Arrange
            Piece piece = new Piece(PieceType.I);
            Block[] expected = { new Block(1, 0), new Block(1, 1), new Block(1, 2), new Block(1, 3) };

            // Act
            Block[] actual = piece.Blocks;

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
