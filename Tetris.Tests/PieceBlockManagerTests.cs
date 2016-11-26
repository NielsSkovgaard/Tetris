using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tetris.Tests
{
    [TestClass]
    public class PieceBlockManagerTests
    {
        [TestMethod]
        public void GetLeftmostBlockIndex_PieceTypeO()
        {
            int[,] blocks = PieceBlockManager.GetBlocks(PieceType.O, 0);
            int actual = PieceBlockManager.GetLeftmostBlockIndex(blocks);
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void GetRightmostBlockIndex_PieceTypeS()
        {
            int[,] blocks = PieceBlockManager.GetBlocks(PieceType.S, 0);
            int actual = PieceBlockManager.GetRightmostBlockIndex(blocks);
            Assert.AreEqual(2, actual);
        }
    }
}
