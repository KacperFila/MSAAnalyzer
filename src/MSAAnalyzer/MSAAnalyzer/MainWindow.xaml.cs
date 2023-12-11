using MSAAnalyzer.Classes;
using MSAAnalyzer.DataContext;
using MSAAnalyzer.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using WebSocket4Net;

namespace MSAAnalyzer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly AppDataContext _appDataContext;
    public FirstProcedure FirstProcedure;
    public SecondProcedure SecondProcedure;
    public ThirdProcedure ThirdProcedure;
    private bool _fieldsValidated;
    private bool _tCalculated;
    private static FirstProcedureResult? _firstProcedureResult;
    private static int _licznikElementow;

    private WebSocket websocket;
    public MainWindow()
    {
        InitializeComponent();
        InitializeWebSocket();
        _appDataContext = (AppDataContext)DataContext;
        FirstProcedure = new FirstProcedure(
            _appDataContext.FirstProcedureMeasurements,
            _appDataContext.K);
        SecondProcedure = new SecondProcedure();
        ThirdProcedure = new ThirdProcedure();
        KolejnyPomiarTextBox.IsEnabled = false;
        menuTabControl.SelectionChanged += TabControl_SelectionChanged;
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (menuTabControl.SelectedItem is TabItem selectedTab && selectedTab.Header.ToString() == "Wartości współczynników")
        {
            KfactorTextBox.Text = _appDataContext.K.ToString("0.##");
            TfactorTextBox.Text = _appDataContext.T.ToString("0.##");
            K1factorTextBox.Text = _appDataContext.K1.ToString("0.##");
            K2factorTextBox.Text = _appDataContext.K2.ToString("0.##");
            K3factorTextBox.Text = _appDataContext.K3.ToString("0.##");
        }
    }

    #region procedura1

    private void ValidateAndEnableKolejnyPomiarTextBox()
    {
        if (ValidateTextBoxes())
        {
            _fieldsValidated = true;
            KolejnyPomiarTextBox.IsEnabled = true;
            undoButton.IsEnabled = true;
        }
        else
        {
            _fieldsValidated = false;
            KolejnyPomiarTextBox.IsEnabled = false;
        }
    }

    private void SaveProcedure1Button_Click(object sender, RoutedEventArgs e) // Zapis procedura 1
    {
        InitializeFirstProcedure();
    }

    private void InitializeFirstProcedure()
    {
        _appDataContext.FirstProcedureMeasurements.Clear();
        
        double.TryParse(WartoscWzorcaTextBox.Text, out var wartoscWzorca);
        double.TryParse(GornaGranicaTextBox.Text, out var gorna);
        double.TryParse(DolnaGranicaTextBox.Text, out var dolna);

        _licznikElementow = 0;

        FirstProcedure.SetWartoscWzorca(wartoscWzorca);
        FirstProcedure.SetGranice(gorna, dolna);

        ValidateAndEnableKolejnyPomiarTextBox();
        if (!_fieldsValidated) return;

        _firstProcedureResult = FirstProcedure.Calculate();

        _appDataContext.T = _firstProcedureResult.T;
        _tCalculated = true;

        UpdateFirstProcedureUIWithResults();
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

            _appDataContext.FirstProcedureMeasurements.Add(pomiarValue);

            KolejnyPomiarTextBox.Clear();
            FirstProcedure.UpdateData(_appDataContext.FirstProcedureMeasurements);
            _licznikElementow++;
            ValidateTextBoxes();
            UpdateFirstProcedureUIWithResults();
        }
    }

    private void UpdateFirstProcedureUIWithResults()
    {
        _firstProcedureResult = FirstProcedure.Calculate();

        TTextBox.Text = Math.Round(_firstProcedureResult.T, 3).ToString();

        LiczbaPomiarowTextBox.Text = (_licznikElementow.ToString() != "0") ? _licznikElementow.ToString() : "-";
        OstatniPomiarTextBox.Text = (_appDataContext.FirstProcedureMeasurements.Any()) ? _appDataContext.FirstProcedureMeasurements.Last().ToString() : "-";

        if (double.TryParse(RozdzielczoscTextBox.Text, out double rozdzielczosc))
        {
            if (rozdzielczosc <= _appDataContext.T * 0.05)
            {
                ZdolnoscRozdzielczoscTextBox.Text = "WYSTARCZAJĄCA";
                ZdolnoscRozdzielczoscTextBox.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAF, 0x83));
            }
            else
            {
                ZdolnoscRozdzielczoscTextBox.Text = "NIEWYSTARCZAJĄCA";
                ZdolnoscRozdzielczoscTextBox.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x3D, 0x3D));
            }
        }

        if (_licznikElementow > 3)
        {
            ZdolnoscSystemuTextBox.Text =
                _firstProcedureResult is { Cg: > 1.33, Cgk: > 1.33 } ? "SYSTEM JEST ZDOLNY" : "SYSTEM NIE JEST ZDOLNY";
            ZdolnoscSystemuTextBox.Foreground = _firstProcedureResult is { Cg: > 1.33, Cgk: > 1.33 }
                ? new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAF, 0x83))
                : new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x3D, 0x3D));

            OdchylenieTextBox.Text = Math.Round(_firstProcedureResult.Sigma, 6).ToString();
            SredniaTextBox.Text = Math.Round(_firstProcedureResult.Mean, 6).ToString();
            CgTextBox.Text = Math.Round(_firstProcedureResult.Cg, 2).ToString();
            CgkTextBox.Text = Math.Round(_firstProcedureResult.Cgk, 2).ToString();
            CgProgressBar.Value = double.IsInfinity(_firstProcedureResult.Cg) || double.IsNaN(_firstProcedureResult.Cg) ? 4 : _firstProcedureResult.Cg;
            CgkProgressBar.Value = double.IsInfinity(_firstProcedureResult.Cgk) || double.IsNaN(_firstProcedureResult.Cgk) ? 4 : _firstProcedureResult.Cgk;
            
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

        MessageBox.Show(string.Join("\n", errorMessages), "Niepoprawne wartości", MessageBoxButton.OK, MessageBoxImage.Information);
        return false;

    }

    private void OpenProcedure1SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new Procedure1SettingsWindow();
        settingsWindow.ShowDialog();
        _appDataContext.K = settingsWindow.KValue;
    }

    #endregion

    #region procedura2
    private void ShowProcedure2TablesButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_appDataContext.SecondProcedureMeasurements.Any())
        {
            MessageBox.Show("Aby wprowadzić pomiary podaj liczbę operatorów, serii oraz wyrobów.", "Błąd",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var window = new Procedure2DataGridWindow();
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

        if (!IsOperatorsSeriesElementsQuantityCorrect(liczbaWyrobow, liczbaSerii, liczbaOperatorow))
        {
            MessageBox.Show("Ilość operatorów, serii lub wyrobów przekracza dopuszczalną wartość!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _appDataContext.SecondProcedureMeasurements.Clear();


        for (var i = 1; i <= liczbaOperatorow; i++)
        {
            for (var j = 1; j <= liczbaSerii; j++)
            {
                for (var k = 1; k <= liczbaWyrobow; k++)
                {
                    _appDataContext.SecondProcedureMeasurements[(i, j, k)] = 0;
                }
            }
        }
        MessageBox.Show("Zapisano ustawienia!", "Ustawienia", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    private void CalculateProcedure2Button_Click(object sender, RoutedEventArgs e)
    {
        int.TryParse(WyrobyTextBox.Text, out var liczbaWyrobow);
        int.TryParse(OperatorzyTextBox.Text, out var liczbaOperatorow);
        int.TryParse(SeriaTextBox.Text, out var numerSerii);

        if (IsSecondProcedureDataReadyToCalculate())
        {
            var secondProcedureResult = SecondProcedure
                .Calculate(
                _appDataContext.SecondProcedureMeasurements,
                liczbaWyrobow,
                liczbaOperatorow,
                numerSerii,
                _appDataContext.T,
                _appDataContext.K1,
                _appDataContext.K2,
                _appDataContext.K3);

            UpdateSecondProcedureUIWithResults(secondProcedureResult);
        }

        else
        {
            UpdateSecondProcedureUIWithResults(null);
            MessageBox.Show("Wprowadzone dane nie są kompletne lub nie obliczono tolerancji!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
    }
    private bool IsSecondProcedureDataReadyToCalculate()
    {
        return !(_appDataContext.SecondProcedureMeasurements.Any(x => x.Value == 0) ||
                 _appDataContext.SecondProcedureMeasurements.Count == 0 ||
                 !_tCalculated);
    }
    private bool IsOperatorsSeriesElementsQuantityCorrect(int liczbaWyrobow, int liczbaSerii, int liczbaOperatorow)
    {
        return (liczbaOperatorow < 5 && liczbaSerii < 5 && liczbaWyrobow < 20);
    }
    private void UpdateSecondProcedureUIWithResults(SecondProcedureResult? result)
    {
        RsrTextBox.Text = (result != null && !double.IsNaN(result.Rsr)) ? result.Rsr.ToString("0.#####") : "0";
        XdiffTextBox.Text = (result != null && !double.IsNaN(result.Xdiff)) ? result.Xdiff.ToString("0.#####") : "0";
        RpTextBox.Text = (result != null && !double.IsNaN(result.Rp)) ? result.Rp.ToString("0.#####") : "0";
        EVTextBox.Text = (result != null && !double.IsNaN(result.EV)) ? result.EV.ToString("0.#####") : "0";
        AVTextBox.Text = (result != null && !double.IsNaN(result.AV)) ? result.AV.ToString("0.#####") : "0";
        GRRTextBox.Text = (result != null && !double.IsNaN(result.GRR)) ? result.GRR.ToString("0.#####") : "0";
        PVTextBox.Text = (result != null && !double.IsNaN(result.PV)) ? result.PV.ToString("0.#####") : "0";
        TVTextBox.Text = (result != null && !double.IsNaN(result.TV)) ? result.TV.ToString("0.#####") : "0";
        PercentEVTextBox.Text = (result != null && !double.IsNaN(result.percentEV)) ? result.percentEV.ToString("0.##") : "0";
        PercentAVTextBox.Text = (result != null && !double.IsNaN(result.percentAV)) ? result.percentAV.ToString("0.##") : "0";
        PercentGRRTextBox.Text = (result != null && !double.IsNaN(result.percentGRR)) ? result.percentGRR.ToString("0.##") : "0";
        PercentPVTextBox.Text = (result != null && !double.IsNaN(result.percentPV)) ? result.percentPV.ToString("0.##") : "0";
        GRRProgressBar.Value = (result != null && !double.IsNaN(result.percentGRR)) ? result.percentGRR : 0;
        ZdolnoscGRRTextBox.Text = (result != null && !double.IsNaN(result.percentGRR))
            ? result.percentGRR < 20 ? "SYSTEM JEST ZDOLNY" :
            result.percentGRR >= 20 && result.percentGRR <= 30 ? "SYSTEM ZDOLNY WARUNKOWO" :
            "SYSTEM NIE JEST ZDOLNY"
            : "";


        ZdolnoscGRRTextBox.Foreground = result is not null && !double.IsNaN(result.percentGRR)
            ? result.percentGRR < 20
                ? new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAF, 0x83))
                : result.percentGRR >= 20 && result.percentGRR <= 30
                    ? new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00))
                    : new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x3D, 0x3D))
            : new SolidColorBrush(Colors.Black);

    }
    private void OpenProcedure2SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new Procedure2SettingsWindow();
        settingsWindow.ShowDialog();
        _appDataContext.K1 = settingsWindow.K1Value;
        _appDataContext.K2 = settingsWindow.K2Value;
    }
    #endregion

    #region procedura3

    private void ShowProcedure3TablesButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new Procedure3DataGridWindow();
        window.ShowDialog();
    }
    
    private void ObliczProcedure3Button_Click(object sender, RoutedEventArgs e)
    {
        if (isThirdProcedureDataReadyToCalculate())
        {
            var thirdProcedureResult = ThirdProcedure.Calculate(_appDataContext.ThirdProcedureMeasurements, _appDataContext.T, _appDataContext.K3);
            UpdateThirdProcedureUIWithResults(thirdProcedureResult);
        }
        else
        {
            MessageBox.Show("Wprowadzone pomiary nie są kompletne lub nie obliczono tolerancji T!", "Pomiary", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private void UpdateThirdProcedureUIWithResults(ThirdProcedureResult result)
    {
        rangeAverageProcedure3TextBox.Text = result.rozstepSredni.ToString("0.#####");
        EVProcedure3TextBox.Text = result.EV.ToString("0.#####");
        result.percentEV = Math.Round(result.percentEV, 2);
        PercentEVProcedure3TextBox.Text = result.percentEV.ToString("0.##");
        Procedure3PercentEVProgressBar.Value = result.percentEV;

        ZdolnoscEVTextBox.Text = result is not null && !double.IsNaN(result.percentEV)
            ? result.percentEV < 20 ? "SYSTEM JEST ZDOLNY" :
            result.percentEV is >= 20 and <= 30 ? "SYSTEM ZDOLNY WARUNKOWO" :
            "SYSTEM NIE JEST ZDOLNY"
            : "";

        ZdolnoscEVTextBox.Foreground = result is not null && !double.IsNaN(result.percentEV)
            ? result.percentEV < 20
                ? new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAF, 0x83))
                : result.percentEV >= 20 && result.percentEV<= 30
                    ? new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00))
                    : new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x3D, 0x3D))
            : new SolidColorBrush(Colors.Black);
    }
    private bool isThirdProcedureDataReadyToCalculate()
    {
        return !(_appDataContext.ThirdProcedureMeasurements.Any(x => x.Value == 0)
                 || _appDataContext.ThirdProcedureMeasurements.Count == 0 ||
                 !_tCalculated);
    }

    #endregion

    #region temp
    private void Undo_pomiar(object sender, RoutedEventArgs e)
    {
        if (_appDataContext.FirstProcedureMeasurements.Count > 0)
        {
            _appDataContext.FirstProcedureMeasurements.RemoveAt(_appDataContext.FirstProcedureMeasurements.Count - 1);
            _licznikElementow--;
            UpdateFirstProcedureUIWithResults();
        }
        else
        {
            MessageBox.Show("Ilość pomiarów wynosi 0!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void OpenProcedure3SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new Procedure3SettingsWindow();
        settingsWindow.ShowDialog();
        _appDataContext.K3 = settingsWindow.K3Value;
    }
    #endregion

    private void InitializeWebSocket()
    {
        websocket = new WebSocket("ws://localhost:6123");

        websocket.Opened += (sender, e) =>
        {
            MessageBox.Show("WebSocket connected!");
        };

        websocket.MessageReceived += (sender, e) =>
        {
            // Odebrano wiadomość
            Debug.WriteLine(e.Message);

            // Podziel wiadomość na fragmenty przy użyciu znaku nowej linii
            string[] lines = e.Message.Split('\n');

            if (lines.Length > 0)
            {
                // Weź ostatni fragment
                string lastLine = lines[lines.Length - 1];

                // Usuń wszystkie niecyfrowe znaki z końca fragmentu
                string cleanedLastLine = new string(lastLine.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());

                if (double.TryParse(cleanedLastLine, out var toAdd))
                {
                    _appDataContext.FirstProcedureMeasurements.Add(toAdd);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FirstProcedure.UpdateData(_appDataContext.FirstProcedureMeasurements);
                        _licznikElementow++;
                        UpdateFirstProcedureUIWithResults();
                    });
                }
                else
                {
                    MessageBox.Show("Nieprawidłowa wartość dla pomiaru.");
                }
            }
            else
            {
                MessageBox.Show("Nie znaleziono liczby w wiadomości.");
            }
        };


        websocket.Closed += (sender, e) =>
        {
            MessageBox.Show("WebSocket closed!");
        };

        websocket.Open();
    }

}

