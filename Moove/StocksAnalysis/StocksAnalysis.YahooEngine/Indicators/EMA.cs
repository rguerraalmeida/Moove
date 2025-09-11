using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine.Indicators
{
    /// <summary>
    /// Exponential Moving Average Indicator. Inherits from Moving Average(MA).
    /// </summary>
    /// <remarks></remarks>
    public class EMA : MA
    {

        public override string Name
        {
            get { return "Exponential Moving Average"; }
        }

        /// <summary>
        /// Calculates values of Exponential Moving Average for historic quote values.
        /// </summary>
        /// <param name="values">An unsorted IEnumerable of HistQuoteData.</param>
        /// <returns>The sorted dictionaries. 1) EMA values; 2) MA values; 3) Quote values.</returns>
        /// <remarks></remarks>
        public override Dictionary<System.DateTime, double>[] Calculate(IEnumerable<KeyValuePair<System.DateTime, double>> values)
        {
            Dictionary<System.DateTime, double> emaResult = new Dictionary<System.DateTime, double>();
            dynamic baseResults = base.Calculate(values);
            Dictionary<System.DateTime, double> maResult = baseResults(0);

            List<KeyValuePair<System.DateTime, double>> histQuoteCloses = new List<KeyValuePair<System.DateTime, double>>(baseResults(1));

            double exponent = 0;
            System.DateTime d = default(System.DateTime);
            if (histQuoteCloses.Count > 1)
            {
                emaResult.Add(histQuoteCloses[0].Key, histQuoteCloses[0].Value);
                for (int i = 1; i <= histQuoteCloses.Count - 1; i++)
                {
                    exponent = 2 / (Math.Min(this.Period, i + 1) + 1);
                    d = histQuoteCloses[i].Key;
                    emaResult.Add(d, (exponent * histQuoteCloses[i].Value) + ((1 - exponent) * emaResult[histQuoteCloses[i - 1].Key]));
                }
            }

            return new Dictionary<System.DateTime, double>[] {
			emaResult,
			maResult,
			baseResults(1)
		};
        }

        public override string ToString()
        {
            return this.Name + " " + this.Period;
        }
    }
}
