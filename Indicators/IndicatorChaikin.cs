using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorChaikin : IndicatorBase, IIndicator
{

    public double limit;
    public int iLong = 10;
    public int iShort = 3;
    public double high = 0;
    public double low = 0;
    public string timeGraph = MainClass.timeGraph;
    public IndicatorChaikin()
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

        if (cfg.ContainsKey("long"))
            this.iLong = int.Parse(cfg["long"]);

        if (cfg.ContainsKey("short"))
            this.iShort = int.Parse(cfg["short"]);

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
        return "CHAIKIN";
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


            double[] arrayresultTA = new double[arrayPriceClose.Length];
            int outBegidx, outNbElement;
            arrayresultTA = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.AdOsc(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, arrayVolume, this.iShort, this.iLong, out outBegidx, out outNbElement, arrayresultTA);
            double chaikin = arrayresultTA[outNbElement - 1];
            this.result = chaikin;
            if (chaikin < this.low)
                return Operation.buy;
            if (chaikin > this.high)
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
