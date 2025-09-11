using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.QuoteProvider
{
    /// <summary>
    /// Represents a quote object, can be used to store value of an index, stock, option, future, equities.
    /// </summary>
    public class Quote
    {
        /// <summary>
        /// Value of the last trade price
        /// </summary>
        public double Last { get; set; }
        
        /// <summary>
        /// Value of the last bid price
        /// </summary>
        public double Bid { get; set; }
        
        /// <summary>
        /// Value of the last ask price
        /// </summary>
        public double Ask { get; set; }

        /// <summary>
        /// Volume traded so far for the current day
        /// </summary>
        public long Volume { get; set; }

        /// <summary>
        /// Average Volume traded by this quote for a range of 10-days
        /// </summary>
        public long AvgVolume { get; set; }
        
        /// <summary>
        /// Value of the quote at market opening
        /// </summary>
        public double Open { get; set; }
        
        /// <summary>
        /// Value of the quote at market close, value for previous day if market is still open
        /// </summary>
        public double Close { get; set; }
        
        /// <summary>
        /// Higher value of the quote for the current day
        /// </summary>
        public double High { get; set; }
        
        /// <summary>
        /// Lower value of the quote for the current day
        /// </summary>
        public double Low { get; set; }
    }
}
