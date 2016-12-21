using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tetris.Models
{
    internal class Statistics : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const int MaximumLevel = 15;
        private const int NumberOfRowsToIncreaseLevel = 10;
        private readonly int[] _scoresToAddForCompletingRows = { 100, 300, 500, 800 };
        private int _level, _score, _lines, _time;

        public int Level { get { return _level; } private set { if (value == _level) return; _level = value; OnPropertyChanged(); } } // Range: 1..15
        public int Score { get { return _score; } private set { if (value == _score) return; _score = value; OnPropertyChanged(); } }
        public int Lines { get { return _lines; } private set { if (value == _lines) return; _lines = value; OnPropertyChanged(); } }
        public int Time { get { return _time; } private set { if (value == _time) return; _time = value; OnPropertyChanged(); } }

        public Statistics()
        {
            Reset();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset()
        {
            Level = 1;
            Score = 0;
            Lines = 0;
            Time = 0;
        }

        public void IncrementScoreForSoftDroppingOneLine() => Score++;
        public void IncrementTime() => Time++;

        public void UpdateOnCompletingRows(int numberOfCompleteRows)
        {
            if (numberOfCompleteRows > 0)
            {
                Lines += numberOfCompleteRows;
                Score += _scoresToAddForCompletingRows[numberOfCompleteRows - 1];
                Level = Math.Min(Lines / NumberOfRowsToIncreaseLevel + 1, MaximumLevel);
            }
        }
    }
}
