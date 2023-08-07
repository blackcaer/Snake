namespace Snake
{
    public class Settings
    {
        // Constants
        public const double tickTimeStart = 100;
        public const double defaulSideCells = 15;
        public const double minSideCells = 3;
        public const double maxSideCells = 30;

        public const double defSpeed = 100;
        public const double minSpeed = 25;
        public const double maxSpeed = 300;

        // When SettingsFreezed flag is true, methods
        // that change the properties of a setting return immediately
        public bool SettingsFreezed { get; private set; } = false;

        // Properties which can be changed while SettingsFreezed is false:
        public double TickTimeMultiplier { get; private set; } = 1.0;
        public double Speed;
        public int TickTime
        {
            get
            {
                return (int)(tickTimeStart / (Speed / 100));
            }
        }
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Settings()
        {
            Speed = defSpeed;
            Rows = (int)defaulSideCells;
            Cols = (int)defaulSideCells;
        }

        public Settings(Settings settings)
        {
            Speed = defSpeed;
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

            Speed = settingsToBeApplied.Speed;
            Rows = settingsToBeApplied.Rows;
            Cols = settingsToBeApplied.Cols;

            return true;
        }

        public void SetSpeed(double speed)
        {
            if (SettingsFreezed)
                return;

            if (speed > maxSpeed)
            {
                Speed = maxSpeed;
            }
            else if (speed < minSpeed)
            {
                Speed = minSpeed;
            }
            else
            {
                Speed = speed;
            }
        }

        public void SetRowsCols(int NewRows, int NewCols)
        {
            if (SettingsFreezed)
                return;

            Rows = (int)((NewRows >= minSideCells && NewRows <= maxSideCells) ? NewRows : defaulSideCells);
            Cols = (int)((NewCols >= minSideCells && NewCols <= maxSideCells) ? NewCols : defaulSideCells);
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
                    Speed == objset.Speed &&
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
            hash += Speed.GetHashCode();
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
                $"Speed = {Speed}\n " +
                $"SettingsFreezed = {SettingsFreezed}";
        }
    }
}
