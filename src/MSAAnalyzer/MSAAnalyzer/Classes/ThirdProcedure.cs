using System;
using System.Collections.Generic;
using System.Linq;

namespace MSAAnalyzer.Classes;

public class ThirdProcedure
{
    public Dictionary<int, double> rozstepy = new();

    public ThirdProcedureResult Calculate(Dictionary<(int, int), double> pomiary, double _t, double K3)
    {
        ClearData();

        foreach (var klucz in pomiary.Keys)
        {
            if (klucz.Item1 == 1)
            {
                var odpKlucz = (2, klucz.Item2);
                if (pomiary.ContainsKey(odpKlucz))
                {
                    rozstepy[klucz.Item2] = Math.Abs(pomiary[klucz] - pomiary[odpKlucz]);
                }
            }
        }

        var sredniRozstep = rozstepy.Values.Average();
        var ev =  K3 * sredniRozstep;
        var percentEv = 100 * (ev / _t);

        return new ThirdProcedureResult
        {
            EV = ev,
            percentEV = percentEv,
            rozstepSredni = sredniRozstep
        };
    }

    private void ClearData()
    {
        rozstepy.Clear();
    }
}

