namespace MSAAnalyzer.Classes;
    public record FirstProcedureResult
    {
        public double Mean { get; set; }
        public double SumOfSquares { get; set; }
        public double Variance { get; set; }
        public double Sigma { get; set; }
        public double Cg { get; set; }
        public double Cgk { get; set; }
        public double T { get; set; }
    }

