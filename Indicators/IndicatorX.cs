﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class IndicatorX : IndicatorBase, IIndicator
    {

        public double high;
        public double low;
        public double limit;
        public string timeGraph = MainClass.timeGraph;

        public IndicatorX()
        {
            this.indicator = this;
        }

        public void Setup(Dictionary<string, string> cfg)
        {
            if (cfg.ContainsKey("high"))
                setHigh(int.Parse(cfg["high"]));

            if (cfg.ContainsKey("low"))
                setLow(int.Parse(cfg["low"]));

            if (cfg.ContainsKey("period"))
                setPeriod(int.Parse(cfg["period"]));

            if (cfg.ContainsKey("timegraph") && (cfg["timegraph"].Trim() == "1m" || cfg["timegraph"].Trim() == "5m" || cfg["timegraph"].Trim() == "1h"))
                timeGraph = cfg["timegraph"].Trim();
        }

        public string getName()
        {
            return "MACD.RSI";
        }
        
        public string getTimegraph()
        {
            return timeGraph;
        }

        public Operation GetOperation(double[] arrayPriceOpen, double[] arrayPriceClose, double[] arrayPriceLow, double[] arrayPriceHigh, double[] arrayVolume)
        {
            try
            {
                double[] arrayresultTA = new double[arrayPriceClose.Length];
                int outBegidx, outNbElement;
                double[] macdSignal = new double[arrayPriceClose.Length];
                double[] macdHist = new double[arrayPriceClose.Length];
                //TicTacTec.TA.Library.Core.Macd(0, arrayPriceClose.Length - 1, arrayPriceClose, 17, 72, 34, out outBegidx, out outNbElement, arrayresultTA, macdSignal, macdHist);
                TicTacTec.TA.Library.Core.Macd(0, arrayPriceClose.Length - 1, arrayPriceClose, 12, 26, 9, out outBegidx, out outNbElement, arrayresultTA, macdSignal, macdHist);
                double macd = arrayresultTA[outNbElement - 1];
                double signal = macdSignal[outNbElement - 1];
                double macdHistory = macdHist[outNbElement - 1];
                this.result = macdHistory;
                this.result2 = signal;
                if (macdHist[outNbElement - 1] > 0 && macdHist[outNbElement - 2] < 0)
                {
                    double[] arrayresultRSI = new double[arrayPriceClose.Length];
                    TicTacTec.TA.Library.Core.Rsi(0, arrayPriceClose.Length - 1, arrayPriceClose, 14, out outBegidx, out outNbElement, arrayresultRSI);
                    this.result = arrayresultRSI[outNbElement - 1];
                    Console.WriteLine("RSI: " + arrayresultRSI[outNbElement - 1]);
                    if (arrayresultRSI[outNbElement - 1] >= 30)
                        return Operation.buy;
                }
                if (macdHist[outNbElement - 1] < 0 && macdHist[outNbElement - 2] > 0)
                {
                    double[] arrayresultRSI = new double[arrayPriceClose.Length];
                    TicTacTec.TA.Library.Core.Rsi(0, arrayPriceClose.Length - 1, arrayPriceClose, 14, out outBegidx, out outNbElement, arrayresultRSI);
                    this.result = arrayresultRSI[outNbElement - 1];
                    Console.WriteLine("RSI: " + arrayresultRSI[outNbElement - 1]);
                    if (arrayresultRSI[outNbElement - 1] <= 70)
                        return Operation.sell;
                }
                return Operation.nothing;
            }
            catch
            {
                return Operation.nothing;
            }
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

    public TypeIndicator getTypeIndicator()
    {
        return TypeIndicator.Normal;
    }

    public void setPeriod(int period)
    {
        
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
