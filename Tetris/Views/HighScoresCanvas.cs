using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.ViewModels;

namespace Tetris.Views
{
    internal class HighScoresCanvas : Canvas
    {
        private readonly HighScoresViewModel _highScoresViewModel;

        private readonly TextBlock[] _textBlockArrayHighScores;

        public HighScoresCanvas(HighScoresViewModel highScoresViewModel)
        {
            _highScoresViewModel = highScoresViewModel;

            // Build and add TextBlocks
            GraphicsTools.BuildTextBlockAndAddToChildren(this, 10, "HIGH SCORES:");
            _textBlockArrayHighScores = _highScoresViewModel.HighScoreList.List
                .Select((entry, index) => GraphicsTools.BuildTextBlockAndAddToChildren(this, 30 + index * 20))
                .ToArray();

            // Update the View (HighScoresCanvas) every time the model (HighScoreList) changes
            // Soon after, the OnRender method is called
            _highScoresViewModel.HighScoreList.Changed += (sender, e) => InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            string[] highScoreEntriesFormated = _highScoresViewModel.HighScoreEntriesFormated;

            for (int i = 0; i < _textBlockArrayHighScores.Length; i++)
                _textBlockArrayHighScores[i].Text = highScoreEntriesFormated[i];
        }
    }
}
