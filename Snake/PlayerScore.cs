using System;

namespace Snake
{
    public class PlayerScore
    {
        public const string DefaultName = "unknown";
        public static readonly PlayerScoreComparer BestScoreComparer = new();
        public string Name { get; private set; }
        public int BestScore { get; private set; } = 0;
        public DateTime Date { get; private set; }


        public PlayerScore(string name = DefaultName, int bestScore = 0)
        {
            SetName(name);
            SetBestScore(bestScore);
            Date = DateTime.Now;
        }

        public void SetBestScore(int bestScore)
        {
            BestScore = bestScore;
        }

        public void SetName(string name)
        {
            if (name == "")
            {
                name = DefaultName;
            }

            Name = name;
        }

        public override string ToString()
        {
            return Name + " " + BestScore.ToString();
        }

    }
}
