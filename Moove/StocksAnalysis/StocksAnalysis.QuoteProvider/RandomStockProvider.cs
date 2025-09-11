using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;

namespace StocksAnalysis.QuoteProvider
{
    public class RandomQuoteSource : IStockSource
    {
        public event Action<Symbol> StockArrived;

        public void Subscribe(string Ticker)
        {
            var source = seedValues();
            source.Subscribe(x => SeedQuote(Ticker,x));
        }
        

        IObservable<double> seedValues()
        {
            var rand = new Random();

            var prices = Observable.Generate(
                5d,
                i => i > 0,
                i => i + rand.NextDouble() - 0.5,
                i => i,
                i => TimeSpan.FromSeconds(5)
            );

            return prices;
        }

        void SeedQuote(string ticker,  double value)
        {
            Symbol stock = new Symbol();
            stock.Ticker = ticker;
            stock.Last = value;

            if (StockArrived != null)
            {
                StockArrived(stock);
            }
        }
    }
}
