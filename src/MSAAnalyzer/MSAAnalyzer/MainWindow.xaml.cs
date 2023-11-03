using MSAAnalyzer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MSAAnalyzer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<double> listaPomiarow = new();
    private bool fieldsValidated = false;
    private FirstProcedure firstProcedure;
    DispatcherTimer timer;
    private List<List<double>> dataValues = new List<List<double>>();
    public static Dictionary<(int, int, int), double> pomiary = new Dictionary<(int, int, int), double>();
        
    public MainWindow()
    {
        InitializeComponent();
        firstProcedure = new FirstProcedure(listaPomiarow);
        UpdateUIWithResults();
        timer = new DispatcherTimer();
        timer.Tick += Timer_Tick;
        timer.Interval = TimeSpan.FromSeconds(2);
        KolejnyPomiarTextBox.IsEnabled = false;

        pomiary.Add((1, 1, 1), 0); 
        pomiary.Add((1, 1, 2), 0);
        pomiary.Add((1, 2, 1), 0);
        pomiary.Add((1, 2, 2), 0);
        pomiary.Add((2, 1, 1), 0);
        pomiary.Add((2, 1, 2), 0);
        pomiary.Add((2, 2, 1), 0);
        pomiary.Add((2, 2, 2), 0);
    }

    #region procedura1
    private void Timer_Tick(object sender, EventArgs e)
    {
        ZapisanoDaneTextBox.Text = "";
        timer.Stop();
    }

    private void ValidateAndEnableKolejnyPomiarTextBox()
    {
        if (ValidateTextBoxes())
        {
            fieldsValidated = true;
            KolejnyPomiarTextBox.IsEnabled = true;
        }
        else
        {
            fieldsValidated = false;
            KolejnyPomiarTextBox.IsEnabled = false;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        listaPomiarow.Clear();
        double.TryParse(WartoscWzorcaTextBox.Text, out var wartoscWzorca);
        double.TryParse(GornaGranicaTextBox.Text, out var gorna);
        double.TryParse(DolnaGranicaTextBox.Text, out var dolna);

        firstProcedure.SetWartoscWzorca(wartoscWzorca);
        firstProcedure.SetGranice(gorna, dolna);

        ValidateAndEnableKolejnyPomiarTextBox();
        if (!fieldsValidated) return;

        UpdateUIWithResults();
        ZapisanoDaneTextBox.Text = "Dane zostały zapisane";
        timer.Start();
    }

    private void KolejnyPomiarTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        var pomiar = double.TryParse(KolejnyPomiarTextBox.Text, out var pomiarValue);
        if (!pomiar || pomiarValue <= 0)
        {
            MessageBox.Show("Nieprawidłowa wartość dla pomiaru.");
        }
        else { 

            listaPomiarow.Add(pomiarValue);

            KolejnyPomiarTextBox.Clear();
            firstProcedure.UpdateData(listaPomiarow);
            firstProcedure.IncrementLicznikElementow();
            ValidateTextBoxes();
            UpdateUIWithResults();
        }
    }

    private void UpdateUIWithResults()
    {
        var result = firstProcedure.Calculate();

        TTextBox.Text = result.T.ToString();
           
        LiczbaPomiarowTextBox.Text = result.LicznikElementow.ToString();

           

        if (result.LicznikElementow > 3)
        {
            ZdolnoscSystemuTextBox.Text = result is { Cg: > 1.33, Cgk: > 1.33 } ? "SYSTEM JEST ZDOLNY" : "SYSTEM NIE JEST ZDOLNY";
            ZdolnoscSystemuTextBox.Foreground = result is { Cg: > 1.33, Cgk: > 1.33 } 
                ? new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAF, 0x83))
                : new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x3D, 0x3D));

            OdchylenieTextBox.Text = result.Sigma.ToString();
            SredniaTextBox.Text = result.Mean.ToString();
            CgTextBox.Text = result.Cg.ToString();
            CgkTextBox.Text = result.Cgk.ToString();
        }
        else
        {
            ZdolnoscSystemuTextBox.Text = string.Empty;
            OdchylenieTextBox.Text = string.Empty;
            SredniaTextBox.Text = string.Empty;
            CgTextBox.Text = string.Empty;
            CgkTextBox.Text = string.Empty;
        }
    }

    private bool ValidateTextBoxes()
    {
        var errorMessages = new List<string>();

        if (!double.TryParse(WartoscWzorcaTextBox.Text, out var wzorzecValue) || wzorzecValue < 0)
        {
            errorMessages.Add("Nieprawidłowa wartość dla wzorca.");
        }

        if (!double.TryParse(GornaGranicaTextBox.Text, out var gornaGranicaValue) || gornaGranicaValue < 0)
        {
            errorMessages.Add("Nieprawidłowa wartość dla górnej granicy.");
        }

        if (!double.TryParse(DolnaGranicaTextBox.Text, out var dolnaGranicaValue) || dolnaGranicaValue < 0)
        {
            errorMessages.Add("Nieprawidłowa wartość dla dolnej granicy.");
        }

        if (!double.TryParse(RozdzielczoscTextBox.Text, out var rozdzielczoscValue) || rozdzielczoscValue < 0)
        {
            errorMessages.Add("Nieprawidłowa wartość dla rozdzielczości.");
        }

        if (dolnaGranicaValue > gornaGranicaValue)
        {
            errorMessages.Add("Górna granica nie może być mniejsza od dolnej.");
        }

        if (dolnaGranicaValue > wzorzecValue)
        {
            errorMessages.Add("Dolna granica nie może być większa od wzorca.");
        }

        if (gornaGranicaValue < wzorzecValue)
        {
            errorMessages.Add("Górna granica nie może być mniejsza od wzorca.");
        }

        if (errorMessages.Count <= 0) return true;

        MessageBox.Show(string.Join("\n", errorMessages));
        return false;

    }

    private void CgTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var cg = firstProcedure.Calculate().Cg;
        if (double.IsPositiveInfinity(cg))
            cg = 4;
        CgProgressBar.Value = cg;
    }

    private void CgkTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var cgk = firstProcedure.Calculate().Cgk;
        if (double.IsPositiveInfinity(cgk))
            cgk = 4;
        CgkProgressBar.Value = cgk;
    }
    #endregion

    #region procedura2
    private void ShowTableButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new Window
        {
            Title = "Pomiary - procedura 2",
            Width = 600,
            Height = 450,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        var mainGrid = new Grid();
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

        var stackPanel = new StackPanel
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


        foreach (var dimension in dimensions)
        {
            var filteredPomiary = pomiary.Where(item => item.Key.Item1.ToString() == dimension).ToList();
            if (filteredPomiary.Count == 0) continue;

            var dataGrid = new DataGrid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                CanUserAddRows = false,
                IsReadOnly = false
            };

            var items = filteredPomiary.Select(item => new DataGridItem
            {
                Key = $"Operator {item.Key.Item1}/Serie {item.Key.Item2}/Wyrób {item.Key.Item3}",
                Value = item.Value == 0 ? " " : item.Value.ToString()
            }).ToList();

            // Usuń istniejące kolumny
            dataGrid.AutoGenerateColumns = false;

            // Dodaj ręcznie kolumny
            var keyColumn = new DataGridTextColumn
            {
                Header = "Operator/Seria/Wyrób",
                Binding = new Binding("Key"),
                IsReadOnly = true
            };
            dataGrid.Columns.Add(keyColumn);

            var valueColumn = new DataGridTextColumn
            {
                Header = "Wartość",
                Binding = new Binding("Value")
            };
            dataGrid.Columns.Add(valueColumn);

            dataGrid.ItemsSource = items;

            stackPanel.Children.Add(dataGrid);
        }

        var saveButton = new Button
        {
            Content = "Zapisz zmiany",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(10, 10, 10, 10)
        };
        saveButton.Click += (sender, e) =>
        {
            foreach (var child in stackPanel.Children)
            {
                if (child is DataGrid dataGrid)
                {
                    foreach (var item in dataGrid.Items)
                    {
                        if (item is DataGridItem dataGridItem)
                        {
                            var keyParts = dataGridItem.Key.Split('/');
                            var key1 = int.Parse(keyParts[0].Split(' ')[1]); // Pobierz numer operatora
                            var key2 = int.Parse(keyParts[1].Split(' ')[1]); // Pobierz numer serii
                            var key3 = int.Parse(keyParts[2].Split(' ')[1]); // Pobierz numer wyrobu
                            if (double.TryParse(dataGridItem.Value, out double value))
                            {
                                pomiary[(key1, key2, key3)] = value;
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
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        scrollViewer.Content = stackPanel;

        Grid.SetColumn(scrollViewer, 0);
        mainGrid.Children.Add(scrollViewer);
        Grid.SetColumn(saveButton, 1);
        mainGrid.Children.Add(saveButton);

        window.Content = mainGrid;
        window.ShowDialog();
    }


    #endregion


    private void ShowPomiaryAlert()
    {
        var message = string.Join("\n", pomiary.Select(item => $"Klucz: {item.Key}, Wartość: {item.Value}"));
        MessageBox.Show(message, "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
    }


    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        int.TryParse(OperatorzyTextBox.Text, out var liczbaOperatorow);
        int.TryParse(SeriaTextBox.Text, out var liczbaSerii);
        int.TryParse(WyrobyTextBox.Text, out var liczbaWyrobow);

        pomiary.Clear();

        for (int i = 1; i <= liczbaOperatorow; i++)
        {
            for (int j = 1; j <= liczbaSerii; j++)
            {
                for (int k = 1; k <= liczbaWyrobow; k++)
                {
                    pomiary[(i, j, k)] = 0;
                }
            }
        }

        ShowPomiaryAlert();
    }

    private void Button_Procedura1_Click(object sender, RoutedEventArgs e)
    {
        tabControl.SelectedIndex = 0; // Przełącz na pierwszą zakładkę
    }

    private void Button_Procedura2_Click(object sender, RoutedEventArgs e)
    {
        tabControl.SelectedIndex = 1; // Przełącz na drugą zakładkę
    }

    private void Button_Procedura3_Click(object sender, RoutedEventArgs e)
    {
        tabControl.SelectedIndex = 2; // Przełącz na trzecią zakładkę
    }
}

