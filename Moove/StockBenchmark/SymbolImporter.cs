using Simple.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBenchmark
{
    public class SymbolImporter
    {
        /// host: db4free.net
        /// db: moovebenchmark
        /// user: mbadmin
        /// senha: zezito
        
        private char[] badchars = new char[] { '!', '@', '#', '$', '%', '\u005F', '-', '\u005E', '/' };
        
        public void Import()
        {
            try
            {
                Console.WriteLine("Opening connection to database");
                dynamic database = Database.OpenConnection(AppParameters.CNNSTRING);
                
                Console.WriteLine("Deleteing all records from 'symbol' table");
                database.Symbol.DeleteAll();       
                
                Console.WriteLine("Loading symbols and filtering odd tickers");
                List<NasdaqSymbol> nasdaqSymbols = ExcludeOddTicker(LoadSymbolsFromNasdaq());
                
                Console.WriteLine("Done loading and filtering symbols");
                nasdaqSymbols.ForEach(symbol => {
                    try
                    {
                        Console.WriteLine(string.Format("Inserting symbol:{0}-{1}", symbol.Ticker, symbol.Name));
                        database.Symbol.Insert(symbol);
                        Console.WriteLine("Done inserting symbol");
                    }
                    catch (Exception ex)
                    {
                        if (null != symbol && string.IsNullOrWhiteSpace(symbol.Ticker) && string.IsNullOrWhiteSpace(symbol.Name))
                        {
                            Console.WriteLine(string.Format("Error inserting symbol:{0}-{1};err:{2}", symbol.Ticker, symbol.Name, ex.Message));
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Error inserting symbol; err:{0}", ex.Message));
                        }
                    }
                });  
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("Uh oh.",exception.Message));
                //throw;
            }
        }

        public static List<NasdaqSymbol> LoadSymbolsFromNasdaq()
        {
            List<NasdaqSymbol> list = (
                from line in File.ReadLines(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, string.Concat("NASDAQ", ".csv"))).Skip<string>(1)
                select line.Split(new char[] { ',' }) into line
                select new NasdaqSymbol()
                {
                    Ticker = line[0].Replace("\"", "").Trim(),
                    Name = line[1].Replace("\"", "").Trim(),
                    Exchange = "NASDAQ"
                }).ToList<NasdaqSymbol>();
            List<NasdaqSymbol> nasdaqSymbols = (
                from line in File.ReadLines(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, string.Concat("NYSE", ".csv"))).Skip<string>(1)
                select line.Split(new char[] { ',' }) into line
                select new NasdaqSymbol()
                {
                    Ticker = line[0].Replace("\"", "").Trim(),
                    Name = line[1].Replace("\"", "").Trim(),
                    Exchange = "NYSE"
                }).ToList<NasdaqSymbol>();
            List<NasdaqSymbol> list1 = (
                from line in File.ReadLines(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, string.Concat("AMEX", ".csv"))).Skip<string>(1)
                select line.Split(new char[] { ',' }) into line
                select new NasdaqSymbol()
                {
                    Ticker = line[0].Replace("\"", "").Trim(),
                    Name = line[1].Replace("\"", "").Trim(),
                    Exchange = "AMEX"
                }).ToList<NasdaqSymbol>();
            List<NasdaqSymbol> nasdaqSymbols1 = list.Union<NasdaqSymbol>(nasdaqSymbols).Union<NasdaqSymbol>(list1).ToList<NasdaqSymbol>();
            return nasdaqSymbols1;
        }

        private List<string> ExcludeOddTicker(List<string> tickers)
        {
            return (
                from x in tickers
                where x.IndexOfAny(this.badchars) == -1
                select x).ToList<string>();
        }
        private List<NasdaqSymbol> ExcludeOddTicker(List<NasdaqSymbol> tickers)
        {
            return (
                from x in tickers
                where x.Ticker.IndexOfAny(badchars) == -1
                select x).ToList<NasdaqSymbol>();
        }
    }
}
