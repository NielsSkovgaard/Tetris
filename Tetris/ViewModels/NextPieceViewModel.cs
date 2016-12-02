using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class NextPieceViewModel
    {
        public GameBoardCore GameBoardCore { get; }
        public int BlockSizeInPixels { get; } // Usually 20px
        private readonly int _rows; // Usually 6
        private readonly int _cols; // Usually 6

        public NextPieceViewModel(GameBoardCore gameBoardCore, int blockSizeInPixels, int rows, int cols)
        {
            GameBoardCore = gameBoardCore;
            BlockSizeInPixels = blockSizeInPixels;
            _rows = rows;
            _cols = cols;
        }

        public double NextPieceCoordsY => (_rows - PieceBlockManager.GetHeightOfBlockArray(GameBoardCore.NextPiece.PieceType)) / 2d;
        public double NextPieceCoordsX => (_cols - PieceBlockManager.GetWidthOfBlockArray(GameBoardCore.NextPiece.PieceType)) / 2d;
    }
}
