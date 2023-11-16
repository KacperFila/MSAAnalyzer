using System.Collections.Generic;

namespace MSAAnalyzer.DataContext;

public class AppDataContext
{
        private static AppDataContext _instance;

        public List<double> FirstProcedureMeasurements = new();
        public Dictionary<(int, int, int), double> SecondProcedureMeasurements = new();
        public Dictionary<(int, int), double> ThirdProcedureMeasurements = new();
        public double K { get; set; } = 0.2;
        public double T { get; set; }
        public double K1 { get; set; } = 3.05; // procedura 2
        public double K2 { get; set; } = 2.7;
        public double K3 { get; set; } = 0.403;

        public string getK3()
        {
            return K3.ToString();
        }
        public AppDataContext()
        {
            for (var i = 1; i <= 2; i++)
            {
                for (var j = 1; j <= 25; j++)
                {
                    ThirdProcedureMeasurements[(i, j)] = 0;
                }
            }
        }

        public static AppDataContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppDataContext();
                }
                return _instance;
            }
        }
}
