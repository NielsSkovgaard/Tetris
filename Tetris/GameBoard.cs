using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    public class GameBoard : Canvas
    {
        public int NumberOfVerticalBlocks { get; set; } //20
        public int NumberOfHorizontalBlocks { get; set; } //10
        public int BlockSizeInPixels { get; set; } //25

        public int[,] StaticBlocks { get; set; }
        public Piece CurrentPiece { get; set; }

        private readonly Random _random = new Random();
        private readonly SolidColorBrush[] _blockBrushes;
        private readonly Pen _blockBorderPen;
        private readonly int _currentPieceSideLengths = 4;

        public GameBoard()
        {
            _blockBrushes = new SolidColorBrush[]
            {
                new SolidColorBrush(Colors.Cyan),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Purple),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Orange),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Red)
            };

            _blockBorderPen = new Pen { Brush = new SolidColorBrush { Color = Colors.DarkGray } };
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //Canvas height and width in pixels
            Height = NumberOfVerticalBlocks * BlockSizeInPixels;
            Width = NumberOfHorizontalBlocks * BlockSizeInPixels;

            StaticBlocks = new int[NumberOfVerticalBlocks, NumberOfHorizontalBlocks];

            //Currently moving piece (randomly selected, and put in the top middle of the canvas)
            CurrentPiece = new Piece((PieceType)_random.Next(1, 8))
            {
                CoordsY = 0,
                CoordsX = ((NumberOfHorizontalBlocks - _currentPieceSideLengths) / 2) * BlockSizeInPixels
            };
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            //Render static blocks
            for (int y = 0; y < NumberOfVerticalBlocks; y++)
            {
                for (int x = 0; x < NumberOfHorizontalBlocks; x++)
                {
                    int blockNumber = StaticBlocks[y, x];

                    // TODO: Or draw black rectangle? Maybe instead have a predefined grid with 20x10 rectangles to color
                    // TODO: How does it work: destruction of rectangles?
                    if (blockNumber != 0)
                    {
                        Rect rect = new Rect(x * BlockSizeInPixels, y * BlockSizeInPixels, BlockSizeInPixels,
                            BlockSizeInPixels);
                        dc.DrawRectangle(_blockBrushes[blockNumber - 1], _blockBorderPen, rect);
                    }
                }
            }

            //Render currently moving piece
            int[,] currentBlocks = CurrentPiece.CurrentBlocks;

            for (int y = 0; y < _currentPieceSideLengths; y++)
            {
                for (int x = 0; x < _currentPieceSideLengths; x++)
                {
                    int blockNumber = currentBlocks[y, x];

                    if (blockNumber != 0)
                    {
                        Rect rect = new Rect(CurrentPiece.CoordsX + x * BlockSizeInPixels,
                            CurrentPiece.CoordsY + y * BlockSizeInPixels, BlockSizeInPixels, BlockSizeInPixels);
                        dc.DrawRectangle(_blockBrushes[blockNumber - 1], _blockBorderPen, rect);
                    }
                }
            }
        }

        // TODO: Detect collision with walls
        public void TryMoveCurrentPieceHorizontally(bool right)
        {
            if (right)
                CurrentPiece.CoordsX += BlockSizeInPixels;
            else
                CurrentPiece.CoordsX -= BlockSizeInPixels;

            // TODO: Is this the right way to do it?
            // TODO: Update the UI in a separate thread: http://stackoverflow.com/questions/5959217/wpf-forcing-redraw-of-canvas
            InvalidateVisual();
        }

        // TODO: Detect collision with walls
        public void TryRotate()
        {
            CurrentPiece.Rotation++;
            InvalidateVisual();
        }
    }
}
