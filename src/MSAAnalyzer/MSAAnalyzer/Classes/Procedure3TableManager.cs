﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MSAAnalyzer.Classes
{
    public class Procedure3TableManager
    {
        private readonly Dictionary<(int, int), double> thirdProcedureMeasurements;
        private readonly Window window;

        public Procedure3TableManager(Dictionary<(int, int), double> data, Window window)
        {
            thirdProcedureMeasurements = data;
            this.window = window;
        }

        public void CreateProcedure3Tables(List<string> serie)
        {
            var mainGrid = new Grid();

            var textBlock = new TextBlock
            {
                Text = "PROCEDURA 3",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10),
                FontWeight = FontWeights.Bold,
                FontSize = 25,
            };

            mainGrid.Children.Add(textBlock);
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
                var pomiaryZSerii = thirdProcedureMeasurements.Where(item => item.Key.Item1.ToString() == seria).ToList();
                if (pomiaryZSerii.Count == 0) continue;

                var items = pomiaryZSerii.Select(item => new ThirdProcedureDataGridItem()
                {
                    SeriaKey = item.Key.Item1.ToString(),
                    WyrobKey = item.Key.Item2.ToString(),
                    Value = item.Value == 0 ? " " : item.Value.ToString()
                }).ToList();

                var dataGrid = CreateThirdProcedureDataGrid(items);

                innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                Grid.SetColumn(dataGrid, columnIndex);
                innerGrid.Children.Add(dataGrid);
                columnIndex++;
            }

            var saveProcedure3ValuesButton = new Button
            {
                Content = "Zapisz zmiany",
                Width = 100,
                Margin = new Thickness(0, 20, 0 ,0)
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
                                    thirdProcedureMeasurements[(key1, key2)] = value;
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

            // Definicja wierszy dla mainGrid
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) }); // Wiersz 0 dla TextBlock
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // Wiersz 1 dla innerGrid
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) }); // Wiersz 2 dla przycisku

            // Ustawienia dla elementów w mainGrid
            Grid.SetRow(textBlock, 0);
            Grid.SetRow(innerGrid, 1);
            Grid.SetRow(saveProcedure3ValuesButton, 2);

            // Dodanie elementów do mainGrid
            mainGrid.Children.Add(innerGrid);
            mainGrid.Children.Add(saveProcedure3ValuesButton);

            window.Content = mainGrid;
            window.ShowDialog();
        }

        private static DataGrid CreateThirdProcedureDataGrid(IEnumerable<ThirdProcedureDataGridItem> items)
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
