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

            //Render static blocks
            for (int y = 0; y < _gameBoard.VerticalBlocks; y++)
            {
                for (int x = 0; x < _gameBoard.HorizontalBlocks; x++)
                {
                    int blockNumber = _gameBoard.StaticBlocks[y, x];

                    // TODO: Or draw black rectangle? Maybe instead have a predefined grid with 20x10 rectangles to color
                    // TODO: How does it work: destruction of rectangles?
                    if (blockNumber != 0)
                    {
                        int blockSizeInPixels = _gameBoard.BlockSizeInPixels;
                        Rect rect = new Rect(x * blockSizeInPixels, y * blockSizeInPixels, blockSizeInPixels, blockSizeInPixels);
                        dc.DrawRectangle(_blockBrushes[blockNumber - 1], _blockBorderPen, rect);
                    }
                }
            }

            //Render currently moving piece
            int[,] currentBlocks = _gameBoard.CurrentPiece.CurrentBlocks;
            int currentPieceSideLengths = _gameBoard.CurrentPieceSideLengths;

            for (int y = 0; y < currentPieceSideLengths; y++)
            {
                for (int x = 0; x < currentPieceSideLengths; x++)
                {
                    int blockNumber = currentBlocks[y, x];

                    if (blockNumber != 0)
                    {
                        int blockSizeInPixels = _gameBoard.BlockSizeInPixels;
                        Rect rect = new Rect(_gameBoard.CurrentPiece.CoordsX + x * blockSizeInPixels, _gameBoard.CurrentPiece.CoordsY + y * blockSizeInPixels, blockSizeInPixels, blockSizeInPixels);
                        dc.DrawRectangle(_blockBrushes[blockNumber - 1], _blockBorderPen, rect);
                    }
                }
            }
        }
    }
}
