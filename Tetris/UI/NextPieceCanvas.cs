using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.UI
{
    internal class NextPieceCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;
        private readonly int _blockSizeInPixels; // Usually 20px
        private readonly int _rows; // Usually 6
        private readonly int _cols; // Usually 6

        // Dependency injection of GameBoard into NextPieceCanvas
        public NextPieceCanvas(GameBoard gameBoard, int blockSizeInPixels, int rows, int cols)
        {
            _gameBoard = gameBoard;
            _blockSizeInPixels = blockSizeInPixels;
            _rows = rows;
            _cols = cols;

            _gameBoard.NextPieceChanged += GameBoard_NextPieceChanged;
        }

        // Update the UI (NextPieceCanvas) every time the model (GameBoard.NextPiece) changes
        // Soon after, the OnRender method is called
        private void GameBoard_NextPieceChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Calculate NextPiece coordinates on the NextPieceCanvas
            // Because the _gameBoard.NextPiece.CoordsY and CoordsX coordinates are only used on the GameCanvas
            double nextPieceCoordsY = (_rows - PieceBlockManager.GetHeightOfBlockArray(_gameBoard.NextPiece.PieceType)) / 2d;
            double nextPieceCoordsX = (_cols - PieceBlockManager.GetWidthOfBlockArray(_gameBoard.NextPiece.PieceType)) / 2d;

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
