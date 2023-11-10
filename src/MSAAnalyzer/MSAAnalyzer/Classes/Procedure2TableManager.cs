using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MSAAnalyzer.Classes
{
    public class Procedure2TableManager
    {
        public Procedure2TableManager()
        {
        }

        public Grid CreateMainGrid(Dictionary<(int, int, int), double> pomiary, Window window)
        {
            var mainGrid = new Grid();

            // Add three rows to mainGrid
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

            var textBlock = new TextBlock
            {
                Text = "PROCEDURA 2",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 25,
                Margin = new Thickness(0, 50, 0, 10)
            };
            Grid.SetRow(textBlock, 0);
            mainGrid.Children.Add(textBlock);

            var dataGridsGrid = new Grid
            {
                Margin = new Thickness(10, 10, 10, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var dimensions = new List<string>();

            foreach (var pomiar in pomiary.Where(pomiar => !dimensions.Contains(pomiar.Key.Item1.ToString())))
            {
                dimensions.Add(pomiar.Key.Item1.ToString());
            }

            var columnIndex = 0;

            foreach (var dimension in dimensions)
            {
                var filteredPomiary = pomiary.Where(item => item.Key.Item1.ToString() == dimension).ToList();
                if (filteredPomiary.Count == 0) continue;

                var dataGrid = new DataGrid
                {
                    CanUserAddRows = false,
                    IsReadOnly = false,
                    Margin = new Thickness(10, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center
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

            var saveButton = new Button
            {
                Content = "Zapisz zmiany",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10, 10, 50, 50)
            };
            saveButton.Click += (sender, e) =>
            {
                foreach (var child in dataGridsGrid.Children)
                {
                    if (child is DataGrid dataGrid)
                    {
                        foreach (var item in dataGrid.Items)
                        {
                            if (item is SecondProcedureDataGridItem dataGridItem)
                            {
                                var key1 = int.Parse(dataGridItem.OperatorKey); // Pobierz numer operatora
                                var key2 = int.Parse(dataGridItem.SeriaKey); // Pobierz numer serii
                                var key3 = int.Parse(dataGridItem.WyrobKey); // Pobierz numer wyrobu

                                if (isValidSecondProcedureGridItem(dataGridItem))
                                {
                                    double.TryParse(dataGridItem.Value, out var value);
                                    pomiary[(key1, key2, key3)] = value;
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

                MessageBox.Show("Zapisano dane!", "Zawartość pomiarów", MessageBoxButton.OK, MessageBoxImage.Information);
                window.Close();
            };

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = dataGridsGrid
            };

            Grid.SetRow(scrollViewer, 1);
            mainGrid.Children.Add(scrollViewer);
            Grid.SetRow(saveButton, 2);
            mainGrid.Children.Add(saveButton);

            return mainGrid;
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
