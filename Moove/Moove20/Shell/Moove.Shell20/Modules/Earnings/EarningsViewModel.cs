using log4net;
using MaasOne.RSS;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Moove.Shell20.Infrastructure;
using Moove20.Entities;
using Moove20.WebProviders.Yahoo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moove.Shell20.Modules.Earnings
{
    public class EarningsDateResult
    {
        public DateTime EarningsDate { get; set; }
        public int CompanyCount { get; set; }
        public List<EarningsDate> Earnings { get; set; }
    }

    public class EarningsViewModel : SimpleViewModel
    {
        private Object _lock = new Object();
        private ILog logger = null;

        private List<EarningsDateResult> _dates;
        public List<EarningsDateResult> Dates
        {
            get { return _dates; }
            set { _dates = value; RaisePropertyChanged("Dates"); }
        }

        private List<MaasOne.RSS.FeedItem> _news;
        public List<MaasOne.RSS.FeedItem> News
        {
            get { return _news; }
            set { _news = value; RaisePropertyChanged("News"); }
        }

        private EarningsDate _selectedCompanyDateResult;
        public EarningsDate SelectedCompanyDateResult
        {
            get { return _selectedCompanyDateResult; }
            set
            {
                _selectedCompanyDateResult = value;
                if (_selectedCompanyDateResult != null)
                {
                    GetNewsForCompany(_selectedCompanyDateResult.Ticker);
                }
                RaisePropertyChanged("SelectedCompanyDateResult");
            }
        }


        public EarningsViewModel(IUnityContainer container, IEventAggregator eventAggregator)
            : base(eventAggregator, container)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            var earnings = DownloadEarningsDates();
            Dates = earnings
                .GroupBy(x => x.Day)
                .Select(group => new EarningsDateResult() { EarningsDate = group.Key, CompanyCount = group.Count(), Earnings = group.ToList() }).ToList();
            //items.GroupBy(item => item.Order.Customer)
            // .Select(group => new { Customer = group.Key, Items = group.ToList() })
            // .ToList() 


            //Must Receive NewsFeed from NewsService
            //_eventAggregator
            //    .GetEvent<HubDataReceivedEvent<List<StockInfo>>>()
            //    .Subscribe(a =>
            //    {
            //        StocksInfoUpdated(a.Data);
            //    });
        }

        private List<EarningsDate> DownloadEarningsDates()
        {
            try
            {
                DateTime earningsDate = DateTime.Today;
                YahooEarningsByDateScrapperDownload yahooEarningsScrapperDownload = new YahooEarningsByDateScrapperDownload();

                List<EarningsDate> downloadedEarnings = new List<EarningsDate>();

                // Download Previous 22 Days
                Enumerable.Range(1, 8)
                    .ToList()
                    .ForEach(dayoffset =>
                    {
                        try
                        {
                            earningsDate = DateTime.Today.AddDays(dayoffset * -1);
                            downloadedEarnings.AddRange(yahooEarningsScrapperDownload.Download(earningsDate));
                        }
                        catch (Exception ex)
                        {
                            logger.Error(string.Format("Error downloading earnings for date {0}", earningsDate), ex);
                        }
                    });

                // Download Today and Next 30 Days
                Enumerable.Range(0, 8)
                    .ToList()
                    .ForEach(dayoffset =>
                    {
                        try
                        {
                            earningsDate = DateTime.Today.AddDays(dayoffset);
                            downloadedEarnings.AddRange(yahooEarningsScrapperDownload.Download(earningsDate));

                        }
                        catch (Exception ex)
                        {
                            logger.Error(string.Format("Error downloading earnings for date {0}", earningsDate), ex);
                        }
                    });

                return downloadedEarnings;
                ////downloadedEarnings.AddRange(yahooEarningsScrapperDownload.Download(DateTime.Today));

                //if (downloadedEarnings == null || downloadedEarnings.Count() == 0)
                //{
                //    logger.Info(string.Format("No earnings for today date.", _name));
                //}
                //else
                //{
                //    logger.Info(string.Format("{0} loading earnings", _name));
                //    List<EarningsDate> earnings = databaseManager.LoadEarnings();

                //    logger.Info(string.Format("{0} loading symbols", _name));
                //    var tickers = databaseManager.LoadSymbols();

                //    logger.Info(string.Format("{0} excluding non existent tickers", _name));
                //    var existentTickersEarnings = downloadedEarnings.Where(f => tickers.Exists(s => f.Ticker == s.Ticker)).ToList();

                //    logger.Info(string.Format("{0} updating existent earnings.", _name));
                //    var existentEarnings = existentTickersEarnings.Intersect(earnings).ToList();
                //    existentEarnings.ForEach(earn => databaseManager.UpdateEarnings(earn));

                //    logger.Info(string.Format("{0} excluding existent earnings .", _name));
                //    List<EarningsDate> definitiveEarnings = existentTickersEarnings.Except(earnings).ToList();

                //    logger.Info(string.Format("{0} bulk adding new earnings", _name));
                //    databaseManager.BulkAddEarningsData(definitiveEarnings);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("DownloadEarningsDates Error."), ex);
            }

            throw new Exception("DEu merda nos earnings");
        }


        private void GetNewsForCompany(string symbol)
        {
            FeedDownload feedDownload = new FeedDownload();
            var result = feedDownload.Download(
                string.Format("http://feeds.finance.yahoo.com/rss/2.0/headline?s={0}&region=US&lang=en-US", symbol));

            News = result.Result.Feeds[0].Items.Select(x => new FeedItem()
            {
                Author = x.Author,
                Source = x.Source,
                GUID = x.GUID,
                Enclosure = x.Enclosure,
                Title = x.Title,
                PublishDate = x.PublishDate,
                Category = x.Category,
                Description = x.Description,
                Link = x.Link,
            }).ToList();
        }
    }
}
