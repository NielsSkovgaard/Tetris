﻿using System.Globalization;
using System.Linq;
using Tetris.Models;

namespace Tetris.ViewModels
{
    internal class HighScoresViewModel
    {
        public HighScoreList HighScoreList { get; }
        private const int TotalTextLength = 15;

        public HighScoresViewModel(HighScoreList highScoreList)
        {
            HighScoreList = highScoreList;
        }

        public string[] HighScoreEntriesFormated
        {
            get
            {
                return HighScoreList.List
                    .Select(entry => BuildText(entry.Initials, entry.Score.ToString(CultureInfo.InvariantCulture)))
                    .ToArray();
            }
        }

        private static string BuildText(string labelText, string value) => $"{labelText.PadRight(TotalTextLength - value.Length, ' ')}{value}";
    }
}
