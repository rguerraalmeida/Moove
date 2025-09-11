using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine
{
    public class Quote
    {
        public QuoteData QuoteData { get; set; }
        public List<HistoricalQuote> QuoteHistory { get; set; }

    }
}
