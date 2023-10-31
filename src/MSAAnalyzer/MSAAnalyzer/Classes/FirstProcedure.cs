﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MSAAnalyzer.Classes
{
    internal class FirstProcedure
    {
        internal List<double> Data;
        private double GornaGranica;
        private double DolnaGranica;
        internal double WartoscWzorca;
        internal int LicznikElementów = 0;
        const double K = 0.2;
        
        public FirstProcedure(List<double> wartosciWzorcowe)
        {
            Data = wartosciWzorcowe;
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

        public void IncrementLicznikElementow()
        {
            LicznikElementów++;
        }
        public FirstProcedureResult Calculate()
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
                    LicznikElementow = 0,
                    T = diffT
                };
            }
            var mean = Data.Average();
            var sumOfSquares = Data.Sum(x => (x - mean) * (x - mean));
            var variance = sumOfSquares / (Data.Count - 1);
            var sigma = Math.Sqrt(variance);
            var cg = K * (diffT/ (4 * sigma));
            var cgk = (0.1 * diffT - (Math.Abs(mean - WartoscWzorca))) / (2 * sigma);
            return new FirstProcedureResult
            {
                Mean = mean,
                SumOfSquares = WartoscWzorca,
                Cg = cg,
                Cgk = cgk,
                Sigma = sigma,
                Variance = variance,
                LicznikElementow = this.LicznikElementów,
                T = diffT
            };
        }
    }
}