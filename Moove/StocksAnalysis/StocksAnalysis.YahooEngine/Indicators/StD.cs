using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine.Indicators
{
    /// <summary>
    /// Standard Deviation Indicator
    /// </summary>
    /// <remarks></remarks>
    public class StD : ISingleValueIndicator
    {

        public virtual string Name
        {
            get { return "Standard Deviation"; }
        }

        public virtual bool IsRealative
        {
            get { return true; }
        }

        public double ScaleMaximum
        {
            get { return double.PositiveInfinity; }
        }

        public double ScaleMinimum
        {
            get { return 0; }
        }

        public int Period
        {
            get { return mMA.Period; }
            set { mMA.Period = value; }
        }
        public bool PopulationStandardDeviation { get; set; }



        private MA mMA = new MA();
        /// <summary>
        /// Calculates values of Standard Deviation for historic quote values.
        /// </summary>
        /// <param name="values">An unsorted IEnumerable of HistQuoteData.</param>
        /// <returns>The sorted dictionaries. 1) StD values; 2) MA values; 3) Quote values.</returns>
        /// <remarks></remarks>
        public virtual Dictionary<System.DateTime, double>[] Calculate(IEnumerable<KeyValuePair<System.DateTime, double>> values)
        {
            Dictionary<System.DateTime, double> stdResult = new Dictionary<System.DateTime, double>();

            dynamic baseResults = mMA.Calculate(values);
            Dictionary<System.DateTime, double> maResult = baseResults(0);
            List<KeyValuePair<System.DateTime, double>> histQuotes = new List<KeyValuePair<System.DateTime, double>>(baseResults(1));

            if (histQuotes.Count > 0)
            {
                double tempResult = 0;
                stdResult.Add(histQuotes[0].Key, 0);
                for (int i = 1; i <= histQuotes.Count - 1; i++)
                {
                    tempResult = 0;
                    if (i >= mMA.Period - 1)
                    {
                        for (int n = i - mMA.Period + 1; n <= i; n++)
                        {
                            tempResult += Math.Pow((histQuotes[n].Value - maResult[histQuotes[i].Key]), 2);
                        }
                        tempResult /= (mMA.Period + Convert.ToInt32((this.PopulationStandardDeviation ? 0 : -1)));
                    }
                    else
                    {
                        for (int n = 0; n <= i; n++)
                        {
                            tempResult += Math.Pow((histQuotes[n].Value - maResult[histQuotes[i].Key]), 2);
                        }
                        tempResult /= (i + Convert.ToInt32((this.PopulationStandardDeviation ? 1 : 0)));
                    }
                    tempResult = Math.Sqrt(tempResult);
                    stdResult.Add(histQuotes[i].Key, tempResult);
                }
            }

            return new Dictionary<System.DateTime, double>[] {
			stdResult,
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
