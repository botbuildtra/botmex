using System;
using System.Collections.Generic;

public class IndicatorOBV : IndicatorBase, IIndicator
{
    public double limit = 10;
    public double previous = 0;
    public double lastPerc;
    public string timeGraph = MainClass.timeGraph;
    public IndicatorOBV()
    {
        this.indicator = this;
        this.period = 14;
    }

    public void Setup(Dictionary<string, string> cfg)
    {
        if (cfg.ContainsKey("period"))
            setPeriod(int.Parse(cfg["period"]));

        if (cfg.ContainsKey("limit"))
            setLimit(int.Parse(cfg["limit"]));

        if (cfg.ContainsKey("timegraph") && (cfg["timegraph"].Trim() == "1m" || cfg["timegraph"].Trim() == "5m" || cfg["timegraph"].Trim() == "1h"))
            timeGraph = cfg["timegraph"].Trim();
    }

    public string getName()
    {
        return "OBV";
    }

    public string getTimegraph()
    {
        return timeGraph;
    }

    public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {

        int obv1 = 0;
        int obv2 = 0;
        double[] obv3 = new double[arrayPriceClose.Length];
        TicTacTec.TA.Library.Core.Obv(0, arrayPriceClose.Length - 1, arrayPriceClose, arrayVolume, out obv1, out obv2, obv3);
        double obvVal = obv3[obv2 - 1];
        this.result = obvVal;
        this.result2 = obvVal;
        double percDiff = ((obvVal * 100) / previous) - 100;

        if ((int)percDiff == 0 || double.IsInfinity(percDiff) )
        {
            previous = obvVal;
        }

        MainClass.log("OBV: " + this.result);
        MainClass.log("OBV Perc Diff: " + Math.Abs(percDiff));
        if (Math.Abs(percDiff) < this.limit || double.IsInfinity(percDiff))
            return Operation.allow;

        MainClass.log("REJECT");
        return Operation.nothing;

    }

    public double getResult()
    {
        return this.result;
    }

    public double getResult2()
    {
        return this.result;
    }

    public Tendency getTendency()
    {
        return Tendency.nothing;
    }

    public TypeIndicator getTypeIndicator()
    {
        return TypeIndicator.Threshold;
    }

    public void setPeriod(int period)
    {
        this.period = period;
    }

    public void setLimit(double limit)
    {
        this.limit = limit;
    }

    public void setHigh(double high)
    {
    }

    public void setLow(double low)
    {
    }
}
