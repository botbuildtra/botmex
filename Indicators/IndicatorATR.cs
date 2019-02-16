using System;
using System.Collections.Generic;

public class IndicatorATR : IndicatorBase, IIndicator
{
    public double atr = 5;
    public bool atrenable = false;
    public string timeGraph = MainClass.timeGraph;

    public IndicatorATR()
    {
        this.indicator = this;
        this.period = 14;
    }

    public void Setup(Dictionary<string, string> cfg)
    {
        if (cfg.ContainsKey("period"))
            setPeriod(int.Parse(cfg["period"]));

        if (cfg.ContainsKey("limit"))
            setLimit(double.Parse(cfg["limit"]));

        if (cfg.ContainsKey("timegraph") && (cfg["timegraph"].Trim() == "1m" || cfg["timegraph"].Trim() == "5m" || cfg["timegraph"].Trim() == "1h"))
            timeGraph = cfg["timegraph"].Trim();
    }

    public string getName()
    {
        return "ATR";
    }

    public string getTimegraph()
    {
        return timeGraph;
    }

    public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {
        if (!atrenable)
            return Operation.allow;

        int atr1 = 0;
        int atr2 = 0;
        double[] atr3 = new double[arrayPriceClose.Length];
        TicTacTec.TA.Library.Core.Atr(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, this.period, out atr1, out atr2, atr3);
        double atrVal = atr3[atr2 - 1];
        this.result = atrVal;
        this.result2 = atrVal;

        MainClass.log("ATR: " + this.result);
        if (atrVal < this.atr)
            return Operation.allow;

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

    public void setAtr(double atr)
    {
        this.atr = atr;
    }

    public void enableAtr(bool val)
    {
        this.atrenable = val;
    }

    public void setLimit(double limit)
    {
        if(Math.Abs(limit) < 0 || Math.Abs(limit) > 0)
        {
            this.setAtr(limit);
            this.enableAtr(true);
        }
    }
}
