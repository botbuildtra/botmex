using System;
using System.Collections.Generic;

public enum Operation
{
    buy,
    sell,
    nothing,
    allow,
    invertbuy,
    invertsell
};

public enum TypeIndicator
{
    Normal,
    Cross,
    Threshold
};

public enum Tendency
{
    high,
    low,
    nothing
};

public interface IIndicator
{
    String getName();
    String getTimegraph();
    TypeIndicator getTypeIndicator();
    Tendency getTendency();
    void Setup(Dictionary<string,string> cfg);
    /*void setPeriod(int period);
    void setHigh(double high);
    void setLow(double low);
    void setLimit(double limit);*/
    double getResult();
    double getResult2();
    Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume);
}
