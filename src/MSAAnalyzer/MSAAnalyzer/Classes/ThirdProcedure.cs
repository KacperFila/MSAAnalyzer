using System;
using System.Collections.Generic;
using System.Linq;

namespace MSAAnalyzer.Classes;

internal class ThirdProcedure
{
    public Dictionary<int, double> roznice = new();
    public Dictionary<int, double> rozstepy = new();

    public ThirdProcedureResult Calculate(Dictionary<(int, int), double> pomiary, double _t)
    {
        ClearData();

        foreach (var klucz in pomiary.Keys)
        {
            if (klucz.Item1 == 1)
            {
                var odpKlucz = (2, klucz.Item2);
                if (pomiary.ContainsKey(odpKlucz))
                {
                    roznice[klucz.Item2] = pomiary[klucz] - pomiary[odpKlucz];
                    rozstepy[klucz.Item2] = Math.Abs(pomiary[klucz] - pomiary[odpKlucz]);
                }
            }
        }


        var sredniaRoznica = roznice.Values.Average();
        var sumOfSquares = roznice.Values.Sum(d => Math.Pow(d - sredniaRoznica, 2));
        var variance = sumOfSquares / (roznice.Count - 1);
        var standardDeviation = Math.Sqrt(variance);
        var rozrzutSystemu = standardDeviation / Math.Sqrt(2);
        var EV = 6 * rozrzutSystemu;
        var percentEV = 100 * (EV / _t);

        return new ThirdProcedureResult
        {
            SredniaRoznica = sredniaRoznica,
            EV = EV,
            percentEV = percentEV,
            rozrzutSystemu = rozrzutSystemu,
            StandardDeviation = standardDeviation,
            SumOfSquares = sumOfSquares,
            Variance = variance
        };
    }

    private void ClearData()
    {
        roznice.Clear();
        rozstepy.Clear();
    }
}

