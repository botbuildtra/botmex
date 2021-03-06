﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorDI : IndicatorBase, IIndicator
{

    private double high;
    private double low;
    private double limit;
    public string timeGraph = MainClass.timeGraph;
    public IndicatorDI()
    {
        this.indicator = this;
        this.period = 14;
    }

    public void Setup(Dictionary<string, string> cfg)
    {
        if (cfg.ContainsKey("high"))
            setHigh(int.Parse(cfg["high"]));

        if (cfg.ContainsKey("low"))
            setLow(int.Parse(cfg["low"]));

        if (cfg.ContainsKey("period"))
            setPeriod(int.Parse(cfg["period"]));

        if (cfg.ContainsKey("timegraph") && (cfg["timegraph"].Trim() == "1m" || cfg["timegraph"].Trim() == "5m" || cfg["timegraph"].Trim() == "1h"))
            timeGraph = cfg["timegraph"].Trim();
    }

    public string getName()
    {
        return "DI";
    }

    public string getTimegraph()
    {
        return timeGraph;
    }

    public void setPeriod(int period)
    {
        this.period = period;
    }

    public TypeIndicator getTypeIndicator()
    {
        return TypeIndicator.Normal;
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
            double priceClose = arrayPriceClose[arrayPriceClose.Length - 1];

            double[] result = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.MinusDI(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, this.period, out outBegidx, out outNbElement, result);
            double valueMinus = result[outNbElement - 1];

            result = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.PlusDI(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, this.period, out outBegidx, out outNbElement, result);
            double valuePlus = result[outNbElement - 1];

            this.result = valueMinus;
            this.result2 = valuePlus;

            if (valuePlus > valueMinus)
                return Operation.buy;
            if (valueMinus > valuePlus)
                return Operation.sell;
            return Operation.nothing;
        }
        catch
        {
            return Operation.nothing;
        }
    }

    public double getResult()
    {
        return this.result;
    }

    public double getResult2()
    {
        return this.result2;
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
