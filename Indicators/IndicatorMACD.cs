using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorMACD : IndicatorBase, IIndicator
{

    public double ilong;
    public double ishort;
    public double limit;
    public double signal;
    public string timeGraph = MainClass.timeGraph;

    public IndicatorMACD()
    {
        this.indicator = this;
    }

    public string getName()
    {
        return "MACD";
    }

    public string getTimegraph()
    {
        return timeGraph;
    }

    public void setPeriod(int period)
    {
        this.period = period;
    }

    public void Setup(Dictionary<string, string> cfg)
    {
        if (cfg.ContainsKey("long"))
            setLong(int.Parse(cfg["long"]));

        if (cfg.ContainsKey("short"))
            setShort(int.Parse(cfg["short"]));

        if (cfg.ContainsKey("signal"))
            setSignal(int.Parse(cfg["signal"]));

        if (cfg.ContainsKey("timegraph") && (cfg["timegraph"].Trim() == "1m" || cfg["timegraph"].Trim() == "5m" || cfg["timegraph"].Trim() == "1h"))
            timeGraph = cfg["timegraph"].Trim();
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
            TicTacTec.TA.Library.Core.Macd(0, arrayPriceClose.Length - 1, arrayPriceClose, Convert.ToInt32(this.ishort), Convert.ToInt32(this.ilong), Convert.ToInt32(this.signal), out outBegidx, out outNbElement, arrayresultTA, macdSignal, macdHist);
            double macd = arrayresultTA[outNbElement - 1];
            double signal = macdSignal[outNbElement - 1];
            double macdHistory = macdHist[outNbElement - 1];
            this.result = macd;
            this.result2 = signal;
            if (macdHistory < this.ishort)
                return Operation.buy;
            if (macdHistory > this.ilong)
                return Operation.sell;

            return Operation.nothing;
        }
        catch
        {
            return Operation.nothing;
        }
    }

    public void setLong(double ilong)
    {
        this.ilong = ilong;
    }

    public void setShort(double ishort)
    {
        this.ishort = ishort;
    }

    public void setLimit(double limit)
    {
        this.limit = limit;
    }

    public void setSignal(double signal)
    {
        this.signal = signal;
    }
}
