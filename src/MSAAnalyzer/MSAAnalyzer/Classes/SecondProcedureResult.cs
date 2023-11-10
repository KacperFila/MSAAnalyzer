using System.Collections.Generic;

namespace MSAAnalyzer.Classes;

public record SecondProcedureResult
{
     // Srednia A, B, C ... RANGE
    public double Rsr;
    public double Xdiff;
    public double Rp;
    public double EV;
    public double AV;
    public double GRR;
    public double PV;
    public double TV;
    public double percentEV;
    public double percentAV;
    public double percentGRR;
    public double percentPV;
    const double k1 = 3.05;
    const double k2 = 3.65;
    const double k3 = 0.4030;
    public double _t;
}
