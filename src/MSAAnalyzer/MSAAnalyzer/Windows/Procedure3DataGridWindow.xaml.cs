using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MSAAnalyzer.Classes;
using MSAAnalyzer.DataContext;

namespace MSAAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for Procedure3DataGridWindow.xaml
    /// </summary>
    public partial class Procedure3DataGridWindow : Window
    {
        private Procedure3TableManager procedure3TableManager;
        private AppDataContext appDataContext;
        public Procedure3DataGridWindow()
        {
            InitializeComponent();
            appDataContext = (AppDataContext)DataContext;
            procedure3TableManager = new Procedure3TableManager(appDataContext.ThirdProcedureMeasurements);
            var serie = new List<string>();
            foreach (var pomiar in appDataContext.ThirdProcedureMeasurements.Where(pomiar => !serie.Contains(pomiar.Key.Item1.ToString())))
            {
                serie.Add(pomiar.Key.Item1.ToString());
            }

            var dataGrid = procedure3TableManager.CreateProcedure3Tables(serie);
            generatedProcedure3DataGrids.Children.Add(dataGrid);
        }

        private void Save2Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in generatedProcedure3DataGrids.Children)
            {
                if (child is Grid dataGrid)
                {
                    foreach (var child2 in dataGrid.Children)
                    {
                        if (child2 is Grid dataGrid2)
                        {
                            foreach (var child3 in dataGrid2.Children)
                            {
                                if (child3 is DataGrid dataGrid3)
                                {
                                    foreach (var item in dataGrid3.Items)
                                    {
                                        if (item is ThirdProcedureDataGridItem dataGridItem)
                                        {
                                            var key1 = int.Parse(dataGridItem.SeriaKey);
                                            var key2 = int.Parse(dataGridItem.WyrobKey);

                                            if (isValidThirdProcedureGridItem(dataGridItem))
                                            {
                                                double.TryParse(dataGridItem.Value, out double value);
                                                appDataContext.ThirdProcedureMeasurements[(key1, key2)] = value;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Wprowadzone wartości zawierają niedozwoloną wartość", "Błąd wartości", MessageBoxButton.OK, MessageBoxImage.Error);
                                                return;
                                            }
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            MessageBox.Show("Zapisano dane!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private bool isValidThirdProcedureGridItem(ThirdProcedureDataGridItem dataGridItem)
        {
            if (string.IsNullOrWhiteSpace(dataGridItem.Value))
            {
                return false;
            }
            if (!double.TryParse(dataGridItem.Value, out double value))
            {
                return false;
            }
            if (value <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
