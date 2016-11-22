using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    public class GameBoard : Canvas
    {
        public int NumberOfBlocksWidth { get; set; } //10
        public int NumberOfBlocksHeight { get; set; } //20
        public int BlockSizeInPixels { get; set; } //25

        private const int BlocksArrayLength = 4;
        private readonly Pen _blockBorderPen;

        public GameBoard()
        {
            //Canvas width and height
            Width = NumberOfBlocksWidth * BlockSizeInPixels;
            Height = NumberOfBlocksHeight * BlockSizeInPixels;

            _blockBorderPen = new Pen { Brush = new SolidColorBrush { Color = Colors.DarkGray } };
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Piece piece = new Piece(PieceType.I);
            bool[,] currentBlocks = piece.CurrentBlocks;
            SolidColorBrush blockBackgroundBrush = new SolidColorBrush { Color = piece.Color };

            int offsetX = 0 * BlockSizeInPixels;
            int offsetY = 0 * BlockSizeInPixels;

            for (int i = 0; i < BlocksArrayLength; i++)
            {
                for (int j = 0; j < BlocksArrayLength; j++)
                {
                    if (currentBlocks[i, j])
                    {
                        Rect rect = new Rect(
                            offsetX + (i * BlockSizeInPixels),
                            offsetY + (j * BlockSizeInPixels),
                            BlockSizeInPixels, BlockSizeInPixels);

                        dc.DrawRectangle(blockBackgroundBrush, _blockBorderPen, rect);
                    }
                }
            }
        }
    }
}
