using MSAAnalyzer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    private FirstProcedure firstProcedure;
    private SecondProcedure secondProcedure;
    private ThirdProcedure thirdProcedure;
    private List<double> listaPomiarow = new();
    private bool fieldsValidated;
    private DispatcherTimer timer;
    public static Dictionary<(int, int, int), double> pomiary = new();
    public Dictionary<(int, int), double> thirdProcedurePomiary = new Dictionary<(int, int), double>
    {
        {(1, 1), 0.0}, {(1, 2), 0.0}, {(1, 3), 0.0}, {(1, 4), 0.0}, {(1, 5), 0.0},
        {(1, 6), 0.0}, {(1, 7), 0.0}, {(1, 8), 0.0}, {(1, 9), 0.0}, {(1, 10), 0.0},
        {(1, 11), 0.0}, {(1, 12), 0.0}, {(1, 13), 0.0}, {(1, 14), 0.0}, {(1, 15), 0.0},
        {(1, 16), 0.0}, {(1, 17), 0.0}, {(1, 18), 0.0}, {(1, 19), 0.0}, {(1, 20), 0.0},
        {(1, 21), 0.0}, {(1, 22), 0.0}, {(1, 23), 0.0}, {(1, 24), 0.0}, {(1, 25), 0.0},
        {(2, 1), 0.0}, {(2, 2), 0.0}, {(2, 3), 0.0}, {(2, 4), 0.0}, {(2, 5), 0.0},
        {(2, 6), 0.0}, {(2, 7), 0.0}, {(2, 8), 0.0}, {(2, 9), 0.0}, {(2, 10), 0.0},
        {(2, 11), 0.0}, {(2, 12), 0.0}, {(2, 13), 0.0}, {(2, 14), 0.0}, {(2, 15), 0.0},
        {(2, 16), 0.0}, {(2, 17), 0.0}, {(2, 18), 0.0}, {(2, 19), 0.0}, {(2, 20), 0.0},
        {(2, 21), 0.0}, {(2, 22), 0.0}, {(2, 23), 0.0}, {(2, 24), 0.0}, {(2, 25), 0.0}
    };


    private static FirstProcedureResult firstProcedureResult;
    private static int LicznikElementow = 0;

    public MainWindow()
    {
        InitializeComponent();
        firstProcedure = new FirstProcedure(listaPomiarow);
        secondProcedure = new SecondProcedure();
        thirdProcedure = new ThirdProcedure();
        UpdateFirstProcedureUIWithResults();
        KolejnyPomiarTextBox.IsEnabled = false;
        firstProcedureResult = firstProcedure.Calculate();
        DataContext = this;
    }

    #region procedura1
    
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

    private void Button_Click(object sender, RoutedEventArgs e) // Zapis procedura 1
    {
        InitializeFirstProcedure();
        UpdateFirstProcedureUIWithResults();
    }

    private void InitializeFirstProcedure()
    {
        listaPomiarow.Clear();
        firstProcedureResult = firstProcedure.Calculate();
        LicznikElementow = 0;

        double.TryParse(WartoscWzorcaTextBox.Text, out var wartoscWzorca);
        double.TryParse(GornaGranicaTextBox.Text, out var gorna);
        double.TryParse(DolnaGranicaTextBox.Text, out var dolna);

        firstProcedure.SetWartoscWzorca(wartoscWzorca);
        firstProcedure.SetGranice(gorna, dolna);

        ValidateAndEnableKolejnyPomiarTextBox();
        if (!fieldsValidated) return;
        MessageBox.Show("Zapisano dane!", "Konfiguracja procedury 1", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void KolejnyPomiarTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        var pomiar = double.TryParse(KolejnyPomiarTextBox.Text, out var pomiarValue);
        if (!pomiar || pomiarValue <= 0)
        {
            MessageBox.Show("Nieprawidłowa wartość dla pomiaru.");
        }
        else
        {

            listaPomiarow.Add(pomiarValue);

            KolejnyPomiarTextBox.Clear();
            firstProcedure.UpdateData(listaPomiarow);
            LicznikElementow++;
            ValidateTextBoxes();
            UpdateFirstProcedureUIWithResults();
        }
    }

    private void UpdateFirstProcedureUIWithResults()
    {
        firstProcedureResult = firstProcedure.Calculate();

        TTextBox.Text = firstProcedureResult.T.ToString();

        LiczbaPomiarowTextBox.Text = LicznikElementow.ToString();
        
        if (LicznikElementow > 3)
        {
            ZdolnoscSystemuTextBox.Text =
                firstProcedureResult is { Cg: > 1.33, Cgk: > 1.33 } ? "SYSTEM JEST ZDOLNY" : "SYSTEM NIE JEST ZDOLNY";
            ZdolnoscSystemuTextBox.Foreground = firstProcedureResult is { Cg: > 1.33, Cgk: > 1.33 }
                ? new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAF, 0x83))
                : new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x3D, 0x3D));

            OdchylenieTextBox.Text = firstProcedureResult.Sigma.ToString();
            SredniaTextBox.Text = firstProcedureResult.Mean.ToString();
            CgTextBox.Text = firstProcedureResult.Cg.ToString();
            CgkTextBox.Text = firstProcedureResult.Cgk.ToString();
            CgProgressBar.Value = double.IsPositiveInfinity(firstProcedureResult.Cg) || double.IsNaN(firstProcedureResult.Cg) ? 4 : firstProcedureResult.Cg;
            CgkProgressBar.Value = double.IsPositiveInfinity(firstProcedureResult.Cgk) || double.IsNaN(firstProcedureResult.Cgk) ? 4 : firstProcedureResult.Cgk;

        }
        else
        {
            ZdolnoscSystemuTextBox.Text = string.Empty;
            OdchylenieTextBox.Text = string.Empty;
            SredniaTextBox.Text = string.Empty;
            CgTextBox.Text = string.Empty;
            CgkTextBox.Text = string.Empty;
            CgProgressBar.Value = 0;
            CgkProgressBar.Value = 0;
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

    #endregion

    #region procedura2
    private void ShowProcedure2TablesButton_Click(object sender, RoutedEventArgs e)
    {
        if (!pomiary.Any())
        {
            MessageBox.Show("Aby wprowadzić pomiary podaj liczbę operatorów, serii oraz wyrobów.", "Błąd",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

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

            var items = filteredPomiary.Select(item => new SecondProcedureDataGridItem()
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
                        if (item is SecondProcedureDataGridItem dataGridItem)
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
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = stackPanel
        };

        Grid.SetColumn(scrollViewer, 0);
        mainGrid.Children.Add(scrollViewer);
        Grid.SetColumn(saveButton, 1);
        mainGrid.Children.Add(saveButton);

        window.Content = mainGrid;
        window.ShowDialog();
    }
    private void SaveProcedure2ConfigClick(object sender, RoutedEventArgs e)
    {

        if (!int.TryParse(OperatorzyTextBox.Text, out var liczbaOperatorow) || liczbaOperatorow < 0)
        {
            MessageBox.Show(
                "Nieprawidłowa wartość dla liczby operatorów. Upewnij się, że wartość jest nieujemną liczbą całkowitą.",
                "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(SeriaTextBox.Text, out var liczbaSerii) || liczbaSerii < 0)
        {
            MessageBox.Show(
                "Nieprawidłowa wartość dla liczby serii. Upewnij się, że wartość jest nieujemną liczbą całkowitą.",
                "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(WyrobyTextBox.Text, out var liczbaWyrobow) || liczbaWyrobow < 0)
        {
            MessageBox.Show(
                "Nieprawidłowa wartość dla liczby wyrobów. Upewnij się, że wartość jest nieujemną liczbą całkowitą.",
                "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        pomiary.Clear();


        for (var i = 1; i <= liczbaOperatorow; i++)
        {
            for (var j = 1; j <= liczbaSerii; j++)
            {
                for (var k = 1; k <= liczbaWyrobow; k++)
                {
                    pomiary[(i, j, k)] = 0;
                }
            }
        }
        MessageBox.Show("Zapisano ustawienia!", "Ustawienia", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    private void CalculateProcedure2Button_Click(object sender, RoutedEventArgs e)
    {
        int.TryParse(WyrobyTextBox.Text, out var  liczbaWyrobow);
        int.TryParse(OperatorzyTextBox.Text, out var liczbaOperatorow);
        int.TryParse(SeriaTextBox.Text, out var numerSerii);
        double.TryParse(TTextBox.Text, out var t);

        if (isSecondProcedureDataReadyToCalculate())
        {
            var secondProcedureResult = secondProcedure.Calculate(pomiary, liczbaWyrobow, liczbaOperatorow, numerSerii, t);
            UpdateSecondProcedureUIWithResults(secondProcedureResult);
        }

        else
        {
            UpdateSecondProcedureUIWithResults(null);
            MessageBox.Show("Wprowadzone pomiary nie są kompletne!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

    }
    private bool isSecondProcedureDataReadyToCalculate()
    {
        return !(pomiary.Any(x => x.Value == 0) || pomiary.Count == 0);
    }
    private void UpdateSecondProcedureUIWithResults(SecondProcedureResult? result)
    {
        RsrTextBox.Text = result is not null ? result.Rsr.ToString() : "";
        XdiffTextBox.Text = result is not null ? result.Xdiff.ToString() : "";
        RpTextBox.Text = result is not null ? result.Rp.ToString() : "";
        EVTextBox.Text = result is not null ? result.EV.ToString() : "";
        AVTextBox.Text = result is not null ? result.AV.ToString() : "";
        GRRTextBox.Text = result is not null ? result.GRR.ToString() : "";
        PVTextBox.Text = result is not null ? result.PV.ToString() : "";
        TVTextBox.Text = result is not null ? result.TV.ToString() : "";
        PercentEVTextBox.Text = result is not null ? result.percentEV.ToString() : "";
        PercentAVTextBox.Text = result is not null ? result.percentAV.ToString() : "";
        PercentGRRTextBox.Text = result is not null ? result.percentGRR.ToString() : "";
        PercentPVTextBox.Text = result is not null ? result.percentPV.ToString() : "";
    }
    #endregion

    #region procedura3

    private void ShowProcedure3TablesButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new Window
        {
            Title = "Pomiary - procedura 3",
            Width = 1000,
            Height = 750,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        var thirdProcedureTableManager = new Procedure3TableManager(thirdProcedurePomiary, window);

        var serie = new List<string>();
        foreach (var pomiar in thirdProcedurePomiary.Where(pomiar => !serie.Contains(pomiar.Key.Item1.ToString())))
        {
            serie.Add(pomiar.Key.Item1.ToString());
        }

        thirdProcedureTableManager.CreateProcedure3Tables(serie);
    }
    
    private void ObliczProcedure3Button_Click(object sender, RoutedEventArgs e)
    {
        double.TryParse(TTextBox.Text, out var t);

        if (isThirdProcedureDataReadyToCalculate())
        {
            var thirdProcedureResult = thirdProcedure.Calculate(thirdProcedurePomiary, t);
            UpdateThirdProcedureUIWithResults(thirdProcedureResult);
        }
        else
        {
            MessageBox.Show("Wprowadzone pomiary nie są kompletne!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void UpdateThirdProcedureUIWithResults(ThirdProcedureResult result)
    {
        sigmaProcedure3TextBox.Text = result.rozrzutSystemu.ToString();
        EVProcedure3TextBox.Text = result.EV.ToString();
        PercentEVProcedure3TextBox.Text = result.percentEV.ToString();
        Procedure3PercentEVProgressBar.Value = double.Parse(PercentEVProcedure3TextBox.Text);
    }
    private bool isThirdProcedureDataReadyToCalculate()
    {
        return !(thirdProcedurePomiary.Any(x => x.Value == 0) || thirdProcedurePomiary.Count == 0);
    }
   
    private DataGrid CreateThirdProcedureDataGrid(List<ThirdProcedureDataGridItem> items)
    {
        var dataGrid = new DataGrid
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            CanUserAddRows = false,
            IsReadOnly = false,
            Margin = new Thickness(10, 10, 10, 10),
            AutoGenerateColumns = false
        };

        var seriaColumn = new DataGridTextColumn
        {
            Header = "Seria",
            Binding = new Binding("SeriaKey"),
            IsReadOnly = true
        };

        var wyrobColumn = new DataGridTextColumn
        {
            Header = "Wyrób",
            Binding = new Binding("WyrobKey"),
            IsReadOnly = true
        };

        var valueColumn = new DataGridTextColumn
        {
            Header = "Wartość",
            Binding = new Binding("Value")
        };

        dataGrid.Columns.Add(seriaColumn);
        dataGrid.Columns.Add(wyrobColumn);
        dataGrid.Columns.Add(valueColumn);

        dataGrid.ItemsSource = items;

        return dataGrid;
    }


    #endregion
}

