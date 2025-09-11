using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
    public class Quote
    {
        /// <summary>
        /// Ticker
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the last trade price
        /// </summary>
        public double Last { get; set; }

        /// <summary>
        /// Value of the change from previous close
        /// </summary>
        public double Change { get; set; }

        /// <summary>
        /// Value of the quote at market opening
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// Value of the quote at market close, value for previous day if market is still open
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// Volume traded so far for the current day
        /// </summary>
        public long Volume { get; set; }

        /// <summary>
        /// Average Volume traded by this quote for a range of 10-days
        /// </summary>
        public long AvgVolume { get; set; }
    }
}
