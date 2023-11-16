using System;
using System.Windows;

namespace MSAAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for Procedure2SettingsWindow.xaml
    /// </summary>
    public partial class Procedure2SettingsWindow : Window
    {
        public Procedure2SettingsWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public double K1Value
        {
            get
            {
                return K1ValueComboBox.SelectedIndex switch
                {
                    0 => 3.05,
                    1 => 4.56,
                    _ => 3.05
                };
            }
        }

        public double K2Value
        {
            get
            {
                return K2ValueComboBox.SelectedIndex switch
                {
                    0 => 2.70,
                    1 => 3.65,
                    _ => 2.7
                };
            }
        }
        
        private void SaveSecondProcedureSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zapisano ustawienia", "Ustawienia", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}