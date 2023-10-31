namespace MSAAnalyzer.Classes;
    internal record FirstProcedureResult
    {
        public double Mean { get; set; } = 0;
        public double SumOfSquares { get; set; } = 0;
        public double Variance { get; set; } = 0;
        public double Sigma { get; set; } = 0;
        public double Cg { get; set; } = 0;
        public double Cgk { get; set; } = 0;
        public double T { get; set; } = 0;
        public int LicznikElementow { get; set; } = 0;
    }

