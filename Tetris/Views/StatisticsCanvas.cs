using System;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.ViewModels;

namespace Tetris.Views
{
    internal class StatisticsCanvas : Canvas
    {
        private readonly StatisticsViewModel _statisticsViewModel;

        private readonly TextBlock _textBlockLevel;
        private readonly TextBlock _textBlockScore;
        private readonly TextBlock _textBlockLines;
        private readonly TextBlock _textBlockTime;

        public StatisticsCanvas(StatisticsViewModel statisticsViewModel)
        {
            _statisticsViewModel = statisticsViewModel;

            // Level, Score, Lines, Time
            _textBlockLevel = GraphicsTools.BuildTextBlockAndAddToChildren(this, 10);
            _textBlockScore = GraphicsTools.BuildTextBlockAndAddToChildren(this, 30);
            _textBlockLines = GraphicsTools.BuildTextBlockAndAddToChildren(this, 50);
            _textBlockTime = GraphicsTools.BuildTextBlockAndAddToChildren(this, 70);

            _statisticsViewModel.Statistics.Changed += StatisticsViewModel_Statistics_Changed;
        }

        // Update the View (StatisticsCanvas) every time the model (Statistics) changes
        // Soon after, the OnRender method is called
        private void StatisticsViewModel_Statistics_Changed(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            _textBlockLevel.Text = _statisticsViewModel.LevelText;
            _textBlockScore.Text = _statisticsViewModel.ScoreText;
            _textBlockLines.Text = _statisticsViewModel.LinesText;
            _textBlockTime.Text = _statisticsViewModel.TimeText;
        }
    }
}
