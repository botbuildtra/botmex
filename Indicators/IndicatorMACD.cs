﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorMACD : IndicatorBase, IIndicator
{

    public double high;
    public double low;
    public double limit;

    public IndicatorMACD()
    {
        this.indicator = this;
    }

    public string getName()
    {
        return "MACD";
    }
    public void setPeriod(int period)
    {
        this.period = period;
    }

    public TypeIndicator getTypeIndicator()
    {
        return TypeIndicator.Cross;
    }

    public double getResult()
    {
        return this.result;
    }

    public Tendency getTendency()
    {
        return this.tendency;
    }

    public double getResult2()
    {
        return this.result2;
    }
    public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {
        try
        {


            double[] arrayresultTA = new double[arrayPriceClose.Length];
            int outBegidx, outNbElement;
            double[] macdSignal = new double[arrayPriceClose.Length];
            double[] macdHist = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Macd(0, arrayPriceClose.Length - 1, arrayPriceClose, 12, 26, 9, out outBegidx, out outNbElement, arrayresultTA, macdSignal, macdHist);
            double macd = arrayresultTA[outNbElement - 1];
            double signal = macdSignal[outNbElement - 1];
            double macdHistory = macdHist[outNbElement - 1];
            this.result = macd;
            this.result2 = signal;
            if (macdHistory < this.low)
                return Operation.buy;
            if (macdHistory > this.high)
                return Operation.sell;

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
