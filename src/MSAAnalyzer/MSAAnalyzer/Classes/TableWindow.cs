using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MSAAnalyzer.Classes
{
    public class TableWindow : Window
    {
        private List<List<List<double>>> dataValues = new List<List<List<double>>>();

        public TableWindow(int rows, int columns, int operators)
        {
            Title = "Tabele";
            Width = 500;
            Height = 500;

            for (int i = 0; i < operators; i++)
            {
                var grid = new Grid();
                grid.Margin = new Thickness(10);
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                var label = new Label();
                label.Content = "Operator " + (i + 1);
                label.HorizontalAlignment = HorizontalAlignment.Center;
                grid.Children.Add(label);
                Grid.SetColumn(label, 0);

                var dataGrid = new DataGrid { AutoGenerateColumns = true };
                dataGrid.ItemsSource = CreateEmptyData(rows, columns);
                grid.Children.Add(dataGrid);
                Grid.SetColumn(dataGrid, 0);

                dataValues.Add(CreateEmptyData(rows, columns));

                Content = grid;
            }
        }

        private List<List<double>> CreateEmptyData(int rows, int columns)
        {
            var data = new List<List<double>>();
            for (int i = 0; i < rows; i++)
            {
                var row = new List<double>();
                for (int j = 0; j < columns; j++)
                {
                    row.Add(0.0);
                }
                data.Add(row);
            }

            return data;
        }
    }
}