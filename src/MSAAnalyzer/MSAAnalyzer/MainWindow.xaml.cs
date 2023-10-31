using MSAAnalyzer.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MSAAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<double> listaPomiarow = new();
        private bool fieldsValidated = false;
        private FirstProcedure firstProcedure;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            firstProcedure = new FirstProcedure(listaPomiarow);
            UpdateUIWithResults();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(2);
            KolejnyPomiarTextBox.IsEnabled = false;
        }

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
            Double.TryParse(WartoscWzorcaTextBox.Text, out double wartoscWzorca);
            Double.TryParse(GornaGranicaTextBox.Text, out double gorna);
            Double.TryParse(DolnaGranicaTextBox.Text, out double dolna);
            firstProcedure.SetWartoscWzorca(wartoscWzorca);
            firstProcedure.SetGranice(gorna, dolna);
            ValidateAndEnableKolejnyPomiarTextBox();
            if (fieldsValidated)
            {
                UpdateUIWithResults();
                ZapisanoDaneTextBox.Text = "Dane zostały zapisane";
                timer.Start();
            }
        }

        private void KolejnyPomiarTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
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

            if (!double.TryParse(WartoscWzorcaTextBox.Text, out double wzorzecValue) || wzorzecValue < 0)
            {
                errorMessages.Add("Nieprawidłowa wartość dla wzorca.");
            }

            if (!double.TryParse(GornaGranicaTextBox.Text, out double gornaGranicaValue) || gornaGranicaValue < 0)
            {
                errorMessages.Add("Nieprawidłowa wartość dla górnej granicy.");
            }

            if (!double.TryParse(DolnaGranicaTextBox.Text, out double dolnaGranicaValue) || dolnaGranicaValue < 0)
            {
                errorMessages.Add("Nieprawidłowa wartość dla dolnej granicy.");
            }

            if (!double.TryParse(RozdzielczoscTextBox.Text, out double rozdzielczoscValue) || rozdzielczoscValue < 0)
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

            if (errorMessages.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errorMessages));
                return false;
            }

            return true;
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
    }
}