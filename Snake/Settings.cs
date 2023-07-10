using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Settings
    {
        public const int tickTimeStart = 100;
        public const int minSideCells = 2;
        public const int maxSideCells = 30;
        public const double minTickTimeMultiplier = 0.1;
        public const double maxTickTimeMultiplier = 10;
        public const int defaulSideCells = 15;

        public bool SettingsFreezed { get; private set; } = false;

        public double TickTimeMultiplier { get; private set; } = 1.0;
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Settings()
        {
            Rows = defaulSideCells;
            Cols = defaulSideCells;
        }

        public Settings(Settings obj) : this()
        {
            TickTimeMultiplier  = obj.TickTimeMultiplier;
        }

        public void SetTickTimeMultiplier(double NewTickTimeMultiplier)
        {
            if (SettingsFreezed)
                return;

            if (NewTickTimeMultiplier > maxTickTimeMultiplier)
                TickTimeMultiplier = maxTickTimeMultiplier;
            else if (NewTickTimeMultiplier < minTickTimeMultiplier)
                TickTimeMultiplier = minTickTimeMultiplier;
            else
                TickTimeMultiplier = NewTickTimeMultiplier;
        }

        public void SetRowsCols(int NewRows,int NewCols)
        {
            if (SettingsFreezed)
                return;
            Rows = (NewRows >= minSideCells && NewRows <= maxSideCells) ? NewRows : defaulSideCells;
            Cols = (NewCols>=minSideCells && NewCols <= maxSideCells) ? NewCols : defaulSideCells;
        }

        public void FreezeSettings()
        {
            SettingsFreezed = true;
        }

        public void UnfreezeSettings()
        {
            SettingsFreezed = false;
        }

    }
}
