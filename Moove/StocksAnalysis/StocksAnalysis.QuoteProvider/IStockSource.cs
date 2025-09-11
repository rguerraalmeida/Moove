using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.QuoteProvider
{
    public interface IStockSource
    {
        void Subscribe(string ID);

        event Action<Symbol> StockArrived;
    }
}
