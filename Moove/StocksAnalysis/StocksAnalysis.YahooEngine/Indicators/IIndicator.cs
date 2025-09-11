using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine.Indicators
{
    public interface IIndicator
    {
        string Name { get; }
        double ScaleMinimum { get; }
        double ScaleMaximum { get; }
        int Period { get; set; }
        bool IsRealative { get; }
    }

    public interface ISingleValueIndicator : IIndicator
    {
        Dictionary<System.DateTime, double>[] Calculate(IEnumerable<KeyValuePair<System.DateTime, double>> values);
    }

    public interface IHistQuoteIndicator : IIndicator
    {
        Dictionary<System.DateTime, double>[] Calculate(IEnumerable<HistoricalQuote> values);
    }
}
