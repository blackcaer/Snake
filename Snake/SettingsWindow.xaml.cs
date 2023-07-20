﻿using System;
using System.Collections.Generic;
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
using System.ComponentModel;

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
        private readonly Settings originalSettings;
        private const string closingMonit = "Are you sure you want to close the window? Changes will be lost.";
        public Settings Settings { get; private set; }

        public double sliderRowsValue;
        public double sliderColsValue;
        public double sliderSpeedMtpValue;

        public double SliderRowsValue 
        {
            get { return sliderRowsValue; }
            set 
            { 
                sliderRowsValue = value;
                OnPropertyChanged(nameof(SliderRowsValue));
            }
        }

        public double SliderColsValue
        {
            get { return sliderColsValue; }
            set
            {
                sliderColsValue = value;
                OnPropertyChanged(nameof(SliderColsValue));
            }
        }

        public double SliderSpeedMtpValue
        {
            get { return sliderSpeedMtpValue; }
            set
            {
                sliderSpeedMtpValue = value;
                OnPropertyChanged(nameof(SliderSpeedMtpValue));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SettingsWindow(Settings settings)
        { 
            originalSettings = settings;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            DataContext = this;
            Settings = new Settings(originalSettings);
            ApplySettingsToView(Settings); 
        }

        public void ApplySettingsToView(Settings settings)
        {
            SliderRows.Value = settings.Rows;
            SliderCols.Value = settings.Cols;
            SliderSpeed.Value = TickTimeMtpToSpeedMtp(settings.TickTimeMultiplier);
        }
        private static double SpeedMtpToTickTimeMtp(double speedMtp)
        {
            return 1/(speedMtp/10);
        }
        private static double TickTimeMtpToSpeedMtp(double tickTimeMtp)
        {
            return (1 / tickTimeMtp) * 10;
        }

        protected virtual void OnUpdateGameSettings(Settings settings)
        {
            UpdateGameSettingsEvent?.Invoke(this, new UpdateGameSettingsEventArgs(settings));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Hide();

            if (Settings != originalSettings)
            {
                var x = MessageBox.Show(closingMonit,"Confirmation",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);
            
                if (x == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ButtonSettingsOk_Click(object sender, RoutedEventArgs e)
        {
            // values from sliders etc -> vars
            // vars -> new settings
            // apply new settings
            // reload grid

            Settings.SetRowsCols((int)Math.Round(SliderRows.Value), (int)Math.Round(SliderCols.Value));
            Settings.SetTickTimeMultiplier(SpeedMtpToTickTimeMtp(SliderSpeed.Value));

            OnUpdateGameSettings(Settings);
        }

        private void ButtonSettingsCancel_Click(object sender, RoutedEventArgs e)
        {
            // values from sliders etc -> vars
            // vars -> new settings
            // discard new settings

            //MainWindow main = (MainWindow)Application.Current.MainWindow;
            //ApplySettingsToView(main.settings);

            string x = sliderRowsValue.ToString() +" "+ sliderColsValue.ToString() + " " + sliderSpeedMtpValue.ToString();
            MessageBox.Show(x);
            //Close();
        }
    }
}
