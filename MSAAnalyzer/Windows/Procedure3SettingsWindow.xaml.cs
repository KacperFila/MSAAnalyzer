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

namespace MSAAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for Procedure3SettingsWindow.xaml
    /// </summary>
    public partial class Procedure3SettingsWindow : Window
    {
        public Procedure3SettingsWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        public double K3Value
        {
            get
            {
                return K3ValueComboBox.SelectedIndex switch
                {
                    0 => 4.57,
                    1 => 5.32,
                    _ => 4.57
                };
            }
        }

        private void SaveThirdProcedureSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zapisano ustawienia", "Ustawienia", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
