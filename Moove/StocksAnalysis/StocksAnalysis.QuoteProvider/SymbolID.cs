using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.QuoteProvider
{

    /// <summary>
    /// Represents the ID of a Symbol/Quote object
    /// </summary>
    public class SymbolID
    {

        private string mTicker;
        public string Ticker
        {
            get { return mTicker; }
            set { mTicker = value; }
        }


        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

    }
}
