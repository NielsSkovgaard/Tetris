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
        public Piece Piece { get; set; }

        public int PieceSideLengths = 4;
        public PieceBlockManager PieceBlockManager = new PieceBlockManager();
        private readonly Random _random = new Random();

        public GameBoard(int verticalBlocks, int horizontalBlocks, int blockSizeInPixels)
        {
            VerticalBlocks = verticalBlocks;
            HorizontalBlocks = horizontalBlocks;
            BlockSizeInPixels = blockSizeInPixels;

            StaticBlocks = new int[verticalBlocks, horizontalBlocks];
            ResetPiece();
        }

        private void ResetPiece()
        {
            //Currently moving piece (randomly selected, and put in the top middle of the canvas)
            Piece = new Piece((PieceType)_random.Next(1, 8))
            {
                CoordsY = 0,
                CoordsX = ((HorizontalBlocks - PieceSideLengths) / 2) * BlockSizeInPixels
            };

            Piece.UpdateCurrentBlocks(PieceBlockManager);
        }

        public void KeyPressed(Key key)
        {
            switch (key)
            {
                case Key.Left:
                case Key.A:
                    TryMovePieceHorizontally(false);
                    break;
                case Key.Right:
                case Key.D:
                    TryMovePieceHorizontally(true);
                    break;
                case Key.Up:
                case Key.W:
                    TryRotatePiece();
                    break;
                case Key.Down:
                case Key.S:
                    MovePieceDown();
                    break;
            }
        }

        // TODO: Detect collision with walls and static blocks
        private void TryMovePieceHorizontally(bool right)
        {
            if (right)
            {
                if (Piece.CoordsX / BlockSizeInPixels + Piece.RightmostBlockIndex + 1 <= HorizontalBlocks - 1)
                {
                    // Example with numbers:
                    // if (100 / 25 + 2 + 1 <= 10 - 1)
                    // if (    4    + 2 + 1 <= 9)        (true, i.e. possible to move right)

                    Piece.CoordsX += BlockSizeInPixels;
                    OnGameBoardChanged();
                }
            }
            else
            {
                if (Piece.CoordsX / BlockSizeInPixels + Piece.LeftmostBlockIndex >= 1)
                {
                    Piece.CoordsX -= BlockSizeInPixels;
                    OnGameBoardChanged();
                }
            }
        }

        // TODO: Detect collision with walls and static blocks
        private void TryRotatePiece()
        {
            Piece.Rotation++;
            Piece.UpdateCurrentBlocks(PieceBlockManager);
            OnGameBoardChanged();
        }

        private void MovePieceDown()
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
