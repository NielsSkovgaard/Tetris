using System;
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
        private readonly Pen _blockBorderPen = new Pen { Brush = Brushes.White };

        // Dependency injection of NextPieceViewModel into NextPieceCanvas
        public NextPieceCanvas(NextPieceViewModel nextPieceViewModel)
        {
            _nextPieceViewModel = nextPieceViewModel;

            _nextPieceViewModel.GameBoardCore.NextPieceChanged += NextPieceViewModel_GameBoardCore_NextPieceChanged;
        }

        // Update the View (NextPieceCanvas) every time the model (GameBoardCore.NextPiece) changes
        // Soon after, the OnRender method is called
        private void NextPieceViewModel_GameBoardCore_NextPieceChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Piece nextPiece = _nextPieceViewModel.GameBoardCore.NextPiece;
            int blockSizeInPixels = _nextPieceViewModel.BlockSizeInPixels;

            // Calculate NextPiece coordinates on the NextPieceCanvas
            // Because the NextPiece.CoordsY and NextPiece.CoordsX refer to coordinates on the LockedBlocksAndCurrentPieceCanvas
            double nextPieceCoordsY = _nextPieceViewModel.NextPieceCoordsY;
            double nextPieceCoordsX = _nextPieceViewModel.NextPieceCoordsX;

            foreach (Block block in nextPiece.Blocks)
            {
                Rect rect = new Rect(
                    (nextPieceCoordsX + block.CoordsX) * blockSizeInPixels,
                    (nextPieceCoordsY + block.CoordsY) * blockSizeInPixels,
                    blockSizeInPixels, blockSizeInPixels);

                dc.DrawRectangle(GraphicsConstants.BlockBrushes[(int)nextPiece.PieceType - 1], _blockBorderPen, rect);
            }
        }
    }
}
