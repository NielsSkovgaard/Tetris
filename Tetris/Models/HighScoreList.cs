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
        public List<HighScoreEntry> List { get; private set; } // Always sorted from highest to lowest score

        public HighScoreList(string fileName)
        {
            _fileName = fileName;

            BuildListFromFileOrDefault();
        }

        protected virtual void RaiseChangedEvent()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void BuildListFromFileOrDefault()
        {
            if (!File.Exists(_fileName))
            {
                List = new List<HighScoreEntry>
                {
                    new HighScoreEntry("-", 0),
                    new HighScoreEntry("-", 0),
                    new HighScoreEntry("-", 0),
                    new HighScoreEntry("-", 0),
                    new HighScoreEntry("-", 0)
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
            string[] lines = List.Select(entry => $"{entry.Initials}\t{entry.Score}").ToArray();
            File.WriteAllLines(_fileName, lines);
        }

        public bool IsRecord(int score) => score > List.Last().Score;

        public void Add(string initials, int score)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (score > List[i].Score)
                {
                    // Format input initials
                    initials = initials.Trim();
                    if (initials.Length == 0)
                        initials = "?";

                    List.Insert(i, new HighScoreEntry(initials, score));
                    break;
                }
            }

            // Limit to 5 high scores, so when one is added, then delete the lowest one
            List.RemoveAt(List.Count - 1);

            SaveToFile();
            RaiseChangedEvent();
        }
    }
}
