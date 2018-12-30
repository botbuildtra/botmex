using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBotBackToTheFuture;

public class IndicatorCAROLBEBENDO : IndicatorBase, IIndicator
{

    public double limit;
    public double high = 34;
    public double low = 17;
    private double atr = 6;
    private bool atrenable = false;
    public IndicatorCAROLBEBENDO()
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
        return "CAROLBEBENDO";
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

        /* Debug*/
        /*Random rnd = new Random();
        int op = rnd.Next(1, 3);
        if (op == 1)
            return Operation.buy;
        if (op == 2)
            return Operation.sell;
        if (op == 3)
            return Operation.nothing;*/

        
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

            MainClass.log("Download candles 5m para processar MA");
            MainClass.getCandles("5m");
            IndicatorMA ma = new IndicatorMA();
            ma.setHigh(this.high);
            ma.setLow(this.low);
            Operation operationMA = ma.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);


            //TicTacTec.TA.Library.Core.Atr();

            MainClass.log("CCI: " + cci.result);
            MainClass.log("CCI Tendency: " + cci.getTendency());
            MainClass.log("RSI: " + rsi.result);
            MainClass.log("RSI Tendency: " + rsi.getTendency());
            MainClass.log("MACD: " + macd.result);
            MainClass.log("MACD Sig: " + macd.result2);
            MainClass.log("MACD OP: " + operationMACD.ToString());
            MainClass.log("EMA Long 5m: " + ma.getResult());
            MainClass.log("EMA Short 5m: " + ma.getResult2());


            if (MainClass.carolatr)
            {
                MainClass.log("ATR: " + atrVal);
            }
            //return Operation.buy;
            if (cci.result > 0 && operationMACD == Operation.buy && rsi.result > 50 /*&& cci.getTendency() == Tendency.high && rsi.getTendency() == Tendency.high*/ && ((MainClass.carolatr && atrVal < MainClass.atrvalue) || !MainClass.carolatr))
            //if (operationMACD == Operation.buy)
            {
                /*double[] arrayresultMAHigh = new double[arrayPriceClose.Length];
                int outBegidxHigh, outNbElementHigh;
                TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, Convert.ToInt32(this.high), TicTacTec.TA.Library.Core.MAType.Ema, out outBegidxHigh, out outNbElementHigh, arrayresultMA);
                if (arrayPriceClose[arrayPriceClose.Length - 1] > arrayresultMAHigh[outNbElementHigh - 1])
                    return Operation.buy;*/

                if (ma.getResult2() > ma.getResult())
                {
                    return Operation.buy;
                }
            }
            if (cci.result < 0 && operationMACD == Operation.sell && rsi.result < 50 /*&& cci.getTendency() == Tendency.low && rsi.getTendency() == Tendency.low */&& ((MainClass.carolatr && atrVal < MainClass.atrvalue) || !MainClass.carolatr))
            //if (operationMACD == Operation.sell)
            {
                /*double[] arrayresultMA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, 100, TicTacTec.TA.Library.Core.MAType.Ema, out outBegidx, out outNbElement, arrayresultMA);
                if (arrayPriceClose[arrayPriceClose.Length - 1] < arrayresultMA[outNbElement - 1])
                    return Operation.sell;*/
                if (ma.getResult2() < ma.getResult())
                {
                    return Operation.sell;
                }
            }
            return Operation.nothing;
        }
        catch
        {
            return Operation.nothing;
        }
    }

    public void setAtr(double atr)
    {
        this.atr = atr;
    }

    public void enableAtr(bool val)
    {
        this.atrenable = val;
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