using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorPPO : IndicatorBase, IIndicator
{
    public double high = 1;
    public double low = -1;

    public double limit;

    public IndicatorPPO()
    {
        this.indicator = this;
    }
    public string getName()
    {
        return "PPO";
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
            TicTacTec.TA.Library.Core.Ppo(0, arrayPriceClose.Length - 1, arrayPriceClose, Convert.ToInt32(this.low), Convert.ToInt32(this.high), TicTacTec.TA.Library.Core.MAType.Sma, out outBegidx, out outNbElement, result);
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
