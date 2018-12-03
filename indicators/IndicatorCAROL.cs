using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndicatorCAROL : IndicatorBase, IIndicator
{
    private int atr = 6;
    private bool atrenable = false;
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
        Operation op1 = GetOperationDetail(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);


        double diffCandle = Math.Abs((((arrayPriceClose[arrayPriceClose.Length - 1] * 100) / arrayPriceClose[arrayPriceClose.Length - 2]) - 100));

        Console.WriteLine("diffCandle: " + diffCandle + "%");

        if (diffCandle < 0.2)
            return op1;


        return Operation.nothing;

    }

    public Operation GetOperationDetail(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {
        try
        {

            int atr1 = 0;
            int atr2 = 0;
            double[] atr3 = new double[arrayPriceClose.Length];
            TicTacTec.TA.Library.Core.Atr(0, arrayPriceClose.Length - 1, arrayPriceHigh, arrayPriceLow, arrayPriceClose, 14, out atr1, out atr2, atr3);
            double atrVal = atr3[atr2 - 1];

            IndicatorMACD macd = new IndicatorMACD();
            Operation operationMACD = macd.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);

            IndicatorCCI cci = new IndicatorCCI();
            Operation operationCCI = cci.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);

            IndicatorRSI rsi = new IndicatorRSI();
            Operation operationRSI = rsi.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);

            MainClass.log("CCI " + cci.result);
            MainClass.log("RSI " + rsi.result);
            if( atrenable)
            {
                MainClass.log("ATR " + atrVal);
            }

            if (cci.result > 0 && operationMACD == Operation.buy && rsi.result > 50 && cci.getTendency() == Tendency.high && rsi.getTendency() == Tendency.high && ( ( atrenable && atrVal < this.atr ) || !atrenable ) )
            {
                double[] arrayresultMA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, 200, TicTacTec.TA.Library.Core.MAType.Ema, out outBegidx, out outNbElement, arrayresultMA);
                if (arrayresultMA[outNbElement - 1] > arrayresultMA[outNbElement - 10] && arrayPriceClose[arrayPriceClose.Length - 1] > arrayresultMA[outNbElement - 1])
                    return Operation.buy;
            }
            if (cci.result < 0 && operationMACD == Operation.sell && rsi.result < 50 && cci.getTendency() == Tendency.low && rsi.getTendency() == Tendency.low && ((atrenable && atrVal < this.atr) || !atrenable) )
            {
                double[] arrayresultMA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, 200, TicTacTec.TA.Library.Core.MAType.Ema, out outBegidx, out outNbElement, arrayresultMA);
                if (arrayresultMA[outNbElement - 1] < arrayresultMA[outNbElement - 10] && arrayPriceClose[arrayPriceClose.Length - 1] < arrayresultMA[outNbElement - 1])
                    return Operation.sell;
            }

            return Operation.nothing;



        }
        catch
        {
            return Operation.nothing;
        }
    }

    public void setAtr(int atr)
    {
        this.atr = atr;
    }

    public void enableAtr(bool val )
    {
        this.atrenable = val;
    }
}
