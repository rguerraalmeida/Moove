using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using StocksAnalysis.YahooEngine.Database;
using System.Globalization;

namespace StocksAnalysis.YahooEngine.API
{
    public class HistoricalQuoteDownload
    {

        public string Ticker { get; set; }

        public double FiveDaysMovingAverage { get; set; }
        public double TwelveDaysMovingAverage { get; set; }
        public double ThirtyDaysMovingAverage { get; set; }
        public double FiftyDaysMovingAverage { get; set; }
        public double TwoHundredDaysMovingAverage { get; set; }


        public bool TwoHundredFiftyCrossed { get; set; }
        public bool FiftyThirtyCrossed { get; set; }
        public bool ThirtyTwelveCrossed { get; set; }
        public bool TwelveFiveCrossed { get; set; }

        public bool CrossOccurred { get; set; }
        public StringBuilder TrendType { get; set; }

        public bool FiftyTwoWeeksAscending { get; set; }
        public bool TwoKaySixYearAscending { get; set; }
        public bool TwoKaySevenYearAscending { get; set; }
        public bool TwoKayEigthYearAscending { get; set; }

        public List<HistoricalQuote> HistoricalQuotes { get; set; }


        public HistoricalQuoteDownload()
        {

        }


        public List<HistoricalQuote> Download(string ticker, int yearToStartFrom, string type)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            Ticker = ticker;
            HistoricalQuotes = new List<HistoricalQuote>();
            TrendType = new StringBuilder();


            using (WebClient web = new WebClient())
            {
                try
                {

                    //string data = web.DownloadString(string.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&c={1}", ticker, yearToStartFrom));
                    //http://finance.yahoo.com/q/hp?s=WU&a=01&b=19&c=2010&d=01&e=19&f=2010&g=d

                    int d, m, y;
                    d = DateTime.Now.Day;
                    m = DateTime.Now.Month;
                    y = DateTime.Now.Year;

                    // Atenção o Yahoo conta os meses como os arrays, começa em 0 em vez de 1;
                    string url = string.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&a=00&b=01&c={1}&d={2}&e={3}&f={4}&g={5}&ignore=.csv", ticker, yearToStartFrom, m - 1, d - 2, y, type);

                    string data = web.DownloadString(url).Trim();

                    data = data.Replace("r", "");

                    string[] rows = data.Split('\n');

                    //First row is headers so Ignore it
                    for (int i = 1; i < rows.Length; i++)
                    {
                        if (rows[i].Replace("\n", "").Trim() == "") continue;

                        string[] cols = rows[i].Split(',');

                        HistoricalQuote hs = new HistoricalQuote();
                        hs.Symbol = Ticker;
                        hs.Date = Convert.ToDateTime(cols[0]);
                        hs.Open = Convert.ToDouble(cols[1]);
                        hs.High = Convert.ToDouble(cols[2]);
                        hs.Low = Convert.ToDouble(cols[3]);
                        hs.Close = Convert.ToDouble(cols[4]);
                        hs.Volume = Convert.ToDouble(cols[5]);
                        hs.AdjClose = Convert.ToDouble(cols[6]);
                        hs.IntervalType = type.ToUpper();
                        HistoricalQuotes.Add(hs);
                    }

                    //CalculateMovingAverages();
                    //TwoKaySixYearAscending = AscendingTrend(new DateTime(2006,01,01));
                    //TwoKaySevenYearAscending = AscendingTrend(new DateTime(2007,01,01));
                    //TwoKayEigthYearAscending = AscendingTrend(new DateTime(2008,01,01));
                    //FiftyTwoWeeksAscending = AscendingTrend(new DateTime(2011, 01, 01));


                }
                catch (Exception)
                {


                }

                return HistoricalQuotes;
            }
        }

        public bool Load()
        {

            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

             new HistoricalQuoteDal().GetHistoricalQuotes();
             
            
            return true;
        }

        public bool Save()
        {

            return SaveDb();
        }

        public bool SaveDb()
        {

            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            return new HistoricalQuoteDal().AddHistoricalQuotes(Ticker, HistoricalQuotes);

        }

        public void CalculateMovingAverages()
        {
            var quoteValues = (from hq in HistoricalQuotes
                               select Convert.ToDouble(hq.Close)).AsEnumerable<double>();


            TwoHundredDaysMovingAverage = CalculateSimpleMovingAverage(quoteValues, 200);
            FiftyDaysMovingAverage = CalculateSimpleMovingAverage(quoteValues, 50);
            ThirtyDaysMovingAverage = CalculateSimpleMovingAverage(quoteValues, 30);
            TwelveDaysMovingAverage = CalculateSimpleMovingAverage(quoteValues, 12);
            FiveDaysMovingAverage = CalculateSimpleMovingAverage(quoteValues, 5);

            var volumeValue = (from hq in HistoricalQuotes
                               select Convert.ToDouble(hq.Volume)).ToList()[0];

            if (volumeValue <= 500000) return;

            var dictionary = (from hq in HistoricalQuotes
                              select new KeyValuePair<DateTime, double>(hq.Date, hq.Close))
                     .AsEnumerable<KeyValuePair<DateTime, double>>();

            DetectPriceMACross(dictionary, 200);
            DetectPriceMACross(dictionary, 50);
            DetectPriceMACross(dictionary, 30);

            //DetectPriceMACross(dictionary, 12);
            //DetectPriceMACross(dictionary, 5);


            ////Trend Folowing 200 Days
            // var movingAverages = CalculateMovingAverage(dictionary, 200);


            //double todayMA200Value, yesterdayMA200Value;
            //movingAverages[0].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 2), out todayMA200Value);
            //movingAverages[0].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 3), out yesterdayMA200Value);

            //double todayQuoteValue, yesterdayQuoteValue;
            //movingAverages[0].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 2), out todayQuoteValue);
            //movingAverages[0].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 3), out yesterdayQuoteValue);

            // //Bullish Trend
            //if (yesterdayMA200Value <= yesterdayQuoteValue && todayMA200Value >= todayQuoteValue)
            //{
            //    CrossOccurred = true;
            //    TrendType.AppendLine("Bullish Cross in 200 Days MA");
            //}

            ////Bearish Trend
            //if (yesterdayMA200Value >= yesterdayQuoteValue && todayMA200Value <= todayQuoteValue)
            //{
            //    CrossOccurred = true;
            //    TrendType.AppendLine("Bearish Cross in 200 Days MA");
            //}



        }

        /// <summary>
        /// Detects a cross between the quote Close price and the Moving Average for a period between today and yesterday.
        /// </summary>
        /// <param name="values">An unsorted IEnumerable of HistQuoteData.</param>
        /// <returns>The sorted dictionaries. 1) MA values; 2) Quote values.</returns>
        /// <remarks></remarks>
        void DetectPriceMACross(IEnumerable<KeyValuePair<System.DateTime, double>> values, int period)
        {

            //Trend Folowing 
            var movingAverages = CalculateMovingAverage(values, period);


            double todayMA200Value, yesterdayMA200Value;
            movingAverages[0].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 2), out todayMA200Value);
            movingAverages[0].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 3), out yesterdayMA200Value);

            double todayQuoteValue, yesterdayQuoteValue;
            movingAverages[1].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 2), out todayQuoteValue);
            movingAverages[1].TryGetValue(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 3), out yesterdayQuoteValue);

            //Bullish Trend
            if (yesterdayMA200Value <= yesterdayQuoteValue && todayMA200Value >= todayQuoteValue)
            {
                CrossOccurred = true;
                TrendType.AppendFormat("Bullish Cross in {0} Days MA \n", period);
            }

            //Bearish Trend
            if (yesterdayMA200Value >= yesterdayQuoteValue && todayMA200Value <= todayQuoteValue)
            {
                CrossOccurred = true;
                TrendType.AppendFormat("Bearish Cross in {0} Days MA \n", period);
            }

        }

        public virtual double CalculateSimpleMovingAverage(IEnumerable<double> quoteValues, int period)
        {
            return quoteValues.Take(period).Sum() / Convert.ToDouble(period);
        }


        /// <summary>
        /// Calculate values of Moving Average for historic quote values.
        /// </summary>
        /// <param name="values">An unsorted IEnumerable of HistQuoteData.</param>
        /// <returns>The sorted dictionaries. 1) MA values; 2) Quote values.</returns>
        /// <remarks></remarks>
        public virtual Dictionary<System.DateTime, double>[] CalculateMovingAverage(IEnumerable<KeyValuePair<System.DateTime, double>> values, int Period)
        {
            Dictionary<System.DateTime, double> maResult = new Dictionary<System.DateTime, double>();

            List<KeyValuePair<System.DateTime, double>> quoteValues = new List<KeyValuePair<System.DateTime, double>>(values);
            quoteValues.Sort(new QuotesSorter());


            Dictionary<System.DateTime, double> valDict = new Dictionary<System.DateTime, double>();
            foreach (KeyValuePair<System.DateTime, double> item in quoteValues)
            {
                valDict.Add(item.Key, item.Value);
            }

            if (quoteValues.Count > 0)
            {
                double ave = 0;
                for (int i = 0; i <= quoteValues.Count - 1; i++)
                {
                    ave = 0;
                    if (i + 1 - Period >= 0)
                    {
                        for (int n = i + 1 - Period; n <= i; n++)
                        {
                            ave += quoteValues[n].Value;
                        }
                        ave = ave / Period;
                    }
                    else
                    {
                        for (int n = 0; n <= i; n++)
                        {
                            ave += quoteValues[n].Value;
                        }
                        ave = ave / (i + 1);
                    }
                    maResult.Add(quoteValues[i].Key, ave);
                }
            }

            return new Dictionary<System.DateTime, double>[] {
		maResult,
		valDict
	};
        }



        //http://thequantinvestor.blogspot.com/2011/03/share-strength-indicator-how-much-bang.html
        public virtual Dictionary<System.DateTime, double>[] CalculateShareStrengthIndicator(IEnumerable<KeyValuePair<System.DateTime, double>> values, int Period)
        {
            Dictionary<System.DateTime, double> maResult = new Dictionary<System.DateTime, double>();

            List<KeyValuePair<System.DateTime, double>> quoteValues = new List<KeyValuePair<System.DateTime, double>>(values);
            quoteValues.Sort(new QuotesSorter());


            Dictionary<System.DateTime, double> valDict = new Dictionary<System.DateTime, double>();
            foreach (KeyValuePair<System.DateTime, double> item in quoteValues)
            {
                valDict.Add(item.Key, item.Value);
            }

            if (quoteValues.Count > 0)
            {
      
            }

            return new Dictionary<System.DateTime, double>[] {
		maResult,
		valDict
	};
        }

        public virtual bool AscendingTrend(DateTime StartDate)
        {
            if (this.HistoricalQuotes[0].Volume < 500000)
                return false;


            double minvalue = this.HistoricalQuotes.LastOrDefault(x => x.Date >= StartDate).Close;
            double maxvalue = this.HistoricalQuotes[0].Close;

            if (maxvalue > minvalue)
                return true;

            return false;
        }

    }
}
