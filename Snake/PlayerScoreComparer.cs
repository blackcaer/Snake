using System.Collections.Generic;

namespace Snake
{
    public class PlayerScoreComparer : IComparer<PlayerScore>
    {
        public int Compare(PlayerScore x, PlayerScore y)
        {
            return x.BestScore.CompareTo(y.BestScore);
        }
    }
}
