using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceBlockManagerTests
    {
        private readonly PieceBlockManager _pieceBlockManager = new PieceBlockManager();

        [TestMethod]
        public void LeftmostBlockIndex_PieceTypeO()
        {
            int[,] blockArray = _pieceBlockManager.GetBlocks(PieceType.O, 0);
            int actual = PieceBlockManager.GetLeftmostBlockIndex(blockArray);
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void GetRightmostBlockIndex_PieceTypeS()
        {
            int[,] blockArray = _pieceBlockManager.GetBlocks(PieceType.S, 0);
            int actual = PieceBlockManager.GetRightmostBlockIndex(blockArray);
            Assert.AreEqual(2, actual);
        }
    }
}
