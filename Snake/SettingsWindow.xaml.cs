using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        private readonly Settings OriginalSettings;
        private const string closingMonit = "Are you sure you want to close the window? Changes will be lost.";
        public Settings Settings { get; private set; }

        public int SliderRowsValue
        {
            get { return Settings.Rows; }
            set
            {
                Settings.SetRowsCols(value, Settings.Cols);

                OnPropertyChanged(nameof(SliderRowsValue));
            }
        }

        public int SliderColsValue
        {
            get { return Settings.Cols; }
            set
            {
                Settings.SetRowsCols(Settings.Rows, value);
                OnPropertyChanged(nameof(SliderColsValue));
            }
        }

        public double SliderSpeedValue
        {
            get { return Settings.Speed; }
            set
            {
                Settings.SetSpeed(value);
                OnPropertyChanged(nameof(SliderSpeedValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
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
                var x = MessageBox.Show(closingMonit, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (x == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ButtonSettingsOk_Click(object sender, RoutedEventArgs e)
        {
            OnUpdateGameSettings(Settings);
        }

        private void ButtonSettingsCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
