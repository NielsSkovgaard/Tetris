using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tetris.Model
{
    internal class HighScoreList
    {
        public event HighScoreListChangedEventHandler HighScoreListChanged;

        private const int NumberOfHighScores = 5;

        private readonly string _fileName;
        public List<HighScoreEntry> List { get; private set; } // Always sorted from highest to lowest score

        public HighScoreList(string fileName)
        {
            _fileName = fileName;

            BuildListFromFileOrDefault();
        }

        protected virtual void RaiseHighScoreListChanged()
        {
            HighScoreListChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BuildListFromFileOrDefault()
        {
            if (!File.Exists(_fileName))
            {
                List = new List<HighScoreEntry>
                {
                    new HighScoreEntry("CPU", 10000),
                    new HighScoreEntry("CPU", 8000),
                    new HighScoreEntry("CPU", 6000),
                    new HighScoreEntry("CPU", 4000),
                    new HighScoreEntry("CPU", 2000)
                };
            }
            else
            {
                string[] lines = File.ReadLines(_fileName).ToArray();

                List = lines.Select(line =>
                {
                    string[] s = line.Split('\t');
                    return new HighScoreEntry(s[0], Convert.ToInt32(s[1]));
                }).ToList();
            }
        }

        private void SaveToFile()
        {
            string[] lines = List.Select(entry => $"{entry.Name}\t{entry.Score}").ToArray();
            File.WriteAllLines(_fileName, lines);
        }

        public bool IsRecord(int score) => score > List.Last().Score;

        // TODO: Call this method when the game is over
        public void Add(string name, int score)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (score > List[i].Score)
                    List.Insert(i, new HighScoreEntry(name, score));
            }

            List.RemoveAt(NumberOfHighScores - 1);
            SaveToFile();
            RaiseHighScoreListChanged();
        }
    }
}
