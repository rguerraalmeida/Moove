using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine
{
    public class QuoteData
    {

        public double LastTrade { get; set; }
        public double Change { get; set; }
        public double ChangePercent { get; set; }
        public double PreviousClose { get; set; }
        public double Open { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public long Volume { get; set; }
        public long AvgVolume { get; set; }
        public string MarketCap { get; set; }
        public string PE { get; set; }
        public double EPS { get; set; }

        public string DaysRange { get; set; }
        public string FiftyTwoWeekRange { get; set; }



    }
}
