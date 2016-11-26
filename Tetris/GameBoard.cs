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

        private readonly Random _random = new Random();

        // Dropping a piece
        private readonly DispatcherTimer _movePieceDownTimer;
        private readonly DispatcherTimer _rotatePieceTimer;

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

            // Timers. 
            _rotatePieceTimer = new DispatcherTimer();
            _rotatePieceTimer.Interval = new TimeSpan(2500000); //2500000 ticks = 250 ms = 4 FPS
            _rotatePieceTimer.Tick += (sender, args) => TryRotatePiece();
            _movePieceDownTimer = new DispatcherTimer();
            _movePieceDownTimer.Interval = new TimeSpan(500000); //500000 ticks = 50 ms = 20 FPS
            _movePieceDownTimer.Tick += (sender, args) => TryMovePieceDown();
        }

        private void ResetPiece()
        {
            // Currently moving piece (randomly selected, and positioned in the top middle of the canvas)
            // The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece = new Piece((PieceType)_random.Next(1, 8));
            Piece.CoordsX = (Cols - Piece.Blocks.GetLength(1)) / 2;
        }

        protected virtual void RaiseGameBoardChangedEvent()
        {
            GameBoardChanged?.Invoke(this, EventArgs.Empty);
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
                case Key.Up:
                case Key.W:
                    if (!isRepeat)
                    {
                        TryRotatePiece();
                        _rotatePieceTimer.Start();
                    }
                    break;
                case Key.Down:
                case Key.S:
                    if (!isRepeat)
                    {
                        TryMovePieceDown();
                        _movePieceDownTimer.Start();
                    }
                    break;
            }
        }

        public void KeyUp(Key key)
        {
            switch (key)
            {
                case Key.Up:
                case Key.W:
                    _rotatePieceTimer.Stop();
                    break;
                case Key.Down:
                case Key.S:
                    _movePieceDownTimer.Stop();
                    break;
            }
        }

        // TODO: Detect collision with static blocks (then stop timer - if using a timer)
        private void TryMovePieceHorizontally(bool right)
        {
            if (right)
            {
                if (Piece.CoordsX + PieceBlockManager.GetRightmostBlockIndex(Piece.Blocks) + 1 <= Cols - 1)
                {
                    // Example with numbers:
                    // if (4 + 2 + 1 <= 10 - 1) // true, i.e. possible to move right

                    Piece.MoveRight();
                    RaiseGameBoardChangedEvent();
                }
            }
            else
            {
                if (Piece.CoordsX + PieceBlockManager.GetLeftmostBlockIndex(Piece.Blocks) >= 1)
                {
                    Piece.MoveLeft();
                    RaiseGameBoardChangedEvent();
                }
            }
        }

        // TODO: Detect collision with static blocks (then stop timer)
        private void TryRotatePiece()
        {
            int[,] blocksAfterNextRotation = PieceBlockManager.GetBlocks(Piece.PieceType, Piece.Rotation + 1);

            // Check if next rotation would make the Piece collide with the walls
            if (Piece.CoordsX + PieceBlockManager.GetLeftmostBlockIndex(blocksAfterNextRotation) >= 0 &&
                Piece.CoordsX + PieceBlockManager.GetRightmostBlockIndex(blocksAfterNextRotation) + 1 <= Cols)
            {
                Piece.Rotate();
                RaiseGameBoardChangedEvent();
            }
            else
            {
                _rotatePieceTimer.Stop();
            }
        }

        private void TryMovePieceDown()
        {
            // TODO: Detect collision (then stop timer)
            Piece.MoveDown();
            RaiseGameBoardChangedEvent();
        }
    }
}
