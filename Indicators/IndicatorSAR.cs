using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorSAR : IndicatorBase, IIndicator
{
    private double high;
    private double low;
    private double limit;
    public double accel = 0.02;
    public double max = 0.2;
    public string timeGraph = MainClass.timeGraph;
    public IndicatorSAR()
    {
        this.indicator = this;
    }

    public void Setup(Dictionary<string, string> cfg)
    {
        if (cfg.ContainsKey("high"))
            setHigh(int.Parse(cfg["high"]));

        if (cfg.ContainsKey("low"))
            setLow(int.Parse(cfg["low"]));

        if (cfg.ContainsKey("period"))
            setPeriod(int.Parse(cfg["period"]));

        if (cfg.ContainsKey("max"))
            this.max = double.Parse(cfg["max"]);
        if (cfg.ContainsKey("accel"))
            this.accel = double.Parse(cfg["accel"]);

        if (cfg.ContainsKey("timegraph") && (cfg["timegraph"].Trim() == "1m" || cfg["timegraph"].Trim() == "5m" || cfg["timegraph"].Trim() == "1h"))
            timeGraph = cfg["timegraph"].Trim();
    }

    public void setPeriod(int period)
    {
        this.period = period;
    }

    public string getTimegraph()
    {
        return timeGraph;
    }

    public TypeIndicator getTypeIndicator()
    {
        return TypeIndicator.Normal;
    }

    public string getName()
    {
        return "SAR";
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

    public double[] arrayresultTA;

    public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {
        try
        {
            int outBegidx, outNbElement;            
            arrayresultTA = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Sar(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, accel, max, out outBegidx, out outNbElement, arrayresultTA);            
            double value = arrayresultTA[outNbElement - 1];
            double lastValue = arrayresultTA[outNbElement - 2];
            double priceClose = arrayPriceClose[arrayPriceClose.Length - 1];
            this.result = value;
            this.result2 = lastValue;
            if (value < priceClose && lastValue > arrayPriceClose[arrayPriceClose.Length - 2])
                return Operation.buy;
            if (value > priceClose && lastValue < arrayPriceClose[arrayPriceClose.Length - 2])
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
