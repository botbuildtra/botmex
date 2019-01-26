using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorMFI : IndicatorBase, IIndicator
{
    public double high = 80;
    public double low = 20;
    public double limit;
    public string timeGraph = MainClass.timeGraph;
    public IndicatorMFI()
    {
        this.indicator = this;
        this.period = 20;
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
        return "MFI";
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
            int outBegidx, outNbElement;
            double[] arrayresultTA = new double[arrayPriceClose.Length];
            arrayresultTA = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Mfi(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, arrayVolume, this.period, out outBegidx, out outNbElement, arrayresultTA);
            double value = arrayresultTA[outNbElement - 1];

            this.tendency = Tendency.nothing;
            if (arrayresultTA[outNbElement - 2] < arrayresultTA[outNbElement - 1] && arrayresultTA[outNbElement - 3] < arrayresultTA[outNbElement - 2])
                this.tendency = Tendency.high;
            if (arrayresultTA[outNbElement - 2] > arrayresultTA[outNbElement - 1] && arrayresultTA[outNbElement - 3] > arrayresultTA[outNbElement - 2])
                this.tendency = Tendency.low;

            this.result = value;
            if (value == 0)
                return Operation.nothing;
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
