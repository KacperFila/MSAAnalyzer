using System;
using System.Collections.Generic;
using System.Linq;

namespace MSAAnalyzer.Classes;

public class FirstProcedure
{
    internal List<double> Data;
    private double GornaGranica;
    private double DolnaGranica;
    internal double WartoscWzorca;
    public double K;
        
    public FirstProcedure(List<double> wartosciWzorcowe, double K)
    {
        Data = wartosciWzorcowe;
        this.K = K;
    }

    public void UpdateData(List<double> updatedData)
    {
        Data = updatedData;
    }

    public void SetWartoscWzorca(double value)
    {
        WartoscWzorca = value;
    }

    public void SetGranice(double gorna, double dolna)
    {
        GornaGranica = gorna;
        DolnaGranica = dolna;
    }

    public FirstProcedureResult? Calculate()
    {
        var diffT = GornaGranica - DolnaGranica;
        if (!Data.Any())
        {
            return new FirstProcedureResult()
            {
                Mean = 0,
                SumOfSquares = 0,
                Cg = 0,
                Cgk = 0,
                Sigma = 0,
                Variance = 0,
                T = diffT
            };
        }
        var mean = Data.Average();
        var sumOfSquares = Data.Sum(x => (x - mean) * (x - mean));
        var variance = sumOfSquares / (Data.Count - 1);
        var sigma = Math.Sqrt(variance);
        var cg = K * (diffT/ (6 * sigma));
        var cgk = (0.5*K * diffT - (Math.Abs(mean - WartoscWzorca))) / (3 * sigma);

        return new FirstProcedureResult
        {
            Mean = mean,
            SumOfSquares = WartoscWzorca,
            Cg = cg,
            Cgk = cgk,
            Sigma = sigma,
            Variance = variance,
            T = diffT
        };
    }
}