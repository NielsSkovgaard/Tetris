using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Models;
using Tetris.ViewModels;

namespace Tetris.Views
{
    internal class NextPieceCanvas : Canvas
    {
        private readonly NextPieceViewModel _nextPieceViewModel;
        private readonly SolidColorBrush[] _blockBrushes;
        private readonly Pen _blockBorderPen = new Pen { Brush = Brushes.White };

        public NextPieceCanvas(NextPieceViewModel nextPieceViewModel, SolidColorBrush[] blockBrushes)
        {
            _nextPieceViewModel = nextPieceViewModel;
            _blockBrushes = blockBrushes;

            // Update the View (NextPieceCanvas) every time the model (GameBoard.NextPiece) changes
            // Soon after, the OnRender method is called
            _nextPieceViewModel.GameBoard.NextPieceChanged += (sender, e) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Piece nextPiece = _nextPieceViewModel.GameBoard.NextPiece;
            int blockSizeInPixels = _nextPieceViewModel.BlockSizeInPixels;

            // Calculate NextPiece coordinates on the NextPieceCanvas
            // Because NextPiece.CoordsY and NextPiece.CoordsX refer to coordinates on GameBoardCanvas
            double nextPieceCoordsY = _nextPieceViewModel.NextPieceCoordsY;
            double nextPieceCoordsX = _nextPieceViewModel.NextPieceCoordsX;

            foreach (Block block in nextPiece.Blocks)
            {
                Rect rect = new Rect(
                    (nextPieceCoordsX + block.OffsetX) * blockSizeInPixels,
                    (nextPieceCoordsY + block.OffsetY) * blockSizeInPixels,
                    blockSizeInPixels, blockSizeInPixels);

                dc.DrawRectangle(_blockBrushes[(int)nextPiece.PieceType - 1], _blockBorderPen, rect);
            }
        }
    }
}
