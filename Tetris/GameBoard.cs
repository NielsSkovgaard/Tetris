using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tetris
{
    internal class GameBoard
    {
        public event GameBoardChangedEventHandler GameBoardChanged;

        // Input parameters
        public int Rows { get; } // Usually 20
        public int Cols { get; } // Usually 10

        // Static blocks and the currently moving piece
        public int[,] StaticBlocks { get; }
        public Piece Piece { get; }

        private readonly Random _random = new Random();

        //Timers for holding down a key to repeat an action (move left/right, rotate, or move down)
        private readonly DispatcherTimer _movePieceLeftRightTimer = new DispatcherTimer();
        private readonly DispatcherTimer _rotatePieceTimer = new DispatcherTimer();
        private readonly DispatcherTimer _movePieceDownTimer = new DispatcherTimer();

        private bool _isLeftKeyDown;
        private bool _isRightKeyDown;
        private bool _leftKeyHasPriority;

        // TODO: Status
        //public int Score { get; set; }
        //public int Level { get; set; }
        //public int Lines { get; set; }

        public GameBoard(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            StaticBlocks = new int[rows, cols];

            // Reset the currently moving piece (randomly selected, and positioned in the top middle of the canvas)
            // The random number is >= 1 and < 8, i.e. in the interval 1..7
            Piece = new Piece((PieceType)_random.Next(1, 8));
            Piece.CoordsX = (Cols - PieceBlockManager.GetWidthOfBlockArray(Piece.PieceType)) / 2;

            //Timers for holding down a key to repeat an action (move left/right, rotate, or move down)
            _movePieceLeftRightTimer.Interval = new TimeSpan(1000000); //1000000 ticks = 100 ms = 10 FPS
            _rotatePieceTimer.Interval = new TimeSpan(2500000); //2500000 ticks = 250 ms = 4 FPS
            _movePieceDownTimer.Interval = new TimeSpan(500000); //500000 ticks = 50 ms = 20 FPS
            _movePieceLeftRightTimer.Tick += (sender, args) => TryMovePieceLeftOrRight();
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

            switch (key)
            {
                case Key.Left:
                case Key.A:
                    _isLeftKeyDown = true;
                    _leftKeyHasPriority = true;
                    TryMovePieceLeft();
                    _movePieceLeftRightTimer.Start();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = true;
                    _leftKeyHasPriority = false;
                    TryMovePieceRight();
                    _movePieceLeftRightTimer.Start();
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
                    _isLeftKeyDown = false;
                    _leftKeyHasPriority = false;

                    if (!_isRightKeyDown)
                        _movePieceLeftRightTimer.Stop();
                    break;
                case Key.Right:
                case Key.D:
                    _isRightKeyDown = false;
                    _leftKeyHasPriority = true;

                    if (!_isLeftKeyDown)
                        _movePieceLeftRightTimer.Stop();
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

        private void TryMovePieceLeftOrRight()
        {
            if (_leftKeyHasPriority)
                TryMovePieceLeft();
            else
                TryMovePieceRight();
        }

        private void TryMovePieceLeft()
        {
            Piece.MoveLeft();

            if (IsPieceInValidPosition())
                RaiseGameBoardChangedEvent();
            else
                Piece.MoveRight();
        }

        private void TryMovePieceRight()
        {
            Piece.MoveRight();

            if (IsPieceInValidPosition())
                RaiseGameBoardChangedEvent();
            else
                Piece.MoveLeft();
        }

        private void TryRotatePiece()
        {
            Piece.Rotate();

            if (IsPieceInValidPosition())
                RaiseGameBoardChangedEvent();
            else
                Piece.RotateBack();
        }

        private void TryMovePieceDown()
        {
            Piece.MoveDown();

            if (IsPieceInValidPosition())
            {
                RaiseGameBoardChangedEvent();
                // TODO: Make current piece part of static blocks, and build a new piece in the top of the canvas
            }
            else
            {
                Piece.MoveUp();
            }
        }

        private bool IsPieceInValidPosition()
        {
            return Piece.Blocks.All(block =>
                Piece.CoordsX + block.CoordsX >= 0 && // Check for collision with left side
                Piece.CoordsX + block.CoordsX + 1 <= Cols && // Check for collision with right side
                Piece.CoordsY + block.CoordsY + 1 <= Rows && // Check for collision with bottom
                StaticBlocks[Piece.CoordsY + block.CoordsY, Piece.CoordsX + block.CoordsX] == 0); // Check for collision with static blocks
        }
    }
}
