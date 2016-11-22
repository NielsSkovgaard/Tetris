using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    public class GameBoard : Canvas
    {
        public int NumberOfBlocksHeight { get; set; } //20
        public int NumberOfBlocksWidth { get; set; } //10
        public int BlockSizeInPixels { get; set; } //25

        public int[,] StaticBlocks;

        private readonly SolidColorBrush[] _blockBrushes;
        private readonly Pen _blockBorderPen;

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

            //Canvas width and height in pixels
            Height = NumberOfBlocksHeight * BlockSizeInPixels;
            Width = NumberOfBlocksWidth * BlockSizeInPixels;

            StaticBlocks = new int[NumberOfBlocksHeight, NumberOfBlocksWidth];
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            for (int y = 0; y < NumberOfBlocksHeight; y++)
            {
                for (int x = 0; x < NumberOfBlocksWidth; x++)
                {
                    int blockNumber = StaticBlocks[y, x];

                    //TODO: Or draw black rectangle? Maybe instead have a predefined grid with 20x10 rectangles to color
                    //TODO: How does it work: destruction of triangles?
                    if (blockNumber != 0)
                    {
                        Rect rect = new Rect(x * BlockSizeInPixels, y * BlockSizeInPixels, BlockSizeInPixels, BlockSizeInPixels);
                        dc.DrawRectangle(_blockBrushes[blockNumber - 1], _blockBorderPen, rect);
                    }
                }
            }
        }
    }
}
