using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Snake
{
    public class UpdateGameSettingsEventArgs : EventArgs
    {
        public Settings Settings { get; }
        public UpdateGameSettingsEventArgs(Settings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
    }

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        public delegate void UpdateGameSettingsEventHandler(object sender, UpdateGameSettingsEventArgs e);
        public event UpdateGameSettingsEventHandler UpdateGameSettingsEvent;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Settings OriginalSettings;
        private const string closingMonit = "Are you sure you want to close the window? Changes will be lost.";
        public Settings Settings { get; private set; }

        public int SliderRowsValue
        {
            get => Settings.Rows;
            set
            {
                Settings.SetRowsCols(value, Settings.Cols);
                OnPropertyChanged();
            }
        }

        public int SliderColsValue
        {
            get => Settings.Cols;
            set
            {
                Settings.SetRowsCols(Settings.Rows, value);
                OnPropertyChanged();
            }
        }

        public double SliderSpeedValue
        {
            get => Settings.Speed;
            set
            {
                Settings.SetSpeed(value);
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SettingsWindow(Settings settings)
        {
            DataContext = this;
            OriginalSettings = settings;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Settings = new Settings(OriginalSettings);

            InitializeComponent();
            ApplySettingsToView(Settings);
        }

        public void ApplySettingsToView(Settings settings)
        {
            SliderRowsValue = settings.Rows;
            SliderColsValue = settings.Cols;
            SliderSpeedValue = settings.Speed;
        }

        protected virtual void OnUpdateGameSettings(Settings settings)
        {
            UpdateGameSettingsEvent?.Invoke(this, new UpdateGameSettingsEventArgs(settings));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Settings != OriginalSettings)
            {
                MessageBoxResult x = MessageBox.Show(closingMonit, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (x == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ButtonSettingsOk_Click(object sender, RoutedEventArgs e)
        {
            OnUpdateGameSettings(Settings);
            Close();
        }

        private void ButtonSettingsCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
