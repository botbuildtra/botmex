﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorSTOCH : IndicatorBase, IIndicator
{

    private double high = 80;
    private double low = 20;
    private double limit;

    public IndicatorSTOCH()
    {
        this.indicator = this;
    }
    public string getName()
    {
        return "STOCH";
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

            double[] outK = new double[arrayPriceClose.Length];
            double[] outD = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Stoch(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, 14, 3, TicTacTec.TA.Library.Core.MAType.Sma, 14, TicTacTec.TA.Library.Core.MAType.Sma, out outBegidx, out outNbElement, outK, outD);
            double stochRsiK = outK[outNbElement - 1];
            double stochRsiD = outD[outNbElement - 1];

            this.result = stochRsiK;
            this.result2 = stochRsiD;

            if (stochRsiK > this.high && stochRsiD > this.high)
                return Operation.sell;
            if (stochRsiK < this.low && stochRsiD < this.low)
                return Operation.buy;
            return Operation.nothing;
        }
        catch
        {
            return Operation.nothing;
        }
    }

    public void setData(int high, int low)
    {
        this.high = high;
        this.low = low;
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