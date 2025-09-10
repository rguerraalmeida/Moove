using HtmlAgilityPack;
using log4net;
using Microsoft.Practices.Prism.Commands;
using Moove.Entities;
using Moove.Finance.WebProviders;
using PresentationInfrastructure;
using PresentationInfrastructure.ViewModels;
using Simple.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Moove.Heatmap
{
    public class MainWindowViewModel : SimpleViewModel
    {
        private ILog _logger = null;

        private SortableObservableCollection<HeatmapItemModel> _items;
        public SortableObservableCollection<HeatmapItemModel> Items
        {
            get { return _items; }
            set { _items = value; RaisePropertyChanged("Items"); }

        }

        private DelegateCommand _downloadCommand;
        public DelegateCommand DownloadCommand
        {
            get { return _downloadCommand; }
            set { _downloadCommand = value; }
        }

        public MainWindowViewModel()
        {
            DownloadCommand = new DelegateCommand(() => DownloadTickerData("NUGT"));
        }

        #region Warmup

        /// <summary>
        /// A distinction was made between ActiveTickers and EnabledTickers. EnabledTickers must only be used in downloadHistory and similar opeations
        /// Where we dont have the historical data to classify the ticker as Active
        /// </summary>
        /// <returns></returns>
        public List<Symbol> LoadEnabledSymbols()
        {
            try
            {
                List<Symbol> ret = new List<Symbol>();
                var db = Database.Open();
                var data = db.GetEnabledSymbols();
                foreach (var symbol in data)
                {
                    ret.Add(new Symbol()
                    {
                        Ticker = symbol.Ticker,
                        Name = symbol.Name,
                        Exchange = symbol.Exchange,
                        Sector = symbol.Sector,
                        Industry = symbol.Industry,
                        Enabled = symbol.Enabled,
                        Attempts = symbol.Attempts
                    });
                }
                return ret;
            }
            catch (Exception ex)
            {
                _logger.Error("Error loading active symbols from database", ex);
                throw;
            }
        }

        /// <summary>
        /// A distinction was made between ActiveTickers and EnabledTickers. EnabledTickers must only be used in downloadHistory and similar opeations
        /// Where we dont have the historical data to classify the ticker as Active
        /// </summary>
        /// <returns></returns>
        public List<string> LoadEnabledTickers()
        {
            return LoadEnabledSymbols().Select(x => x.Ticker.Trim()).ToList();
        }


        public void DownloadTickerData(string ticker) 
        {
            WebClient client = new WebClient();
            var pageDoc = client.DownloadString(string.Format("http://finance.yahoo.com/q?s={0}", ticker));
            HtmlDocument hdoc = new HtmlDocument();
            hdoc.LoadHtml(pageDoc);
            //HtmlNode divnode = hdoc.DocumentNode.SelectSingleNode("//div[@class='yfi_rt_quote_summary_rt_top']");
            
            // Price
            HtmlNode pricenode = hdoc.DocumentNode.SelectSingleNode("//span[@class='time_rtq_ticker']");
            //Percentage
            string percentagenodename = string.Concat("yfs_p43_", ticker.ToLower());
            string percentagenodeexpression = string.Format("//span[@id='{0}']", percentagenodename);
            HtmlNode percentagenode = hdoc.DocumentNode.SelectSingleNode(percentagenodeexpression);

            //Volume
            string volumenodename = string.Concat("yfs_v53_", ticker.ToLower());
            string volumenodeexpression = string.Format("//span[@id='{0}']", volumenodename);
            HtmlNode volumenode = hdoc.DocumentNode.SelectSingleNode(volumenodeexpression);
            //Average Volume
            HtmlNode averagevolumenode = hdoc.DocumentNode.SelectNodes("//td[@class='yfnc_tabledata1']")[10];

            var heatmapItemModel = new HeatmapItemModel() { Ticker = ticker};
            heatmapItemModel.Price = Convert.ToDouble(pricenode.InnerText);
            heatmapItemModel.ChangePercentage = Convert.ToDouble(percentagenode.InnerText.Replace("(", "").Replace(")", "").Replace("%", ""));
            heatmapItemModel.Volume = Convert.ToInt64(volumenode.InnerText.Replace(",",""));
            heatmapItemModel.AverageVolume = Convert.ToInt64(averagevolumenode.InnerText.Replace(",", ""));
        }

        #endregion
    }


    public class HeatmapItemModel : SimpleViewModel
    {
        public string Ticker { get; set; }
        public double Price { get; set; }
        public double ChangePercentage { get; set; }
        public Int64 AverageVolume { get; set; }
        public Int64 Volume { get; set; }
    }
}
