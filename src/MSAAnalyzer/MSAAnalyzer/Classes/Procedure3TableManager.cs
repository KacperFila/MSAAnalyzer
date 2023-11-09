using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MSAAnalyzer.Classes;

public class Procedure3TableManager
{
    private readonly Dictionary<(int, int), double> thirdProcedurePomiary;
    private readonly Window window;

    public Procedure3TableManager(Dictionary<(int, int), double> data, Window window)
    {
        thirdProcedurePomiary = data;
        this.window = window;
    }

    public void CreateProcedure3Tables(List<string> serie)
    {
        var mainGrid = new Grid();
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

        var innerGrid = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(10, 10, 10, 10)
        };

        var columnIndex = 0;

        foreach (var seria in serie)
        {
            var pomiaryZSerii = thirdProcedurePomiary.Where(item => item.Key.Item1.ToString() == seria).ToList();
            if (pomiaryZSerii.Count == 0) continue;

            var items = pomiaryZSerii.Select(item => new ThirdProcedureDataGridItem()
            {
                SeriaKey = item.Key.Item1.ToString(),
                WyrobKey = item.Key.Item2.ToString(),
                Value = item.Value == 0 ? " " : item.Value.ToString()
            }).ToList();

            var dataGrid = CreateThirdProcedureDataGrid(items);

            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            Grid.SetColumn(dataGrid, columnIndex);
            innerGrid.Children.Add(dataGrid);
            columnIndex++;
        }

        var saveProcedure3ValuesButton = new Button
        {
            Content = "Zapisz zmiany",
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(10, 10, 50, 50)
        };

        saveProcedure3ValuesButton.Click += (sender, e) =>
        {
            foreach (var child in innerGrid.Children)
            {
                if (child is DataGrid dataGrid)
                {
                    foreach (var item in dataGrid.Items)
                    {
                        if (item is ThirdProcedureDataGridItem dataGridItem)
                        {
                            var key1 = int.Parse(dataGridItem.SeriaKey);
                            var key2 = int.Parse(dataGridItem.WyrobKey);

                            if (isValidThirdProcedureGridItem(dataGridItem))
                            {
                                double.TryParse(dataGridItem.Value, out double value);
                                thirdProcedurePomiary[(key1, key2)] = value;
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

        innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetColumn(saveProcedure3ValuesButton, 1);
        mainGrid.Children.Add(saveProcedure3ValuesButton);

        window.Content = mainGrid;
        window.ShowDialog();
    }

    private DataGrid CreateThirdProcedureDataGrid(List<ThirdProcedureDataGridItem> items)
    {
        var dataGrid = new DataGrid
        {
            AutoGenerateColumns = false,
            
        };

        dataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Seria",
            Binding = new System.Windows.Data.Binding("SeriaKey")
        });

        dataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Wyrob",
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
        if (value == 0)
        {
            return false;
        }

        return true;
    }
    // Utwórz klasę ThirdProcedureDataGridItem w osobnym pliku lub w tym samym pliku, przed klasą Procedure3TableManager.
    // Następnie utwórz instancję Procedure3TableManager w swoim kodzie i wywołaj metodę CreateProcedure3Tables, przekazując wymagane parametry.
}