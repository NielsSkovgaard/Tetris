using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using Tetris.Models;
using Tetris.ViewModels;

namespace Tetris.Views
{
    /// <summary>
    /// Interaction logic for HighScoresUserControl.xaml
    /// </summary>
    internal partial class HighScoresUserControl : UserControl
    {
        private readonly HighScoresViewModel _highScoresViewModel;
        private readonly TextBlock[] _textBlocksInitials;
        private readonly TextBlock[] _textBlocksScores;

        public HighScoresUserControl(HighScoresViewModel highScoresViewModel)
        {
            InitializeComponent();

            _highScoresViewModel = highScoresViewModel;
            _textBlocksScores = new[] { TextBlockScore1, TextBlockScore2, TextBlockScore3, TextBlockScore4, TextBlockScore5 };
            _textBlocksInitials = new[] { TextBlockInitials1, TextBlockInitials2, TextBlockInitials3, TextBlockInitials4, TextBlockInitials5 };

            UpdateTextBlocks();

            _highScoresViewModel.HighScoreList.Changed += HighScoresViewModel_HighScoreList_Changed;
        }

        private void UpdateTextBlocks()
        {
            List<HighScoreEntry> highScoreEntries = _highScoresViewModel.HighScoreList.List;

            for (int i = 0; i < highScoreEntries.Count; i++)
            {
                _textBlocksInitials[i].Text = highScoreEntries[i].Initials;
                _textBlocksScores[i].Text = highScoreEntries[i].Score.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void HighScoresViewModel_HighScoreList_Changed(object sender, EventArgs e)
        {
            UpdateTextBlocks();
        }
    }
}
