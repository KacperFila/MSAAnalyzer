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
        }

        public double K1Value
        {
            get
            {
                return K1ValueComboBox.SelectedIndex switch
                {
                    0 => 0.8862,
                    1 => 0.0598,
                    _ => 0.8862
                };
            }
        }

        public double K2Value
        {
            get
            {
                return K2ValueComboBox.SelectedIndex switch
                {
                    0 => 0.7071,
                    1 => 0.5231,
                    2 => 0.20,
                    _ => 0.7071
                };
            }
        }

        public double K3Value
        {
            get
            {
                return K3ValueComboBox.SelectedIndex switch
                {
                    0 => 0.7071,
                    1 => 0.5231,
                    2 => 0.4467,
                    3 => 0.4030,
                    4 => 0.3742,
                    5 => 0.3534,
                    6 => 0.3375,
                    7 => 0.3249,
                    8 => 0.3146,
                    _ => 0.7071
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