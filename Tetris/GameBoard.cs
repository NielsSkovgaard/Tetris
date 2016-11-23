using System;
using System.Windows.Input;

namespace Tetris
{
    internal class GameBoard
    {
        public event GameBoardChangedEventHandler GameBoardChanged;

        //Input parameters
        public int VerticalBlocks { get; } //Usually 20
        public int HorizontalBlocks { get; } //Usually 10
        public int BlockSizeInPixels { get; } //Usually 25

        //Static blocks and the currently moving piece
        public int[,] StaticBlocks { get; private set; }
        public Piece Piece { get; set; }

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
            //Currently moving piece (randomly selected, and positioned in the top middle of the canvas)
            //The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece = new Piece((PieceType)_random.Next(1, 8), PieceBlockManager);
            Piece.CoordsX = (HorizontalBlocks - Piece.CurrentBlocks.GetLength(1)) / 2 * BlockSizeInPixels;
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

        // TODO: Detect collision with static blocks
        private void TryMovePieceHorizontally(bool right)
        {
            if (right)
            {
                int rightmostBlockIndex = PieceBlockManager.GetRightmostBlockIndex(Piece.CurrentBlocks);

                if (Piece.CoordsX / BlockSizeInPixels + rightmostBlockIndex + 1 <= HorizontalBlocks - 1)
                {
                    // Example with numbers:
                    // if (100 / 25 + 2 + 1 <= 10 - 1)
                    // if (    4    + 2 + 1 <= 9)        (true, i.e. possible to move right)

                    Piece.CoordsX += BlockSizeInPixels;
                    RaiseGameBoardChangedEvent();
                }
            }
            else
            {
                int leftmostBlockIndex = PieceBlockManager.GetLeftmostBlockIndex(Piece.CurrentBlocks);

                if (Piece.CoordsX / BlockSizeInPixels + leftmostBlockIndex >= 1)
                {
                    Piece.CoordsX -= BlockSizeInPixels;
                    RaiseGameBoardChangedEvent();
                }
            }
        }

        // TODO: Detect collision with static blocks
        private void TryRotatePiece()
        {
            int[,] blocksAfterNextRotation = PieceBlockManager.GetBlocks(Piece.PieceType, Piece.Rotation + 1);
            int leftmostBlockIndex = PieceBlockManager.GetLeftmostBlockIndex(blocksAfterNextRotation);
            int rightmostBlockIndex = PieceBlockManager.GetRightmostBlockIndex(blocksAfterNextRotation);

            //Detects collision with the walls
            if (Piece.CoordsX / BlockSizeInPixels + leftmostBlockIndex >= 0 &&
                Piece.CoordsX / BlockSizeInPixels + rightmostBlockIndex + 1 <= HorizontalBlocks)
            {
                Piece.Rotate();
                RaiseGameBoardChangedEvent();
            }
        }

        //TODO
        private void MovePieceDown()
        {
            RaiseGameBoardChangedEvent();
        }

        protected virtual void RaiseGameBoardChangedEvent()
        {
            GameBoardChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
