using System;
using System.Collections.Generic;
using System.Linq;

namespace MSAAnalyzer.Classes;

public class SecondProcedure
{
    public Dictionary<int, double> calkowiteSrednieWyrobow = new(); // wyrob                       PART AVERAGE
    public Dictionary<int, double> calkowityRozstepWyrobow = new(); // wyrob                       PART RANGE
    public Dictionary<(int, int), double> srednieWyrobowDlaOperatora = new(); // operator/wyrob     A, B, C ... AVERAGE
    public Dictionary<(int, int), double> rozstepWyrobowDlaOperatora = new(); // operator/wyrob    A, B, C ... RANGE
    public Dictionary<int, double> sredniaOperatora = new(); // Srednia A, B, C ... AVERAGE
    public Dictionary<int, double> rozstepOperatora = new();
    const double k1 = 0.5908;
    const double k2 = 0.5231;
    const double k3 = 0.4030;
    public SecondProcedureResult Calculate(Dictionary<(int, int, int), double> pomiary, int _liczbaWyrobow, int _liczbaOperatorow, int _numerSerii, double _t)
    {
        ClearData();

        #region OBLICZANIE CALKOWITYCH SREDNICH WYROBOW
        for (var k = 1; k <= _liczbaWyrobow; k++)
        {
            var listaPomiarowWyrobu = pomiary
                .Where(p => p.Key.Item3 == k)
                .Select(pomiar => pomiar.Value)
                .ToList();

            var calkowitaSredniaWyrobu = listaPomiarowWyrobu.Sum() / listaPomiarowWyrobu.Count;
            calkowiteSrednieWyrobow.Add(k, calkowitaSredniaWyrobu);

        }
        #endregion

        #region OBLICZANIE SREDNICH WYROBOW DANEGO OPERATORA
        for (var i = 1; i <= _liczbaOperatorow; i++)
        {
            for (var j = 1; j <= _liczbaWyrobow; j++)
            {
                var sumaSerii = new List<double>();
                for (var k = 1; k <= _numerSerii; k++)
                {
                    sumaSerii.Add(pomiary[(i, k, j)]);
                }

                var sredniaSerii = sumaSerii.Sum() / sumaSerii.Count;
                srednieWyrobowDlaOperatora.Add((i, j), sredniaSerii);
            }
        }
        #endregion

        #region OBLICZANIE ROZSTEPU WYROBOW 
        for (var i = 1; i <= _liczbaOperatorow; i++)
        {

            for (var j = 1; j <= _liczbaWyrobow; j++)
            {
                var pomiaryWSerii = new List<double>();
                for (var k = 1; k <= _numerSerii; k++)
                {
                    pomiaryWSerii.Add(pomiary[(i, k, j)]);
                }
                var rozstep = Math.Abs(pomiaryWSerii.Max() - pomiaryWSerii.Min());
                rozstepWyrobowDlaOperatora.Add((i, j), rozstep);
            }

        }
        #endregion

        #region CALKOWITE SREDNIE DLA SERII
        for (var i = 1; i <= _liczbaOperatorow; i++)
        {
            var listaPomiarow = srednieWyrobowDlaOperatora.Where(x => x.Key.Item1 == i);
            var sumaSrednich = new List<double>();
            foreach (var lista in listaPomiarow)
            {
                sumaSrednich.Add(lista.Value);
            }
            sredniaOperatora.Add(i, sumaSrednich.Sum() / sumaSrednich.Count);
        }
        #endregion

        #region CALKOWITE ROZSTEPY DLA SERII
        for (var i = 1; i <= _liczbaOperatorow; i++)
        {
            var listaPomiarow = rozstepWyrobowDlaOperatora.Where(x => x.Key.Item1 == i);
            var sumaRozstepow = new List<double>();
            foreach (var lista in listaPomiarow)
            {
                sumaRozstepow.Add(lista.Value);
            }
            rozstepOperatora.Add(i, sumaRozstepow.Sum() / sumaRozstepow.Count);
        }
        #endregion

        var Rsr = (rozstepOperatora.Values.Sum() / rozstepOperatora.Count);
        var Xdiff = (sredniaOperatora.Values.Max() - sredniaOperatora.Values.Min());
        var Rp = (calkowiteSrednieWyrobow.Values.Max() - calkowiteSrednieWyrobow.Values.Min());
        var EV = k1 * Rsr;
        var AV = k2 * Xdiff;
        var GRR = Math.Sqrt(Math.Pow(EV, 2) + Math.Pow(AV, 2));
        var PV = (Rp * k3);
        var TV = (Math.Sqrt(Math.Pow(GRR, 2) + Math.Pow(PV, 2)));
        var percentEV = 100 * ((Math.Pow(EV, 2) / (GRR * _t)));
        var percentAV = 100 * (Math.Pow(AV, 2) / (GRR * _t));
        var percentGRR = percentEV + percentAV;
        var percentPV = (100 * (PV / TV));


        return new SecondProcedureResult()
        {
            Rp = Rp,
            EV = EV,
            AV = AV,
            GRR = GRR,
            PV = PV,
            TV = TV,
            percentEV = percentEV,
            percentAV = percentAV,
            percentGRR = percentGRR,
            percentPV = percentPV,
            _t = _t,
            Rsr = Rsr,
            Xdiff = Xdiff
        };
    }

    private void ClearData()
    {
        calkowiteSrednieWyrobow.Clear();
        calkowityRozstepWyrobow.Clear();
        srednieWyrobowDlaOperatora.Clear();
        rozstepWyrobowDlaOperatora.Clear();
        sredniaOperatora.Clear();
        rozstepOperatora.Clear();
    }
}

