using System;
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

namespace Snake
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void ButtonSettingsOk_Click(object sender, RoutedEventArgs e)
        {
            // values from sliders etc -> vars
            // vars -> new settings
            // apply new settings
            // reload grid

            
        }

        private void ButtonSettingsCancel_Click(object sender, RoutedEventArgs e)
        {
            // values from sliders etc -> vars
            // vars -> new settings
            // discard new settings
        }
    }
}
