using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MSAAnalyzer.Classes
{
    public class Procedure2TableManager
    {
        public Dictionary<(int, int, int), double> _pomiary;

        public Procedure2TableManager(Dictionary<(int, int, int), double> pomiary)
        {
            _pomiary = pomiary;
        }

        public Grid CreateMainGrid(Window window)
        {

            var mainGrid = new Grid();
            
            var dataGridsGrid = new Grid
            {
                Margin = new Thickness(10, 10, 10, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var dimensions = new List<string>();

            foreach (var pomiar in _pomiary.Where(pomiar => !dimensions.Contains(pomiar.Key.Item1.ToString())))
            {
                dimensions.Add(pomiar.Key.Item1.ToString());
            }

            var columnIndex = 0;

            foreach (var dimension in dimensions)
            {
                var filteredPomiary = _pomiary.Where(item => item.Key.Item1.ToString() == dimension).ToList();
                if (filteredPomiary.Count == 0) continue;

                var dataGrid = new DataGrid
                {
                    CanUserAddRows = false,
                    IsReadOnly = false,
                    Margin = new Thickness(10, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    AlternatingRowBackground = Brushes.LightGray,
                };

                var items = filteredPomiary.Select(item => new SecondProcedureDataGridItem()
                {
                    OperatorKey = item.Key.Item1.ToString(),
                    SeriaKey = item.Key.Item2.ToString(),
                    WyrobKey = item.Key.Item3.ToString(),
                    Value = item.Value == 0 ? " " : item.Value.ToString()
                }).ToList();

                dataGrid.AutoGenerateColumns = false;

                var operatorColumn = new DataGridTextColumn
                {
                    Header = "Operator",
                    Binding = new Binding("OperatorKey"),
                    IsReadOnly = true
                };
                dataGrid.Columns.Add(operatorColumn);

                var seriaColumn = new DataGridTextColumn
                {
                    Header = "Seria",
                    Binding = new Binding("SeriaKey"),
                    IsReadOnly = true
                };
                dataGrid.Columns.Add(seriaColumn);

                var wyrobColumn = new DataGridTextColumn
                {
                    Header = "Wyrób",
                    Binding = new Binding("WyrobKey"),
                    IsReadOnly = true
                };
                dataGrid.Columns.Add(wyrobColumn);

                var valueColumn = new DataGridTextColumn
                {
                    Header = "Wartość",
                    Binding = new Binding("Value")
                };
                dataGrid.Columns.Add(valueColumn);

                dataGrid.ItemsSource = items;

                dataGridsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                Grid.SetColumn(dataGrid, columnIndex);
                dataGridsGrid.Children.Add(dataGrid);
                columnIndex++;
            }

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = dataGridsGrid
            };

            Grid.SetRow(scrollViewer, 1);
            mainGrid.Children.Add(scrollViewer);

            return mainGrid;
        }


    }
}
