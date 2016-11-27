using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    internal class GameCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;
        private readonly int _blockSizeInPixels; // Usually 25px

        // Graphics
        private readonly Pen _blockBorderPen = new Pen { Brush = Brushes.DarkGray };
        private readonly SolidColorBrush[] _blockBrushes = {
            Brushes.Cyan,
            Brushes.Yellow,
            Brushes.Purple,
            Brushes.Blue,
            Brushes.Orange,
            Brushes.Green,
            Brushes.Red
        };

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

            // Render static blocks
            for (int r = 0; r < _gameBoard.Rows; r++)
            {
                for (int c = 0; c < _gameBoard.Cols; c++)
                {
                    // TODO: Consider having a predefined grid with 10x20 rectangles to color
                    // TODO: Here, are many rectangles created and destroyed all the time?
                    // TODO: The GameBoardChanged EventArgs could then include how the Shape looked before (reset these rectangles) and after for repainting
                    int blockType = _gameBoard.StaticBlocks[r, c];

                    if (blockType > 0)
                    {
                        Rect rect = new Rect(
                            c * _blockSizeInPixels,
                            r * _blockSizeInPixels,
                            _blockSizeInPixels, _blockSizeInPixels);

                        dc.DrawRectangle(_blockBrushes[blockType - 1], _blockBorderPen, rect);
                    }
                }
            }

            // Render currently moving piece
            foreach (Block block in _gameBoard.Piece.Blocks)
            {
                Rect rect = new Rect(
                    (_gameBoard.Piece.CoordsX + block.CoordsX) * _blockSizeInPixels,
                    (_gameBoard.Piece.CoordsY + block.CoordsY) * _blockSizeInPixels,
                    _blockSizeInPixels, _blockSizeInPixels);

                dc.DrawRectangle(_blockBrushes[block.BlockType - 1], _blockBorderPen, rect);
            }
        }
    }
}
