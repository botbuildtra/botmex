using System;

public enum Operation
{
    buy,
    sell,
    nothing,
    allow
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
    TypeIndicator getTypeIndicator();
    Tendency getTendency();
    void setPeriod(int period);
    void setHigh(double high);
    void setLow(double low);
    void setLimit(double limit);
    double getResult();
    double getResult2();
    Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume);
}
