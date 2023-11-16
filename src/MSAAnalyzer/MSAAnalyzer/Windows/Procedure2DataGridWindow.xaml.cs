using MSAAnalyzer.Classes;
using MSAAnalyzer.DataContext;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MSAAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for Procedure2DataGridWindow.xaml
    /// </summary>
    public partial class Procedure2DataGridWindow : Window
    {
        private AppDataContext appDataContext;
        private Dictionary<(int, int, int), double> pomiary;
        private Procedure2TableManager procedure2TableManager;
        public Procedure2DataGridWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            appDataContext = (AppDataContext)DataContext;
            procedure2TableManager = new Procedure2TableManager(appDataContext.SecondProcedureMeasurements);
            var dataGrid = procedure2TableManager.CreateMainGrid(this);
            generatedProcedure2DataGrids.Children.Add(dataGrid);
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in generatedProcedure2DataGrids.Children)
            {
                if (child is not Grid grid) continue;
                foreach (var temp in grid.Children)
                {
                    if (child is not Grid temp2) continue;
                    foreach (var item in temp2.Children)
                    {
                        if (item is not ScrollViewer scrollViewer) continue;
                        var scroll = (Grid)scrollViewer.Content;
                        foreach (var item2 in scroll.Children)
                        {
                            if (item2 is not DataGrid dataGrid) continue;
                            foreach (var item3 in dataGrid.ItemsSource)
                            {
                                if (item3 is not SecondProcedureDataGridItem dataGridItem) continue;
                                if (isValidSecondProcedureGridItem(dataGridItem))
                                {
                                    double.TryParse(dataGridItem.Value, out double value);
                                    int.TryParse(dataGridItem.OperatorKey, out int key1);
                                    int.TryParse(dataGridItem.SeriaKey, out int key2);
                                    int.TryParse(dataGridItem.WyrobKey, out int key3);

                                    appDataContext.SecondProcedureMeasurements[(key1, key2, key3)] = value;
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
            
            MessageBox.Show("Zapisano dane!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }


        private bool isValidSecondProcedureGridItem(SecondProcedureDataGridItem dataGridItem)
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
