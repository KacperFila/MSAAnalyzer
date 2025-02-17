using System;
using System.Collections.Generic;
using System.Linq;

namespace MSAAnalyzer.Classes;

public class SecondProcedure
{
    public Dictionary<int, double>
        CalkowiteSrednieWyrobow = new();
    public Dictionary<int, double>
        CalkowityRozstepWyrobow = new();
    public Dictionary<(int, int), double>
        SrednieWyrobowDlaOperatora = new();
    public Dictionary<(int, int), double>
        RozstepWyrobowDlaOperatora = new();
    public Dictionary<int, double>
        SredniaOperatora = new();
    public Dictionary<int, double>
        RozstepOperatora = new();

    public SecondProcedureResult Calculate(
        Dictionary<(int, int, int), double> pomiary,
        int liczbaWyrobow, int liczbaOperatorow, int numerSerii,
        double t, double k1, double k2, double k3)
    {
        ClearData();

        #region OBLICZANIE CALKOWITYCH SREDNICH WYROBOW
        for (var k = 1; k <= liczbaWyrobow; k++)
        {
            var listaPomiarowWyrobu = pomiary
                .Where(p => p.Key.Item3 == k)
                .Select(pomiar => pomiar.Value)
                .ToList();

            var calkowitaSredniaWyrobu = listaPomiarowWyrobu.Sum() / listaPomiarowWyrobu.Count;
            CalkowiteSrednieWyrobow.Add(k, calkowitaSredniaWyrobu);

        }
        #endregion

        #region OBLICZANIE SREDNICH WYROBOW DANEGO OPERATORA
        for (var i = 1; i <= liczbaOperatorow; i++)
        {
            for (var j = 1; j <= liczbaWyrobow; j++)
            {
                var sumaSerii = new List<double>();
                for (var k = 1; k <= numerSerii; k++)
                {
                    sumaSerii.Add(pomiary[(i, k, j)]);
                }

                var sredniaSerii = sumaSerii.Sum() / sumaSerii.Count;
                SrednieWyrobowDlaOperatora.Add((i, j), sredniaSerii);
            }
        }
        #endregion

        #region OBLICZANIE ROZSTEPU WYROBOW 
        for (var i = 1; i <= liczbaOperatorow; i++)
        {

            for (var j = 1; j <= liczbaWyrobow; j++)
            {
                var pomiaryWSerii = new List<double>();
                for (var k = 1; k <= numerSerii; k++)
                {
                    pomiaryWSerii.Add(pomiary[(i, k, j)]);
                }
                var rozstep = Math.Abs(pomiaryWSerii.Max() - pomiaryWSerii.Min());
                RozstepWyrobowDlaOperatora.Add((i, j), rozstep);
            }

        }
        #endregion

        #region CALKOWITE SREDNIE DLA SERII
        for (var i = 1; i <= liczbaOperatorow; i++)
        {
            var listaPomiarow = SrednieWyrobowDlaOperatora.Where(x => x.Key.Item1 == i);
            var sumaSrednich = new List<double>();
            foreach (var lista in listaPomiarow)
            {
                sumaSrednich.Add(lista.Value);
            }
            SredniaOperatora.Add(i, sumaSrednich.Sum() / sumaSrednich.Count);
        }
        #endregion

        #region CALKOWITE ROZSTEPY DLA SERII
        for (var i = 1; i <= liczbaOperatorow; i++)
        {
            var listaPomiarow = RozstepWyrobowDlaOperatora.Where(x => x.Key.Item1 == i);
            var sumaRozstepow = new List<double>();
            foreach (var lista in listaPomiarow)
            {
                sumaRozstepow.Add(lista.Value);
            }
            RozstepOperatora.Add(i, sumaRozstepow.Sum() / sumaRozstepow.Count);
        }
        #endregion

        var rsr = RozstepOperatora.Values.Sum()
                  / RozstepOperatora.Count;
        var xdiff = SredniaOperatora.Values.Max()
                    - SredniaOperatora.Values.Min();
        var rp = CalkowiteSrednieWyrobow.Values.Max()
                 - CalkowiteSrednieWyrobow.Values.Min();
        var ev = k1 * rsr;
        var av = k2 * xdiff;
        var grr = Math.Sqrt(Math.Pow(ev, 2) + Math.Pow(av, 2));
        var pv = (rp * k3);
        var tv = Math.Sqrt(Math.Pow(grr, 2) + Math.Pow(pv, 2));
        var percentEv = 100 * (Math.Pow(ev, 2) / (grr * t));
        var percentAv = 100 * (Math.Pow(av, 2) / (grr * t));
        var percentGrr = percentEv + percentAv;
        var percentPv = 100 * (pv / tv);


        return new SecondProcedureResult()
        {
            Rp = rp,
            EV = ev,
            AV = av,
            GRR = grr,
            PV = pv,
            TV = tv,
            percentEV = percentEv,
            percentAV = percentAv,
            percentGRR = percentGrr,
            percentPV = percentPv,
            _t = t,
            Rsr = rsr,
            Xdiff = xdiff
        };
    }

    private void ClearData()
    {
        CalkowiteSrednieWyrobow.Clear();
        CalkowityRozstepWyrobow.Clear();
        SrednieWyrobowDlaOperatora.Clear();
        RozstepWyrobowDlaOperatora.Clear();
        SredniaOperatora.Clear();
        RozstepOperatora.Clear();
    }
}

