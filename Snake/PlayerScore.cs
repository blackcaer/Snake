using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class PlayerScore
    {
        public static readonly PlayerScoreComparer BestScoreComparer = new();
        public int BestScore { get; private set; } = 0;
        public string Name { get; private set; }

        public PlayerScore(string name,int bestScore=0)
        {
            SetName(name);
            SetBestScore(bestScore);
        }

        public void SetBestScore(int bestScore)
        {
            BestScore = bestScore;
        }

        public void SetName(string name)
        {
            Name = name;
        }

    }
}
