using System;
using System.Windows.Input;

namespace Tetris
{
    internal class GameBoard
    {
        public event GameBoardChangedEventHandler GameBoardChanged;

        //Input parameters
        public int VerticalBlocks { get; private set; } //Usually 20
        public int HorizontalBlocks { get; private set; } //Usually 10
        public int BlockSizeInPixels { get; private set; } //Usually 25

        //Static blocks and the currently moving piece
        public int[,] StaticBlocks { get; private set; }
        public Piece CurrentPiece { get; set; }

        public int CurrentPieceSideLengths = 4;
        public PieceBlockManager PieceBlockManager = new PieceBlockManager();
        private readonly Random _random = new Random();

        public GameBoard(int verticalBlocks, int horizontalBlocks, int blockSizeInPixels)
        {
            VerticalBlocks = verticalBlocks;
            HorizontalBlocks = horizontalBlocks;
            BlockSizeInPixels = blockSizeInPixels;

            StaticBlocks = new int[verticalBlocks, horizontalBlocks];
            ResetCurrentPiece();
        }

        private void ResetCurrentPiece()
        {
            //Currently moving piece (randomly selected, and put in the top middle of the canvas)
            CurrentPiece = new Piece((PieceType)_random.Next(1, 8))
            {
                CoordsY = 0,
                CoordsX = ((HorizontalBlocks - CurrentPieceSideLengths) / 2) * BlockSizeInPixels
            };
        }

        public void KeyPressed(Key key)
        {
            switch (key)
            {
                case Key.Left:
                case Key.A:
                    TryMoveCurrentPieceHorizontally(false);
                    break;
                case Key.Right:
                case Key.D:
                    TryMoveCurrentPieceHorizontally(true);
                    break;
                case Key.Up:
                case Key.W:
                    TryRotate();
                    break;
                case Key.Down:
                case Key.S:
                    MoveDown();
                    break;
            }
        }

        // TODO: Detect collision with walls
        private void TryMoveCurrentPieceHorizontally(bool right)
        {
            if (right)
                CurrentPiece.CoordsX += BlockSizeInPixels;
            else
                CurrentPiece.CoordsX -= BlockSizeInPixels;

            OnGameBoardChanged();
        }

        // TODO: Detect collision with walls
        private void TryRotate()
        {
            CurrentPiece.Rotation++;
            OnGameBoardChanged();
        }

        private void MoveDown()
        {
            //TODO
            OnGameBoardChanged();
        }

        protected virtual void OnGameBoardChanged()
        {
            GameBoardChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
