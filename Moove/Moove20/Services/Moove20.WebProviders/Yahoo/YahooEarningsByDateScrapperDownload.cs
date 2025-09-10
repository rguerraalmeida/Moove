using HtmlAgilityPack;
using log4net;
using Moove20.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Moove20.WebProviders.Yahoo
{
    public class YahooEarningsByDateScrapperDownload
    {
        private ILog _logger = null;

        public List<EarningsDate> Download(DateTime date)
        {
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            List<EarningsDate> earnings = new List<EarningsDate>();

            try
            {
                WebClient client = new WebClient();
                var datestring = date.ToString("yyyyMMdd");
                var pageDoc = client.DownloadString(string.Format("http://biz.yahoo.com/research/earncal/{0}.html", datestring));

                HtmlDocument hdoc = new HtmlDocument();
                hdoc.LoadHtml(pageDoc);
                HtmlNode table = hdoc.DocumentNode.SelectNodes("//table[4]")[0];

                foreach (var tr in table.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes.Skip(2))
                {
                    try
                    {
                        var cols = tr.ChildNodes.Select(x => x.InnerText.Trim()).ToArray();
                        if (cols != null && cols.Count() >= 0)
                        {
                            if (cols.Count() == 1)
                                continue;
                            else if (cols.Count() >= 3 && cols.Count() <= 4)
                                earnings.Add(WebScrapYahoo3ColsEarnings(cols));
                            else if (cols.Count() >= 5 && cols.Count() <= 6)
                                earnings.Add(WebScrapYahoo6ColsEarnings(cols));
                            else if (date >= DateTime.Today)
                                earnings.Add(WebScrapYahooFutureEarnings(cols));
                            else
                                earnings.Add(WebScrapYahooPastEarnings(cols));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(string.Concat("Error parsing columns in YahooEarningsScrapperDownload downloading earnings for date ", date), ex);
                    }
                }
                earnings.ForEach(x => x.Day = date);
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Error in YahooEarningsScrapperDownload downloading earnings for date ", date), ex);
            }
            return earnings;
        }

        private EarningsDate WebScrapYahooPastEarnings(string[] cols)
        {
            return new EarningsDate()
            {
                Company = cols[0],
                Ticker = cols[1],
                WebDate = cols[2],
                //Day = date,
                Time = cols[2],
            };
        }

        private EarningsDate WebScrapYahooFutureEarnings(string[] cols)
        {
            return new EarningsDate()
            {
                Company = cols[0],
                Ticker = cols[1],
                WebDate = cols[3],
                //Day = date,
                Time = cols[3]
            };
        }

        private EarningsDate WebScrapYahoo6ColsEarnings(string[] cols)
        {
            return new EarningsDate()
            {
                Company = cols[0],
                Ticker = cols[1],
                WebDate = cols[3],
                Time = cols[3],
            };
        }

        private EarningsDate WebScrapYahoo3ColsEarnings(string[] cols)
        {
            return new EarningsDate()
            {
                Company = cols[0],
                Ticker = cols[1],
                WebDate = cols[2],
                Time = cols[2],
            };
        }
    }
}
