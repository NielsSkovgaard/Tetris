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

        public NextPieceCanvas(NextPieceViewModel nextPieceViewModel)
        {
            _nextPieceViewModel = nextPieceViewModel;

            _nextPieceViewModel.GameBoard.NextPieceChanged += NextPieceViewModel_GameBoard_NextPieceChanged;
        }

        // Update the View (NextPieceCanvas) every time the model (GameBoard.NextPiece) changes
        // Soon after, the OnRender method is called
        private void NextPieceViewModel_GameBoard_NextPieceChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Piece nextPiece = _nextPieceViewModel.GameBoard.NextPiece;
            int blockSizeInPixels = _nextPieceViewModel.BlockSizeInPixels;

            // Calculate NextPiece coordinates on the NextPieceCanvas
            // Because the NextPiece.CoordsY and NextPiece.CoordsX refer to coordinates on the GameBoardCanvas
            double nextPieceCoordsY = _nextPieceViewModel.NextPieceCoordsY;
            double nextPieceCoordsX = _nextPieceViewModel.NextPieceCoordsX;

            foreach (Block block in nextPiece.Blocks)
            {
                Rect rect = new Rect(
                    (nextPieceCoordsX + block.CoordsX) * blockSizeInPixels,
                    (nextPieceCoordsY + block.CoordsY) * blockSizeInPixels,
                    blockSizeInPixels, blockSizeInPixels);

                dc.DrawRectangle(GraphicsTools.BlockBrushes[(int)nextPiece.PieceType - 1], _blockBorderPen, rect);
            }
        }
    }
}
