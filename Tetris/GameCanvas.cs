using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    internal class GameCanvas : Canvas
    {
        private readonly GameBoard _gameBoard;

        //Graphics
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

        public GameCanvas(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;

            //Set Canvas height and width (in pixels)
            Height = _gameBoard.VerticalBlocks * _gameBoard.BlockSizeInPixels;
            Width = _gameBoard.HorizontalBlocks * _gameBoard.BlockSizeInPixels;

            _gameBoard.GameBoardChanged += GameBoard_GameBoardChanged;
        }

        private void GameBoard_GameBoardChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            int blockSizeInPixels = _gameBoard.BlockSizeInPixels;

            //Render static blocks
            for (int y = 0; y < _gameBoard.VerticalBlocks; y++)
            {
                for (int x = 0; x < _gameBoard.HorizontalBlocks; x++)
                {
                    // TODO: Consider having a predefined grid with 20x10 rectangles to color
                    // TODO: Here, are many rectangles created and destroyed all the time?
                    if (_gameBoard.StaticBlocks[y, x] != 0)
                    {
                        Rect rect = new Rect(x * blockSizeInPixels, y * blockSizeInPixels, blockSizeInPixels, blockSizeInPixels);
                        dc.DrawRectangle(_blockBrushes[_gameBoard.StaticBlocks[y, x] - 1], _blockBorderPen, rect);
                    }
                }
            }

            //Render currently moving piece
            int[,] currentBlocks = _gameBoard.Piece.CurrentBlocks;

            for (int y = 0; y < currentBlocks.GetLength(0); y++)
            {
                for (int x = 0; x < currentBlocks.GetLength(1); x++)
                {
                    if (currentBlocks[y, x] != 0)
                    {
                        Rect rect = new Rect(_gameBoard.Piece.CoordsX + x * blockSizeInPixels, _gameBoard.Piece.CoordsY + y * blockSizeInPixels, blockSizeInPixels, blockSizeInPixels);
                        dc.DrawRectangle(_blockBrushes[currentBlocks[y, x] - 1], _blockBorderPen, rect);
                    }
                }
            }
        }
    }
}
