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
        public Piece Piece { get; }

        private readonly Random _random = new Random();

        //Timers for holding down a key to repeat an action (move left/right, rotate, or move down)
        private readonly DispatcherTimer _movePieceLeftTimer = new DispatcherTimer();
        private readonly DispatcherTimer _movePieceRightTimer = new DispatcherTimer();
        private readonly DispatcherTimer _rotatePieceTimer = new DispatcherTimer();
        private readonly DispatcherTimer _movePieceDownTimer = new DispatcherTimer();

        // TODO: Status
        //public int Score { get; set; }
        //public int Level { get; set; }
        //public int Lines { get; set; }

        public GameBoard(int cols, int rows)
        {
            Cols = cols;
            Rows = rows;

            StaticBlocks = new int[cols, rows];

            // Reset the currently moving piece (randomly selected, and positioned in the top middle of the canvas)
            // The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece = new Piece((PieceType)_random.Next(1, 8));
            Piece.CoordsX = (Cols - Piece.Blocks.GetLength(1)) / 2;

            //Timers for holding down a key to repeat an action (move left, move right, rotate, or move down)
            _movePieceLeftTimer.Interval = new TimeSpan(1000000); //1000000 ticks = 100 ms = 10 FPS
            _movePieceRightTimer.Interval = new TimeSpan(1000000); //1000000 ticks = 100 ms = 10 FPS
            _rotatePieceTimer.Interval = new TimeSpan(2500000); //2500000 ticks = 250 ms = 4 FPS
            _movePieceDownTimer.Interval = new TimeSpan(500000); //500000 ticks = 50 ms = 20 FPS
            _movePieceLeftTimer.Tick += (sender, args) => TryMovePieceLeft();
            _movePieceRightTimer.Tick += (sender, args) => TryMovePieceRight();
            _rotatePieceTimer.Tick += (sender, args) => TryRotatePiece();
            _movePieceDownTimer.Tick += (sender, args) => TryMovePieceDown();
        }

        protected virtual void RaiseGameBoardChangedEvent()
        {
            GameBoardChanged?.Invoke(this, EventArgs.Empty);
        }

        public void KeyDown(Key key, bool isRepeat)
        {
            if (isRepeat)
                return;

            // TODO: Should not be possible to move left and right at the same time (gives flicker now)

            switch (key)
            {
                case Key.Left:
                case Key.A:
                    TryMovePieceLeft();
                    _movePieceLeftTimer.Start();
                    break;
                case Key.Right:
                case Key.D:
                    TryMovePieceRight();
                    _movePieceRightTimer.Start();
                    break;
                case Key.Up:
                case Key.W:
                    TryRotatePiece();
                    _rotatePieceTimer.Start();
                    break;
                case Key.Down:
                case Key.S:
                    TryMovePieceDown();
                    _movePieceDownTimer.Start();
                    break;
            }
        }

        public void KeyUp(Key key)
        {
            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _movePieceLeftTimer.Stop();
                    break;
                case Key.Right:
                case Key.D:
                    _movePieceRightTimer.Stop();
                    break;
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

        // TODO: Detect collision with static blocks (then stop timer)
        private void TryMovePieceLeft()
        {
            if (Piece.CoordsX + PieceBlockManager.GetLeftmostBlockIndex(Piece.Blocks) >= 1)
            {
                Piece.MoveLeft();
                RaiseGameBoardChangedEvent();
            }
        }

        // TODO: Detect collision with static blocks (then stop timer)
        private void TryMovePieceRight()
        {
            // Example with numbers:
            // if (4 + 2 + 1 <= 10 - 1) // true, i.e. possible to move right

            if (Piece.CoordsX + PieceBlockManager.GetRightmostBlockIndex(Piece.Blocks) + 1 <= Cols - 1)
            {
                Piece.MoveRight();
                RaiseGameBoardChangedEvent();
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
        }

        private void TryMovePieceDown()
        {
            // TODO: Detect collision with bottom or static blocks (then stop timer)
            Piece.MoveDown();
            RaiseGameBoardChangedEvent();
        }
    }
}
