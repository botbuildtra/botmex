using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorMATENDENCY : IndicatorBase, IIndicator
{
    public double ilong;
    public double ishort;
    public double limit;
    public string timeGraph = MainClass.timeGraph;
    public IndicatorMATENDENCY()
    {
        this.indicator = this;
    }

    public void Setup(Dictionary<string,string> cfg )
    {
        if (cfg.ContainsKey("long"))
            setHigh(int.Parse(cfg["long"]));

        if (cfg.ContainsKey("short"))
            setLow(int.Parse(cfg["short"]));

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
        return TypeIndicator.Cross;
    }

    public string getName()
    {
        return "MATENDENCY";
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
            int outBegidxLonga, outNbElementLonga, outBegidxCurta, outNbElementCurta;
            double[] arrayLonga = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, Convert.ToInt32(this.ilong), TicTacTec.TA.Library.Core.MAType.Ema, out outBegidxLonga, out outNbElementLonga, arrayLonga);
            double value = arrayLonga[outNbElementLonga - 1];
            this.result = value;

            double[] arrayCurta = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, Convert.ToInt32(this.ishort), TicTacTec.TA.Library.Core.MAType.Ema, out outBegidxCurta, out outNbElementCurta, arrayCurta);
            double value2 = arrayCurta[outNbElementCurta - 1];
            this.result2 = value2;

            /* Debug*/
            /*Random rnd = new Random();
            int op = rnd.Next(1, 3);
            if (op == 1)
                return Operation.buy;
            if (op == 2)
                return Operation.sell;
            if (op == 3)
                return Operation.nothing;*/

            if (value2 > value)
            {
                return Operation.buy;
            }

            if( value > value2)
            {
                return Operation.sell;
            }

            return Operation.nothing;

            /*if ((arrayLonga[outNbElementLonga - 2] >= arrayCurta[outNbElementCurta - 2]) && arrayCurta[outNbElementCurta - 1] > arrayLonga[outNbElementLonga - 1])
                return Operation.buy;
            if ((arrayLonga[outNbElementLonga - 2] <= arrayCurta[outNbElementCurta - 2]) && arrayCurta[outNbElementCurta - 1] < arrayLonga[outNbElementLonga - 1])
                return Operation.sell;

            return Operation.nothing;*/
        }
        catch
        {
            return Operation.nothing;
        }
    }

    public void setHigh(double high)
    {
        this.ilong = high;
    }

    public void setLow(double low)
    {
        this.ishort = low;
    }

    public void setLimit(double limit)
    {
        this.limit = limit;
    }
}
