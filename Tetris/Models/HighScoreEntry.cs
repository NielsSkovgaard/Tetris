namespace Tetris.Models
{
    internal class HighScoreEntry
    {
        public string Initials { get; set; }
        public int Score { get; set; }

        public HighScoreEntry(string initials, int score)
        {
            Initials = initials;
            Score = score;
        }
    }
}
