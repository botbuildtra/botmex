using System;


public class IndicatorATR : IndicatorBase, IIndicator
{
    public double atr = 5;
    public bool atrenable = false;

    public IndicatorATR()
    {
        this.indicator = this;
        this.period = 14;
    }
    public string getName()
    {
        return "ATR";
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

    public void setHigh(double high)
    {
    }

    public void setLow(double low)
    {
    }
}
