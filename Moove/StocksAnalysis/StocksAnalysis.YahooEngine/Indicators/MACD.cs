using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.YahooEngine.Indicators
{
    public class MACD : EMA
    {

        public override string Name
        {
            get { return "Moving Average Convergence/Divergence"; }
        }

        public override bool IsRealative
        {
            get { return true; }
        }

        public int PeriodFast { get; set; }
        public int PeriodSlow { get; set; }


        public MACD()
        {
            base.Period = 9;
        }


        public override System.Collections.Generic.Dictionary<System.DateTime, double>[] Calculate(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.DateTime, double>> values)
        {
            int basePeriod = base.Period;
            Dictionary<System.DateTime, double> macdValues = new Dictionary<System.DateTime, double>();
            //Dim signalLineValues As New Dictionary(Of Date, Double)

            base.Period = this.PeriodFast;
            dynamic ema12values = base.Calculate(values)[0];

            base.Period = this.PeriodSlow;
            dynamic ema26values = base.Calculate(values)[0];

            List<KeyValuePair<System.DateTime, double>> closeValues = new List<KeyValuePair<System.DateTime, double>>(values);

            System.DateTime d = default(System.DateTime);
            for (int i = 0; i <= closeValues.Count - 1; i++)
            {
                d = closeValues[i].Key;
                macdValues.Add(d, ema12values(d) - ema26values(d));
            }

            base.Period = basePeriod;
            dynamic ema9values = base.Calculate(macdValues)[0];

            return new Dictionary<System.DateTime, double>[] {
			macdValues,
			ema9values
		};
        }

        public override string ToString()
        {
            return this.Name + " " + this.Period;
        }

    }
}
