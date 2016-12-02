﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.Views
{
    internal class LockedBlocksAndCurrentPieceCanvas : Canvas
    {
        private readonly GameBoardCore _gameBoardCore;
        private readonly int _blockSizeInPixels; // Usually 25px

        private readonly Pen _blockBorderPen = new Pen { Brush = Brushes.White };

        public LockedBlocksAndCurrentPieceCanvas(GameBoardCore gameBoardCore, int blockSizeInPixels)
        {
            _gameBoardCore = gameBoardCore;
            _blockSizeInPixels = blockSizeInPixels;

            _gameBoardCore.LockedBlocksOrCurrentPieceChanged += GameBoardCore_LockedBlocksOrCurrentPieceChanged;
        }

        // Update the View (LockedBlocksAndCurrentPieceCanvas) every time the model (GameBoardCore) changes
        // Soon after, the OnRender method is called
        private void GameBoardCore_LockedBlocksOrCurrentPieceChanged(object sender, EventArgs e)
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

                        dc.DrawRectangle(GraphicsTools.BlockBrushes[blockType - 1], _blockBorderPen, rect);
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

                dc.DrawRectangle(GraphicsTools.BlockBrushes[(int)_gameBoardCore.CurrentPiece.PieceType - 1], _blockBorderPen, rect);
            }
        }
    }
}
