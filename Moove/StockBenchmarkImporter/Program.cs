using StockBenchmark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBenchmarkImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            SymbolImporter si = new SymbolImporter();
            si.Import();

            Console.ReadKey();
        }
    }
}
