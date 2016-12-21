using System;
using System.Globalization;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class StatisticsViewModel
    {
        public Statistics Statistics { get; }

        public StatisticsViewModel(Statistics statistics)
        {
            Statistics = statistics;
        }

        public string LevelText => Statistics.Level.ToString(CultureInfo.InvariantCulture);
        public string ScoreText => Statistics.Score.ToString(CultureInfo.InvariantCulture);
        public string LinesText => Statistics.Lines.ToString(CultureInfo.InvariantCulture);
        public string TimeText => TimeSpan.FromSeconds(Statistics.Time).ToString("mm\\:ss", CultureInfo.InvariantCulture);
    }
}
