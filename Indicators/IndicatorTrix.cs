﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorTRIX : IndicatorBase, IIndicator
{
    public double high, low = 0;
    public double limit;
    public IndicatorTRIX()
    {
        this.indicator = this;
        this.period = 18;
    }
    public string getName()
    {
        return "TRIX";
    }
    public TypeIndicator getTypeIndicator()
    {
        return TypeIndicator.Normal;
    }

    public void setPeriod(int period)
    {
        this.period = period;
    }

    public double getResult()
    {
        return this.result;
    }

    public double getResult2()
    {
        return this.result2;
    }
    public Tendency getTendency()
    {
        return this.tendency;
    }

    public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {
        try
        {
            int outBegidx, outNbElement;
            double[] arrayresultTA = new double[arrayPriceClose.Length];
            arrayresultTA = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Trix(0, arrayPriceClose.Length - 1, arrayPriceClose, this.period, out outBegidx, out outNbElement, arrayresultTA);
            double value = arrayresultTA[outNbElement - 1];
            this.result = value;
            if (value > this.high)
                return Operation.sell;
            if (value < this.low)
                return Operation.buy;
            return Operation.nothing;
        }
        catch
        {
            return Operation.nothing;
        }
    }

    public void setHigh(double high)
    {
        this.high = high;
    }

    public void setLow(double low)
    {
        this.low = low;
    }

    public void setLimit(double limit)
    {
        this.limit = limit;
    }
}