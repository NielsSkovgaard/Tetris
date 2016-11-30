using System;

namespace Tetris.Models
{
    internal class Statistics
    {
        public event EventHandler Changed;

        public int Level { get; private set; } // Between 1 and 15
        public int Score { get; private set; }
        public int Lines { get; private set; }
        public int Time { get; private set; }

        private const int MaximumLevel = 15;
        private const int NumberOfRowsToIncreaseLevel = 10;
        private readonly int[] _scoresToAddForCompletingRows = { 100, 300, 500, 800 };

        public Statistics()
        {
            Reset();
        }

        protected virtual void RaiseChangedEvent()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
        
        public void Reset()
        {
            Level = 1;
            Score = 0;
            Lines = 0;
            Time = 0;

            RaiseChangedEvent();
        }

        public void IncrementScoreForSoftDroppingOneLine()
        {
            Score++;
            RaiseChangedEvent();
        }

        public void IncrementTime()
        {
            Time++;
            RaiseChangedEvent();
        }

        public void UpdateOnCompletingRows(int numberOfCompleteRows)
        {
            if (numberOfCompleteRows > 0)
            {
                Lines += numberOfCompleteRows;
                Score += _scoresToAddForCompletingRows[numberOfCompleteRows - 1];
                Level = Math.Min(Lines / NumberOfRowsToIncreaseLevel + 1, MaximumLevel);

                RaiseChangedEvent();
            }
        }
    }
}
