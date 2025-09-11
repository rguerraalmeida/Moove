using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine.API
{
    internal class QuotesSorter : IComparer<KeyValuePair<System.DateTime, double>>
    {

        public int Compare(KeyValuePair<System.DateTime, double> x, KeyValuePair<System.DateTime, double> y)
        {
            if (x.Key > y.Key)
            {
                return 1;
            }
            else if (x.Key < y.Key)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
