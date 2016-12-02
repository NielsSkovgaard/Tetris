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

        // Dependency injection of StatisticsViewModel into StatisticsCanvas
        public StatisticsCanvas(StatisticsViewModel statisticsViewModel)
        {
            _statisticsViewModel = statisticsViewModel;

            // Level, Score, Lines, Time
            _textBlockLevel = BuildTextBlockAndAddToChildren(10);
            _textBlockScore = BuildTextBlockAndAddToChildren(30);
            _textBlockLines = BuildTextBlockAndAddToChildren(50);
            _textBlockTime = BuildTextBlockAndAddToChildren(70);

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

            // Level, Score, Lines, Time
            _textBlockLevel.Text = _statisticsViewModel.LevelText;
            _textBlockScore.Text = _statisticsViewModel.ScoreText;
            _textBlockLines.Text = _statisticsViewModel.LinesText;
            _textBlockTime.Text = _statisticsViewModel.TimeText;
        }

        // ----------------------------------------------------------------------------------------
        // HELPER METHODS (same in StatisticsCanvas and HighScoresCanvas)
        // ----------------------------------------------------------------------------------------

        private TextBlock BuildTextBlockAndAddToChildren(double top, string text = null)
        {
            TextBlock textBlock = new TextBlock { Text = text ?? string.Empty, Foreground = Brushes.White, FontFamily = new FontFamily("Consolas, Courier New") };
            SetTop(textBlock, top);
            SetLeft(textBlock, 10);
            Children.Add(textBlock);
            return textBlock;
        }
    }
}
