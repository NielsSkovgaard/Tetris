using System.ComponentModel;
using System.Windows.Controls;
using Tetris.Models;
using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for StatisticsUserControl.xaml
    /// </summary>
    internal partial class StatisticsUserControl : UserControl
    {
        private readonly StatisticsViewModel _statisticsViewModel;

        public StatisticsUserControl(StatisticsViewModel statisticsViewModel)
        {
            InitializeComponent();

            _statisticsViewModel = statisticsViewModel;

            TextBlockLevel.Text = _statisticsViewModel.LevelText;
            TextBlockScore.Text = _statisticsViewModel.ScoreText;
            TextBlockLines.Text = _statisticsViewModel.LinesText;
            TextBlockTime.Text = _statisticsViewModel.TimeText;

            _statisticsViewModel.Statistics.PropertyChanged += StatisticsViewModel_Statistics_PropertyChanged;
        }

        private void StatisticsViewModel_Statistics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Statistics.Level):
                    TextBlockLevel.Text = _statisticsViewModel.LevelText;
                    break;
                case nameof(Statistics.Score):
                    TextBlockScore.Text = _statisticsViewModel.ScoreText;
                    break;
                case nameof(Statistics.Lines):
                    TextBlockLines.Text = _statisticsViewModel.LinesText;
                    break;
                case nameof(Statistics.Time):
                    TextBlockTime.Text = _statisticsViewModel.TimeText;
                    break;
            }
        }
    }
}
