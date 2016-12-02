using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.UI
{
    internal class GameCanvas : Canvas
    {
        private readonly GameBoardCore _gameBoardCore;
        private readonly int _blockSizeInPixels; // Usually 25px

        private readonly Pen _blockBorderPen = new Pen { Brush = Brushes.White };

        // Dependency injection of GameBoardCore into GameCanvas
        public GameCanvas(GameBoardCore gameBoardCore, int blockSizeInPixels)
        {
            _gameBoardCore = gameBoardCore;
            _blockSizeInPixels = blockSizeInPixels;

            _gameBoardCore.LockedBlocksOrCurrentPieceChanged += GameBoard_LockedBlocksOrCurrentPieceChanged;
        }

        // Update the UI (GameCanvas) every time the model (GameBoard) changes
        // Soon after, the OnRender method is called
        private void GameBoard_LockedBlocksOrCurrentPieceChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Render the LockedBlocks array
            for (int row = 0; row < _gameBoardCore.Rows; row++)
            {
                for (int col = 0; col < _gameBoardCore.Cols; col++)
                {
                    int blockType = _gameBoardCore.LockedBlocks[row, col];

                    if (blockType != 0)
                    {
                        Rect rect = new Rect(
                            col * _blockSizeInPixels,
                            row * _blockSizeInPixels,
                            _blockSizeInPixels, _blockSizeInPixels);

                        dc.DrawRectangle(GraphicsConstants.BlockBrushes[blockType - 1], _blockBorderPen, rect);
                    }
                }
            }

            // Render CurrentPiece
            foreach (Block block in _gameBoardCore.CurrentPiece.Blocks)
            {
                Rect rect = new Rect(
                    (_gameBoardCore.CurrentPiece.CoordsX + block.CoordsX) * _blockSizeInPixels,
                    (_gameBoardCore.CurrentPiece.CoordsY + block.CoordsY) * _blockSizeInPixels,
                    _blockSizeInPixels, _blockSizeInPixels);

                dc.DrawRectangle(GraphicsConstants.BlockBrushes[(int)_gameBoardCore.CurrentPiece.PieceType - 1], _blockBorderPen, rect);
            }
        }
    }
}
