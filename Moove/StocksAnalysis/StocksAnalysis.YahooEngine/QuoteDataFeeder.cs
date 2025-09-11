using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine
{
    /// <summary>
    /// Provides Methods for asynchonosly get a list of quotes based on the RX Framework
    /// </summary>
    public class QuoteDataFeeder : IObservable<QuoteData>
    {
        public IDisposable Subscribe(IObserver<QuoteData> observer)
        {
            throw new NotImplementedException();
        }
    }
}
