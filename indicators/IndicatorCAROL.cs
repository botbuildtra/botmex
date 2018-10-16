using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorCAROL: IndicatorBase, IIndicator
{

    public IndicatorCAROL()
    {
        this.indicator = this;
    }

    public void setPeriod(int period)
    {
        this.period = period;
    }

    public TypeIndicator getTypeIndicator()
    {
        return TypeIndicator.Normal;
    }

    public string getName()
    {
        return "CAROL";
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
            Operation operationSAR = Operation.nothing;

            
            int outBegidx, outNbElement;            
            arrayresultTA = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Sar(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, 0.02, 0.2, out outBegidx, out outNbElement, arrayresultTA);            
            double value = arrayresultTA[outNbElement - 1];
            double lastValue = arrayresultTA[outNbElement - 2];
            double priceClose = arrayPriceClose[arrayPriceClose.Length - 1];
            this.result = value;
            this.result2 = lastValue;
            if (value < priceClose )
                operationSAR = Operation.buy;
            else if (value > priceClose )
                operationSAR =  Operation.sell;
            else
                operationSAR = Operation.nothing;



            IndicatorCCI cci = new IndicatorCCI();
            Operation operationCCI = cci.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);

            IndicatorRSI rsi = new IndicatorRSI();
            Operation operationRSI = rsi.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);

            MainClass.log("CCI " + cci.result);
            MainClass.log("RSI " + rsi.result);

            if (cci.result > 0 && operationSAR == Operation.buy && rsi.result > 50 && cci.getTendency() == Tendency.high && rsi.getTendency() == Tendency.high)
            {
                return Operation.buy;
            }
            if (cci.result < 0 && operationSAR == Operation.sell && rsi.result < 50 && cci.getTendency() == Tendency.low && rsi.getTendency() == Tendency.low)
            {
                return Operation.sell;
            }

            return Operation.nothing;



        }
        catch
        {
            return Operation.nothing;
        }
    }
}
