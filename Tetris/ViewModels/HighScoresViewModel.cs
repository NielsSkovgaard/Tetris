using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class HighScoresViewModel
    {
        public HighScoreList HighScoreList { get; }

        public HighScoresViewModel(HighScoreList highScoreList)
        {
            HighScoreList = highScoreList;
        }
    }
}
