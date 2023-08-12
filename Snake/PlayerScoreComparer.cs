using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
