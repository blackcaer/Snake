using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Settings
    {
        // Constants
        public const int tickTimeStart = 100;
        public const int minSideCells = 2;
        public const int maxSideCells = 30;
        public const double minTickTimeMultiplier = 0.1;
        public const double maxTickTimeMultiplier = 10;
        public const int defaulSideCells = 15;

        // When SettingsFreezed flag is true, when the X flag is false, methods,
        // that change the properties of a setting return immediately
        public bool SettingsFreezed { get; private set; } = false;

        // Properties which can be changed while SettingsFreezed is false:
        public double TickTimeMultiplier { get; private set; } = 1.0;
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Settings()
        {
            Rows = defaulSideCells;
            Cols = defaulSideCells;
        }

        public Settings(Settings settings)
        {
            ApplySettings(settings);
        }

        public void FreezeSettings()
        {
            SettingsFreezed = true;
        }

        public void UnfreezeSettings()
        {
            SettingsFreezed = false;

        }

        /// <summary>
        /// Copies other Settings to this Settings (if !SettingsFreezed)
        /// </summary>
        /// <param name="settingsToBeApplied"> Settings object to be pasted</param>
        /// <returns> bool indicating if action succeeded</returns>
        public bool ApplySettings(Settings settingsToBeApplied)
        {
            if (SettingsFreezed)
                return false;

            TickTimeMultiplier = settingsToBeApplied.TickTimeMultiplier;
            Rows = settingsToBeApplied.Rows;
            Cols = settingsToBeApplied.Cols;

            return true;
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

        public void SetRowsCols(int NewRows, int NewCols)
        {
            if (SettingsFreezed)
                return;

            Rows = (NewRows >= minSideCells && NewRows <= maxSideCells) ? NewRows : defaulSideCells;
            Cols = (NewCols >= minSideCells && NewCols <= maxSideCells) ? NewCols : defaulSideCells;
        }

        public static bool operator ==(Settings x, Settings y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Settings x, Settings y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is Settings objset)
            {
                return (
                    TickTimeMultiplier == objset.TickTimeMultiplier &&
                    Rows == objset.Rows &&
                    Cols == objset.Cols &&
                    SettingsFreezed == objset.SettingsFreezed
                    );
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash += TickTimeMultiplier.GetHashCode();
            hash = hash * 23 + Rows.GetHashCode();
            hash = hash * 23 + Cols.GetHashCode();
            hash = hash * 23 + SettingsFreezed.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            // Return a custom string representation of the class
            return $"Rows = {Rows} \n " +
                $"Cols = {Cols} \n " +
                $"TickTimeMultiplier = {TickTimeMultiplier}\n " +
                $"SettingsFreezed = {SettingsFreezed}";
        }
    }
}
