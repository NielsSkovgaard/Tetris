using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class GameCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;
        private readonly int _blockSizeInPixels; // Usually 25px

        // Dependency injection of GameBoard into GameCanvas
        public GameCanvas(GameBoard gameBoard, int blockSizeInPixels)
        {
            _gameBoard = gameBoard;
            _blockSizeInPixels = blockSizeInPixels;

            _gameBoard.GameBoardChanged += GameBoard_GameBoardChanged;
        }

        // Update the UI (GameCanvas) every time the model (GameBoard) changes
        // Soon after, the OnRender method is called
        private void GameBoard_GameBoardChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Render the static blocks
            for (int row = 0; row < _gameBoard.Rows; row++)
            {
                for (int col = 0; col < _gameBoard.Cols; col++)
                {
                    // TODO: Consider having a predefined grid with 10x20 rectangles to color
                    // TODO: Here, are many rectangles created and destroyed all the time?
                    // TODO: The GameBoardChanged EventArgs could then include how the Shape looked before (reset these rectangles) and after for repainting
                    int blockType = _gameBoard.StaticBlocks[row, col];

                    if (blockType != 0)
                    {
                        Rect rect = new Rect(
                            col * _blockSizeInPixels,
                            row * _blockSizeInPixels,
                            _blockSizeInPixels, _blockSizeInPixels);

                        dc.DrawRectangle(GraphicsConstants.BlockBrushes[blockType - 1], GraphicsConstants.BlockBorderPen, rect);
                    }
                }
            }

            // Render the currently moving Piece
            foreach (Block block in _gameBoard.Piece.Blocks)
            {
                Rect rect = new Rect(
                    (_gameBoard.Piece.CoordsX + block.CoordsX) * _blockSizeInPixels,
                    (_gameBoard.Piece.CoordsY + block.CoordsY) * _blockSizeInPixels,
                    _blockSizeInPixels, _blockSizeInPixels);

                dc.DrawRectangle(GraphicsConstants.BlockBrushes[(int)_gameBoard.Piece.PieceType - 1], GraphicsConstants.BlockBorderPen, rect);
            }
        }
    }
}
