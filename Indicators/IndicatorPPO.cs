using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorPPO : IndicatorBase, IIndicator
{
    public double high = 1;
    public double low = -1;
    public int iLong = 21;
    public int iShort = 10;
    public string timeGraph = MainClass.timeGraph;
    public double limit;

    public IndicatorPPO()
    {
        this.indicator = this;
    }

    public void Setup(Dictionary<string, string> cfg)
    {
        if (cfg.ContainsKey("high"))
            setHigh(double.Parse(cfg["high"]));

        if (cfg.ContainsKey("low"))
            setLow(double.Parse(cfg["low"]));

        if (cfg.ContainsKey("period"))
            setPeriod(int.Parse(cfg["period"]));

        if (cfg.ContainsKey("long"))
            this.iLong = int.Parse(cfg["long"]);

        if (cfg.ContainsKey("short"))
            this.iShort = int.Parse(cfg["short"]);

        if (cfg.ContainsKey("timegraph") && (cfg["timegraph"].Trim() == "1m" || cfg["timegraph"].Trim() == "5m" || cfg["timegraph"].Trim() == "1h"))
            timeGraph = cfg["timegraph"].Trim();
    }

    public string getName()
    {
        return "PPO";
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
            double[] result = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Ppo(0, arrayPriceClose.Length - 1, arrayPriceClose, this.iShort, this.iLong, TicTacTec.TA.Library.Core.MAType.Sma, out outBegidx, out outNbElement, result);
            double priceClose = arrayPriceClose[arrayPriceClose.Length - 1];
            double value = result[outNbElement - 1];
            this.result = value;
            if (value > this.low)
                return Operation.buy;
            if (value > this.high)
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
