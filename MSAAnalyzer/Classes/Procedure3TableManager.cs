using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MSAAnalyzer.Classes
{
    public class Procedure3TableManager
    {
        private readonly Dictionary<(int, int), double> thirdProcedureMeasurements;

        public Procedure3TableManager(Dictionary<(int, int), double> data)
        {
            thirdProcedureMeasurements = data;
        }

        public Grid CreateProcedure3Tables(List<string> serie)
        {
            var mainGrid = new Grid();
            
            mainGrid.VerticalAlignment = VerticalAlignment.Center;
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            var innerGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 50, 10, 10)
            };

            var columnIndex = 0;

            foreach (var seria in serie)
            {
                var pomiaryZSerii = thirdProcedureMeasurements
                    .Where(item => item.Key.Item1.ToString() == seria)
                    .ToList();
                if (pomiaryZSerii.Count == 0) continue;

                var items = pomiaryZSerii
                    .Select(item => new ThirdProcedureDataGridItem()
                {
                    SeriaKey = item.Key.Item1.ToString(),
                    WyrobKey = item.Key.Item2.ToString(),
                    Value = item.Value == 0 ? " " : item.Value.ToString()
                }).ToList();

                var dataGrid = CreateThirdProcedureDataGrid(items);

                innerGrid.ColumnDefinitions.Add(new ColumnDefinition() 
                    { Width = new GridLength(1, GridUnitType.Auto) });
                Grid.SetColumn(dataGrid, columnIndex);
                innerGrid.Children.Add(dataGrid);
                columnIndex++;
            }

            // Definicja wierszy dla mainGrid
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) }); // Wiersz 0 dla TextBlock
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // Wiersz 1 dla innerGrid
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) }); // Wiersz 2 dla przycisku

            // Ustawienia dla elementów w mainGrid
            Grid.SetRow(innerGrid, 1);

            // Dodanie elementów do mainGrid
            mainGrid.Children.Add(innerGrid);
            return mainGrid;
        }

        private static DataGrid CreateThirdProcedureDataGrid(
            IEnumerable<ThirdProcedureDataGridItem> items)
        {
            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                Margin = new Thickness(10, 0, 10, 0),
                AlternatingRowBackground = Brushes.LightGray
            };

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Seria",
                Binding = new System.Windows.Data.Binding("SeriaKey")
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Wyrób",
                Binding = new System.Windows.Data.Binding("WyrobKey")
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Wartość",
                Binding = new System.Windows.Data.Binding("Value")
            });

            dataGrid.ItemsSource = items;
            return dataGrid;
        }
    }
}
