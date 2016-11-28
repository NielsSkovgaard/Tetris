using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Model;

namespace Tetris.View
{
    internal class NextPieceCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;
        private readonly int _blockSizeInPixels; // Usually 25px
        private readonly int _nextPieceRows;
        private readonly int _nextPieceCols;

        // Dependency injection of GameBoard into NextPieceCanvas
        public NextPieceCanvas(GameBoard gameBoard, int blockSizeInPixels, int nextPieceRows, int nextPieceCols)
        {
            _gameBoard = gameBoard;
            _blockSizeInPixels = blockSizeInPixels;
            _nextPieceRows = nextPieceRows;
            _nextPieceCols = nextPieceCols;

            _gameBoard.GameBoardNextPieceChanged += GameBoard_GameBoardNextPieceChanged;
        }

        // Update the UI (NextPieceCanvas) every time the model (GameBoard.NextPiece) changes
        // Soon after, the OnRender method is called
        private void GameBoard_GameBoardNextPieceChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double nextPieceCoordsY = (_nextPieceRows - PieceBlockManager.GetHeightOfBlockArray(_gameBoard.NextPiece.PieceType)) / 2d;
            double nextPieceCoordsX = (_nextPieceCols - PieceBlockManager.GetWidthOfBlockArray(_gameBoard.NextPiece.PieceType)) / 2d;

            // Render the currently moving Piece
            foreach (Block block in _gameBoard.NextPiece.Blocks)
            {
                Rect rect = new Rect(
                    (nextPieceCoordsX + block.CoordsX) * _blockSizeInPixels,
                    (nextPieceCoordsY + block.CoordsY) * _blockSizeInPixels,
                    _blockSizeInPixels, _blockSizeInPixels);

                dc.DrawRectangle(GraphicsConstants.BlockBrushes[(int)_gameBoard.NextPiece.PieceType - 1], GraphicsConstants.BlockBorderPen, rect);
            }
        }
    }
}
