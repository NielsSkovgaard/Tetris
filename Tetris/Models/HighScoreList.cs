using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tetris.Models
{
    internal class HighScoreList
    {
        public event EventHandler Changed;
        private readonly string _fileName;

        public List<HighScoreEntry> List { get; } // Always sorted from highest to lowest score

        public HighScoreList(string fileName)
        {
            _fileName = fileName;

            List = BuildListFromFileOrDefault();
        }

        protected virtual void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);

        private List<HighScoreEntry> BuildListFromFileOrDefault()
        {
            if (!File.Exists(_fileName))
                return Enumerable.Repeat(new HighScoreEntry("-", 0), 5).ToList();

            return File.ReadLines(_fileName)
                .Select(line => line.Split('\t'))
                .Select(subStrings => new HighScoreEntry(subStrings[0], Convert.ToInt32(subStrings[1])))
                .ToList();
        }

        private void SaveToFile()
        {
            IEnumerable<string> lines = List.Select(entry => $"{entry.Initials}\t{entry.Score}");
            File.WriteAllLines(_fileName, lines);
        }

        public bool IsRecord(int score) => score >= List.Last().Score;

        public void Add(string initials, int score)
        {
            if (!IsRecord(score))
                return;

            for (int i = 0; i < List.Count; i++)
            {
                if (score >= List[i].Score)
                {
                    List.Insert(i, new HighScoreEntry(initials, score));
                    break;
                }
            }

            // Limit to 5 high scores, so when one is added, delete the lowest one
            List.RemoveAt(List.Count - 1);

            SaveToFile();
            OnChanged();
        }
    }
}
