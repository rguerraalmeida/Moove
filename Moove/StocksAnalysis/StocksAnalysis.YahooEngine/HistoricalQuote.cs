﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine
{
    // <summary>
    // Stores informations about one historic trading period (day, week or month).
    // </summary>
    // <remarks></remarks>
    public class HistoricalQuote
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public double AdjClose { get; set; }
        public string IntervalType { get; set; }
    }
}
