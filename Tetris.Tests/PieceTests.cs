using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void Color()
        {
            Piece piece = new Piece(PieceType.I);
            Color actual = piece.Color;
            Assert.AreEqual(Colors.DarkCyan, actual);
        }

        [TestMethod]
        public void CurrentBlocks()
        {
            Piece piece = new Piece(PieceType.I);
            bool[,] expected = { { false, true, false, false }, { false, true, false, false }, { false, true, false, false }, { false, true, false, false } };
            bool[,] actual = piece.CurrentBlocks;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NextBlocks()
        {
            Piece piece = new Piece(PieceType.I);
            bool[,] expected = { { false, false, false, false }, { true, true, true, true }, { false, false, false, false }, { false, false, false, false } };
            bool[,] actual = piece.NextBlocks;
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
