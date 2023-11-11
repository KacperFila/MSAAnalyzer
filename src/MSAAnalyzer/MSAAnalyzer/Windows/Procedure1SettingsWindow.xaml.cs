using System.Windows;

namespace MSAAnalyzer
{
    public partial class Procedure1SettingsWindow : Window
    {
        public Procedure1SettingsWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public double KValue
        {
            get
            {
                return KValueComboBox.SelectedIndex switch
                {
                    0 => 0.10,
                    1 => 0.15,
                    2 => 0.20,
                    _ => 0.20,
                };
            }
        }

        private void SaveFirstProcedureSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zapisano ustawienia", "Ustawienia", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}