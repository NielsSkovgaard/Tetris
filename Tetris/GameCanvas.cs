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

        //Dependency injection of GameBoard into GameCanvas
        public GameCanvas(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;

            //Set Canvas height and width (in pixels)
            Width = _gameBoard.Cols * _gameBoard.BlockSizeInPixels;
            Height = _gameBoard.Rows * _gameBoard.BlockSizeInPixels;

            _gameBoard.GameBoardChanged += GameBoard_GameBoardChanged;
        }

        //Update the UI (GameCanvas) every time the model (GameBoard) changes
        //Soon after, the OnRender method is called
        private void GameBoard_GameBoardChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            int blockSizeInPixels = _gameBoard.BlockSizeInPixels; //25px

            //Render static blocks
            for (int r = 0; r < _gameBoard.Rows; r++)
            {
                for (int c = 0; c < _gameBoard.Cols; c++)
                {
                    // TODO: Consider having a predefined grid with 10x20 rectangles to color
                    // TODO: Here, are many rectangles created and destroyed all the time?
                    if (_gameBoard.StaticBlocks[c, r] != 0)
                    {
                        Rect rect = new Rect(
                            c * blockSizeInPixels,
                            r * blockSizeInPixels,
                            blockSizeInPixels, blockSizeInPixels);

                        dc.DrawRectangle(_blockBrushes[_gameBoard.StaticBlocks[c, r] - 1], _blockBorderPen, rect);
                    }
                }
            }

            //Render currently moving piece
            int[,] currentPieceBlocks = _gameBoard.Piece.Blocks;
            int rows = currentPieceBlocks.GetLength(0); //3 or 4
            int cols = currentPieceBlocks.GetLength(1); //3 or 4

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (currentPieceBlocks[r, c] != 0)
                    {
                        Rect rect = new Rect(
                            _gameBoard.Piece.CoordsX + c * blockSizeInPixels,
                            _gameBoard.Piece.CoordsY + r * blockSizeInPixels,
                            blockSizeInPixels, blockSizeInPixels);

                        dc.DrawRectangle(_blockBrushes[currentPieceBlocks[r, c] - 1], _blockBorderPen, rect);
                    }
                }
            }
        }
    }
}
