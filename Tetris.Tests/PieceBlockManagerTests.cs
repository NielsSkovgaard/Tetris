using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceBlockManagerTests
    {
        private readonly PieceBlockManager _pieceBlockManager = new PieceBlockManager();

        [TestMethod]
        public void GetLeftmostBlockIndex_PieceTypeO()
        {
            int[,] blocks = _pieceBlockManager.GetBlocks(PieceType.O, 0);
            int actual = PieceBlockManager.GetLeftmostBlockIndex(blocks);
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void GetRightmostBlockIndex_PieceTypeS()
        {
            int[,] blocks = _pieceBlockManager.GetBlocks(PieceType.S, 0);
            int actual = PieceBlockManager.GetRightmostBlockIndex(blocks);
            Assert.AreEqual(2, actual);
        }
    }
}
