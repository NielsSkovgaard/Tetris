using System;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.ViewModels;

namespace Tetris.Views
{
    internal class HighScoresCanvas : Canvas
    {
        private readonly HighScoresViewModel _highScoresViewModel;

        private readonly TextBlock[] _textBlockArrayHighScores = new TextBlock[5];

        public HighScoresCanvas(HighScoresViewModel highScoresViewModel)
        {
            _highScoresViewModel = highScoresViewModel;

            GraphicsTools.BuildTextBlockAndAddToChildren(this, 10, "HIGH SCORES:");

            for (int i = 0; i < _highScoresViewModel.HighScoreList.List.Count; i++)
                _textBlockArrayHighScores[i] = GraphicsTools.BuildTextBlockAndAddToChildren(this, 30 + i * 20);

            _highScoresViewModel.HighScoreList.Changed += HighScoreViewModel_HighScoreList_Changed;
        }

        // Update the View (HighScoresCanvas) every time the model (HighScoreList) changes
        // Soon after, the OnRender method is called
        private void HighScoreViewModel_HighScoreList_Changed(object sender, EventArgs e)
        {
            InvalidateVisual();
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
