using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tetris
{
    internal class GameBoard
    {
        public event GameBoardChangedEventHandler GameBoardChanged;

        // Input parameters
        public int Cols { get; } // Usually 10
        public int Rows { get; } // Usually 20

        // Static blocks and the currently moving piece
        public int[,] StaticBlocks { get; }
        public Piece Piece { get; private set; }

        private readonly PieceBlockManager _pieceBlockManager = new PieceBlockManager();
        private readonly Random _random = new Random();

        // Dropping a piece
        private readonly DispatcherTimer _dropPieceTimer;

        // TODO: Status
        //public int Score { get; set; }
        //public int Level { get; set; }
        //public int Lines { get; set; }

        public GameBoard(int cols, int rows)
        {
            Cols = cols;
            Rows = rows;

            StaticBlocks = new int[cols, rows];
            ResetPiece();

            // Timer
            _dropPieceTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(500000) // 500000 = 50 ms = 20 FPS
            };
            _dropPieceTimer.Tick += DropPieceTimerTick;
        }

        private void ResetPiece()
        {
            // Currently moving piece (randomly selected, and positioned in the top middle of the canvas)
            // The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece = new Piece((PieceType)_random.Next(1, 8), _pieceBlockManager);
            Piece.CoordsX = (Cols - Piece.Blocks.GetLength(1)) / 2;
        }

        protected virtual void RaiseGameBoardChangedEvent()
        {
            GameBoardChanged?.Invoke(this, EventArgs.Empty);
        }

        // ----------------------------------------------------------------------------------------
        // KEY DOWN/UP AND TIMER -- START
        // ----------------------------------------------------------------------------------------

        private void DropPieceTimerTick(object sender, EventArgs e)
        {
            TryMovePieceDown();
        }

        private void TryMovePieceDown()
        {
            // TODO: Collision detection
            Piece.MoveDown();
            RaiseGameBoardChangedEvent();
        }

        public void KeyDown(Key key, bool isRepeat)
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
                case Key.Down:
                case Key.S:
                    if (!isRepeat)
                    {
                        TryMovePieceDown();
                        _dropPieceTimer.Start();
                    }
                    break;
                case Key.Up:
                case Key.W:
                    TryRotatePiece();
                    break;
            }
        }

        public void KeyUp(Key key)
        {
            _dropPieceTimer.Stop();
        }

        // ----------------------------------------------------------------------------------------
        // KEY DOWN/UP AND TIMER -- END
        // ----------------------------------------------------------------------------------------

        // TODO: Detect collision with static blocks
        private void TryRotatePiece()
        {
            int[,] blocksAfterNextRotation = _pieceBlockManager.GetBlocks(Piece.PieceType, Piece.Rotation + 1);
            int leftmostBlockIndex = PieceBlockManager.GetLeftmostBlockIndex(blocksAfterNextRotation);
            int rightmostBlockIndex = PieceBlockManager.GetRightmostBlockIndex(blocksAfterNextRotation);

            // Detects collision with the walls
            if (Piece.CoordsX + leftmostBlockIndex >= 0 &&
                Piece.CoordsX + rightmostBlockIndex + 1 <= Cols)
            {
                Piece.Rotate();
                RaiseGameBoardChangedEvent();
            }
        }

        // TODO: Detect collision with static blocks
        private void TryMovePieceHorizontally(bool right)
        {
            if (right)
            {
                int rightmostBlockIndex = PieceBlockManager.GetRightmostBlockIndex(Piece.Blocks);

                if (Piece.CoordsX + rightmostBlockIndex + 1 <= Cols - 1)
                {
                    // Example with numbers:
                    // if (4 + 2 + 1 <= 10 - 1) // true, i.e. possible to move right

                    Piece.MoveRight();
                    RaiseGameBoardChangedEvent();
                }
            }
            else
            {
                int leftmostBlockIndex = PieceBlockManager.GetLeftmostBlockIndex(Piece.Blocks);

                if (Piece.CoordsX + leftmostBlockIndex >= 1)
                {
                    Piece.MoveLeft();
                    RaiseGameBoardChangedEvent();
                }
            }
        }
    }
}
