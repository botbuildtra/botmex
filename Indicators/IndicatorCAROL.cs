using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBotBackToTheFuture;

public class IndicatorCAROL : IndicatorBase, IIndicator
{

    public double high, low, limit;
    private double atr = 6;
    private bool atrenable = false;
    public string timeGraph = MainClass.timeGraph;
    public IndicatorCAROL()
    {
        this.indicator = this;
    }

    public void Setup(Dictionary<string, string> cfg)
    {
        if (cfg.ContainsKey("atr"))
            setAtr(int.Parse(cfg["atr"]));

        if (cfg.ContainsKey("period"))
            setPeriod(int.Parse(cfg["period"]));

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

            //TicTacTec.TA.Library.Core.Atr();

            MainClass.log("CCI: " + cci.result);
            MainClass.log("CCI Tendency: " + cci.getTendency());  
            MainClass.log("RSI: " + rsi.result);
            MainClass.log("RSI Tendency: " + rsi.getTendency());
            MainClass.log("MACD: " + macd.result);
            if( MainClass.carolatr )
            {
                MainClass.log("ATR: " + atrVal);
            }

            if (cci.result > 0 && operationMACD == Operation.buy && rsi.result > 50 && cci.getTendency() == Tendency.high && rsi.getTendency() == Tendency.high && ((MainClass.carolatr && atrVal < MainClass.atrvalue) || !MainClass.carolatr))
            //if (operationMACD == Operation.buy)
            {
                double[] arrayresultMA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, 100, TicTacTec.TA.Library.Core.MAType.Ema, out outBegidx, out outNbElement, arrayresultMA);
                if (arrayPriceClose[arrayPriceClose.Length - 1] > arrayresultMA[outNbElement - 1])
                    return Operation.buy;
            }
            if (cci.result < 0 && operationMACD == Operation.sell && rsi.result < 50 && cci.getTendency() == Tendency.low && rsi.getTendency() == Tendency.low && ((MainClass.carolatr && atrVal < MainClass.atrvalue) || !MainClass.carolatr))
            //if (operationMACD == Operation.sell)
            {
                double[] arrayresultMA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                TicTacTec.TA.Library.Core.MovingAverage(0, arrayPriceClose.Length - 1, arrayPriceClose, 100, TicTacTec.TA.Library.Core.MAType.Ema, out outBegidx, out outNbElement, arrayresultMA);
                if (arrayPriceClose[arrayPriceClose.Length - 1] < arrayresultMA[outNbElement - 1])
                    return Operation.sell;
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