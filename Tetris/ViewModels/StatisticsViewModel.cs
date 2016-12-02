using System;
using System.Globalization;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class StatisticsViewModel
    {
        public Statistics Statistics { get; }
        private const int TotalTextLength = 15;

        public StatisticsViewModel(Statistics statistics)
        {
            Statistics = statistics;
        }

        public string LevelText => GetText("Level:", Statistics.Level.ToString(CultureInfo.InvariantCulture));
        public string ScoreText => GetText("Score:", Statistics.Score.ToString(CultureInfo.InvariantCulture));
        public string LinesText => GetText("Lines:", Statistics.Lines.ToString(CultureInfo.InvariantCulture));
        public string TimeText => GetText("Time:", TimeSpan.FromSeconds(Statistics.Time).ToString("mm\\:ss"));

        private string GetText(string labelText, string value) => $"{labelText.PadRight(TotalTextLength - value.Length, ' ')}{value}";
    }
}
