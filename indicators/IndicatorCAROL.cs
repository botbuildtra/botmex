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
        Operation op1 = GetOperationDetail(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);
        Console.WriteLine("sleep 10s");
        System.Threading.Thread.Sleep(10000);
        Operation op2 = GetOperationDetail(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);
        Console.WriteLine("sleep 10s");
        System.Threading.Thread.Sleep(10000);
        Operation op3 = GetOperationDetail(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);


        if (op1 == op2 && op2 == op3)
            return op1;
        else
            return Operation.nothing;

    }

    public Operation GetOperationDetail(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
    {
        try
        {
            IndicatorMACD macd = new IndicatorMACD();
            Operation operationMACD = macd.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);            

            IndicatorCCI cci = new IndicatorCCI();
            Operation operationCCI = cci.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);

            IndicatorRSI rsi = new IndicatorRSI();
            Operation operationRSI = rsi.GetOperation(arrayPriceOpen, arrayPriceClose, arrayPriceLow, arrayPriceHigh, arrayVolume);

            MainClass.log("CCI " + cci.result);
            MainClass.log("RSI " + rsi.result);

            if (cci.result > 0 && operationMACD == Operation.buy && rsi.result > 50 && cci.getTendency() == Tendency.high && rsi.getTendency() == Tendency.high)
            {
                return Operation.buy;
            }
            if (cci.result < 0 && operationMACD == Operation.sell && rsi.result < 50 && cci.getTendency() == Tendency.low && rsi.getTendency() == Tendency.low)
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
