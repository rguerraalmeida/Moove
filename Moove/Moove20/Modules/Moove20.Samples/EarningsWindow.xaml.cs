using MaasOne.RSS;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using Moove20.Entities;
using Moove20.WebProviders.Yahoo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Moove20.Samples
{
    /// <summary>
    /// Interaction logic for EarningsWindow.xaml
    /// </summary>
    public partial class EarningsWindow : Window
    {
        public EarningsWindow()
        {
            InitializeComponent();

            EarningsViewModel vm = new EarningsViewModel();
            DataContext = vm;

        }
    }

    public class EarningsDateResult
    {
        public DateTime EarningsDate { get; set; }
        public int CompanyCount { get; set; }
        public List<EarningsDate> Earnings { get; set; }
    }

    public class EarningsViewModel : NotificationObject
    {

        public List<EarningsDateResult> Dates { get; set; }
        //public List<MaasOne.RSS.FeedItem> News { get; set; }




        //private List<EarningsDateResult> _dates;
        //public List<EarningsDateResult> Dates
        //{
        //    get { return _dates; }
        //    set { _dates = value; RaisePropertyChanged("Dates"); }
        //}

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


        public EarningsViewModel()
        {
            var earnings = GetEarnings();
            Dates = earnings
                .GroupBy(x => x.Day)
                .Select(group => new EarningsDateResult() { EarningsDate = group.Key, CompanyCount = group.Count(), Earnings = group.ToList() }).ToList();
        }

        private List<EarningsDate> GetEarnings()
        {
            DateTime earningsDate = DateTime.Today; 
            List<EarningsDate> downloadedEarnings = new List<EarningsDate>();
            YahooEarningsByDateScrapperDownload yahooEarningsScrapperDownload = new YahooEarningsByDateScrapperDownload();

            Enumerable.Range(1, 5)
                .ToList()
                .ForEach(dayoffset =>
                {
                    earningsDate = DateTime.Today.AddDays(dayoffset * -1);
                    downloadedEarnings.AddRange(yahooEarningsScrapperDownload.Download(earningsDate));
                });

            // Download Today and Next 30 Days
            Enumerable.Range(0, 30)
                .ToList()
                .ForEach(dayoffset =>
                {
                    earningsDate = DateTime.Today.AddDays(dayoffset);
                    downloadedEarnings.AddRange(yahooEarningsScrapperDownload.Download(earningsDate));
                });

            return downloadedEarnings;
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
