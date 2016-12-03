using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.Views
{
    internal class GameBoardCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;
        private readonly int _blockSizeInPixels; // Usually 25px

        private readonly Pen _blockBorderPen = new Pen { Brush = Brushes.White };

        public GameBoardCanvas(GameBoard gameBoard, int blockSizeInPixels)
        {
            _gameBoard = gameBoard;
            _blockSizeInPixels = blockSizeInPixels;

            _gameBoard.Changed += GameBoard_Changed;
        }

        // Update the View (GameBoardCanvas) every time the model (GameBoard) changes
        // Soon after, the OnRender method is called
        private void GameBoard_Changed(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Render the LockedBlocks array
            for (int row = 0; row < _gameBoard.Rows; row++)
            {
                for (int col = 0; col < _gameBoard.Cols; col++)
                {
                    int blockType = _gameBoard.LockedBlocks[row, col];

                    if (blockType != 0)
                    {
                        Rect rect = new Rect(
                            col * _blockSizeInPixels,
                            row * _blockSizeInPixels,
                            _blockSizeInPixels, _blockSizeInPixels);

                        dc.DrawRectangle(GraphicsTools.BlockBrushes[blockType - 1], _blockBorderPen, rect);
                    }
                }
            }

            // Render CurrentPiece (only blocks that are within the GameBoard)
            foreach (Block block in _gameBoard.CurrentPiece.Blocks.Where(block => _gameBoard.CurrentPiece.CoordsY + block.CoordsY >= 0))
            {
                Rect rect = new Rect(
                    (_gameBoard.CurrentPiece.CoordsX + block.CoordsX) * _blockSizeInPixels,
                    (_gameBoard.CurrentPiece.CoordsY + block.CoordsY) * _blockSizeInPixels,
                    _blockSizeInPixels, _blockSizeInPixels);

                dc.DrawRectangle(GraphicsTools.BlockBrushes[(int)_gameBoard.CurrentPiece.PieceType - 1], _blockBorderPen, rect);
            }
        }
    }
}
