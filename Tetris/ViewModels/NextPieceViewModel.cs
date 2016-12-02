using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class NextPieceViewModel
    {
        public GameBoard GameBoard { get; }
        public int BlockSizeInPixels { get; } // Usually 20px
        private readonly int _rows; // Usually 6
        private readonly int _cols; // Usually 6

        public NextPieceViewModel(GameBoard gameBoard, int blockSizeInPixels, int rows, int cols)
        {
            GameBoard = gameBoard;
            BlockSizeInPixels = blockSizeInPixels;
            _rows = rows;
            _cols = cols;
        }

        public double NextPieceCoordsY => (_rows - PieceBlockManager.GetHeightOfBlockArray(GameBoard.NextPiece.PieceType)) / 2d;
        public double NextPieceCoordsX => (_cols - PieceBlockManager.GetWidthOfBlockArray(GameBoard.NextPiece.PieceType)) / 2d;
    }
}
