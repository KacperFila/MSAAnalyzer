using MSAAnalyzer.Classes;
using MSAAnalyzer.DataContext;
using MSAAnalyzer.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MSAAnalyzer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private AppDataContext appDataContext;

    public FirstProcedure firstProcedure;
    public SecondProcedure secondProcedure;
    public ThirdProcedure thirdProcedure;

    
    private bool fieldsValidated;
    private bool _tCalculated;
    
    private static FirstProcedureResult firstProcedureResult;
    private static int LicznikElementow;

    public MainWindow()
    {
        InitializeComponent();
        appDataContext = (AppDataContext)DataContext;

        firstProcedure = new FirstProcedure(appDataContext.FirstProcedureMeasurements, appDataContext.K);
        secondProcedure = new SecondProcedure();
        thirdProcedure = new ThirdProcedure();
        KolejnyPomiarTextBox.IsEnabled = false;
        menuTabControl.SelectionChanged += TabControl_SelectionChanged;
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (menuTabControl.SelectedItem is TabItem selectedTab && selectedTab.Header.ToString() == "Wartości współczynników")
        {
            KfactorTextBox.Text = appDataContext.K.ToString();
            TfactorTextBox.Text = appDataContext.T.ToString();
            K1factorTextBox.Text = appDataContext.K1.ToString();
            K2factorTextBox.Text = appDataContext.K2.ToString();
            K3factorTextBox.Text = appDataContext.K3.ToString();
        }
    }

    #region procedura1

    private void ValidateAndEnableKolejnyPomiarTextBox()
    {
        if (ValidateTextBoxes())
        {
            fieldsValidated = true;
            KolejnyPomiarTextBox.IsEnabled = true;
            undoButton.IsEnabled = true;
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
    }

    private void InitializeFirstProcedure()
    {
        appDataContext.FirstProcedureMeasurements.Clear();
        
        double.TryParse(WartoscWzorcaTextBox.Text, out var wartoscWzorca);
        double.TryParse(GornaGranicaTextBox.Text, out var gorna);
        double.TryParse(DolnaGranicaTextBox.Text, out var dolna);

        LicznikElementow = 0;

        firstProcedure.SetWartoscWzorca(wartoscWzorca);
        firstProcedure.SetGranice(gorna, dolna);

        ValidateAndEnableKolejnyPomiarTextBox();
        if (!fieldsValidated) return;

        firstProcedureResult = firstProcedure.Calculate();

        appDataContext.T = firstProcedureResult.T;
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

            appDataContext.FirstProcedureMeasurements.Add(pomiarValue);

            KolejnyPomiarTextBox.Clear();
            firstProcedure.UpdateData(appDataContext.FirstProcedureMeasurements);
            LicznikElementow++;
            ValidateTextBoxes();
            UpdateFirstProcedureUIWithResults();
        }
    }

    private void UpdateFirstProcedureUIWithResults()
    {
        firstProcedureResult = firstProcedure.Calculate();

        TTextBox.Text = firstProcedureResult.T.ToString();

        LiczbaPomiarowTextBox.Text = (LicznikElementow.ToString() != "0") ? LicznikElementow.ToString() : "-";
        OstatniPomiarTextBox.Text = (appDataContext.FirstProcedureMeasurements.Any()) ? appDataContext.FirstProcedureMeasurements.Last().ToString() : "-";
        
        if (LicznikElementow > 3)
        {
            ZdolnoscSystemuTextBox.Text =
                firstProcedureResult is { Cg: > 1.33, Cgk: > 1.33 } ? "SYSTEM JEST ZDOLNY" : "SYSTEM NIE JEST ZDOLNY";
            ZdolnoscSystemuTextBox.Foreground = firstProcedureResult is { Cg: > 1.33, Cgk: > 1.33 }
                ? new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAF, 0x83))
                : new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x3D, 0x3D));

            OdchylenieTextBox.Text = firstProcedureResult.Sigma.ToString();
            SredniaTextBox.Text = firstProcedureResult.Mean.ToString();
            CgTextBox.Text =  firstProcedureResult.Cg.ToString();
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

        MessageBox.Show(string.Join("\n", errorMessages), "Niepoprawne wartości", MessageBoxButton.OK, MessageBoxImage.Information);
        return false;

    }

    private void OpenProcedure1SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new Procedure1SettingsWindow();
        settingsWindow.ShowDialog();
        appDataContext.K = settingsWindow.KValue;
    }

    #endregion

    #region procedura2

    private void ShowProcedure2TablesButton_Click(object sender, RoutedEventArgs e)
    {
        if (!appDataContext.SecondProcedureMeasurements.Any())
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

        if (!isOperatorsSeriesElementsQuantityCorrect(liczbaWyrobow, liczbaSerii, liczbaOperatorow))
        {
            MessageBox.Show("Ilość operatorów, serii lub wyrobów przekracza dopuszczalną wartość!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        appDataContext.SecondProcedureMeasurements.Clear();


        for (var i = 1; i <= liczbaOperatorow; i++)
        {
            for (var j = 1; j <= liczbaSerii; j++)
            {
                for (var k = 1; k <= liczbaWyrobow; k++)
                {
                    appDataContext.SecondProcedureMeasurements[(i, j, k)] = 0;
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

        if (isSecondProcedureDataReadyToCalculate())
        {
            var secondProcedureResult = secondProcedure
                .Calculate(
                appDataContext.SecondProcedureMeasurements,
                liczbaWyrobow,
                liczbaOperatorow,
                numerSerii,
                appDataContext.T,
                appDataContext.K1,
                appDataContext.K2,
                appDataContext.K3);

            UpdateSecondProcedureUIWithResults(secondProcedureResult);
        }

        else
        {
            UpdateSecondProcedureUIWithResults(null);
            MessageBox.Show("Wprowadzone dane nie są kompletne lub nie obliczono tolerancji!", "Pomiary", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
    }

    private bool isSecondProcedureDataReadyToCalculate()
    {
        return !(appDataContext.SecondProcedureMeasurements.Any(x => x.Value == 0) ||
                 appDataContext.SecondProcedureMeasurements.Count == 0 ||
                 !_tCalculated);
    }

    private bool isOperatorsSeriesElementsQuantityCorrect(int liczbaWyrobow, int liczbaSerii, int liczbaOperatorow)
    {
        return (liczbaOperatorow < 5 && liczbaSerii < 5 && liczbaWyrobow < 20);
    }

    private void UpdateSecondProcedureUIWithResults(SecondProcedureResult? result)
    {
        RsrTextBox.Text = result is not null && !double.IsNaN(result.Rsr) ? result.Rsr.ToString() : "0";
        XdiffTextBox.Text = result is not null && !double.IsNaN(result.Xdiff) ? result.Xdiff.ToString() : "0";
        RpTextBox.Text = result is not null && !double.IsNaN(result.Rp) ? result.Rp.ToString() : "0";
        EVTextBox.Text = result is not null && !double.IsNaN(result.EV) ? result.EV.ToString() : "0";
        AVTextBox.Text = result is not null && !double.IsNaN(result.AV) ? result.AV.ToString() : "0";
        GRRTextBox.Text = result is not null && !double.IsNaN(result.GRR) ? result.GRR.ToString() : "0";
        PVTextBox.Text = result is not null && !double.IsNaN(result.PV) ? result.PV.ToString() : "0";
        TVTextBox.Text = result is not null && !double.IsNaN(result.TV) ? result.TV.ToString() : "0";
        PercentEVTextBox.Text = result is not null && !double.IsNaN(result.percentEV) ? result.percentEV.ToString() : "0";
        PercentAVTextBox.Text = result is not null && !double.IsNaN(result.percentAV) ? result.percentAV.ToString() : "0";
        PercentGRRTextBox.Text = result is not null && !double.IsNaN(result.percentGRR) ? result.percentGRR.ToString() : "0";
        PercentPVTextBox.Text = result is not null && !double.IsNaN(result.percentPV) ? result.percentPV.ToString() : "0";
        GRRProgressBar.Value = (result != null && !double.IsNaN(result.percentGRR)) ? result.percentGRR : 0;
        ZdolnoscGRRTextBox.Text = result is not null && !double.IsNaN(result.percentGRR)
            ? result.percentGRR < 20 ? "SYSTEM JEST ZDOLNY" :
            result.percentGRR is >= 20 and <= 30 ? "SYSTEM ZDOLNY WARUNKOWO" :
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
        appDataContext.K1 = settingsWindow.K1Value;
        appDataContext.K2 = settingsWindow.K2Value;
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
            var thirdProcedureResult = thirdProcedure.Calculate(appDataContext.ThirdProcedureMeasurements, appDataContext.T, appDataContext.K3);
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
        rangeAverageProcedure3TextBox.Text = result.rozstepSredni.ToString();
        EVProcedure3TextBox.Text = result.EV.ToString();
        PercentEVProcedure3TextBox.Text = result.percentEV.ToString();
        Procedure3PercentEVProgressBar.Value = double.Parse(PercentEVProcedure3TextBox.Text);

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
        return !(appDataContext.ThirdProcedureMeasurements.Any(x => x.Value == 0)
                 || appDataContext.ThirdProcedureMeasurements.Count == 0 ||
                 !_tCalculated);
    }

    #endregion

    private void Undo_pomiar(object sender, RoutedEventArgs e)
    {
        if (appDataContext.FirstProcedureMeasurements.Count > 0)
        {
            appDataContext.FirstProcedureMeasurements.RemoveAt(appDataContext.FirstProcedureMeasurements.Count - 1);
            LicznikElementow--;
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
        appDataContext.K3 = settingsWindow.K3Value;
    }
}

