using System;
using System.Collections.Generic;

public class IndicatorATRD : IndicatorBase, IIndicator
{
    public double limit = 5;
    public double lastPerc;
    public string timeGraph = MainClass.timeGraph;

    public IndicatorATRD()
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
        return "ATRD";
    }

    public string getTimegraph()
    {
        return timeGraph;
    }

    public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {
        int atr1 = 0;
        int atr2 = 0;
        double[] atr3 = new double[arrayPriceClose.Length];
        TicTacTec.TA.Library.Core.Atr(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, this.period, out atr1, out atr2, atr3);
        double atrVal = atr3[atr2 - 1];
        double prevAtr = atr3[atr2 - 2];
        this.result = atrVal;
        this.result2 = prevAtr;



        double percDiff = ((atrVal * 100) / prevAtr) - 100;
        double rlp = lastPerc;    
        lastPerc = percDiff;
            
        MainClass.log("ATR: " + this.result);
        MainClass.log("ARR Prev ATR: " + prevAtr);
        MainClass.log("Previous ATR: " + prevAtr);
        MainClass.log("ATR Perc Diff: " + Math.Abs(percDiff));
        MainClass.log("ATR Last Perc Diff: " + Math.Abs(rlp));
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
        if(period != 0)
            this.period = period;
    }

    public void setLimit(double limit)
    {
        if((int)limit != 0)
            this.limit = limit;
    }
}
