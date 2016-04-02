using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moove.PresentationFramework.Culture;
using Moove.Framework.Extensions;

namespace MooveUI
{
    //public class SingleIndexViewModel : NotificationObject, IDisposable
    //{
    //    /* Ticker deve ser a sigla que identifica o activo na Internet/Provider (ex: ^DJIA é o Dow Jones no YAhoo então o ticker deve se ^DJIA, se fosse na bloomberg e fosse
    //        IB00035 então seria esse o ticker) */
    //    /* Symbol deverá ser a sigla que queremos mostrar no ecrã (ex: Dow Jones seria DOW ou outro seria NAS100 ou NASDAQ100 ou seja é um descritivo curto) */

    //    public string Ticker { get; set; }
    //    public string Symbol { get; set; }
    //    public double Price { get; set; }
    //    public double Change { get; set; }
    //    public double PercentChange { get; set; }

    //    //public string TrayText { get { return string.Format("Index {0} is trading at , { 0:0.#}", Symbol, PercentChange); } }
    //    //public string TrayIconText { get { return string.Format("{0:0.#}", PercentChange); } }


    //    public void SetId(string ticker, string symbol)
    //    {
    //        Ticker = ticker;
    //        Symbol = symbol;
    //        RaisePropertyChanged(new string[] { "Ticker", "Symbol" });
    //    }

    //    public void NotifyPriceChanged()
    //    {
    //        RaisePropertyChanged(new string[] { "Price", "Change", "PercentChange" });
    //    }

    //    private string _url;
    //    private CancellationTokenSource _cancellationTokenSource;

    //    public SingleIndexViewModel(string ticker, string symbol)
    //    {
    //        SetId(ticker, symbol);

    //        var cults = CultureManager.Instance.AllCultures;

    //        _cancellationTokenSource = new CancellationTokenSource();

    //        var continuousTask = TaskRepeatExtension.Interval(TimeSpan.FromSeconds(30), () =>
    //        {
    //            try
    //            {
    //                CultureManager.Instance.ExecuteIn(CultureManager.EnglishCultureCode,
    //                () =>
    //                {
    //                    //WebClient client = new WebClient();
    //                    _url = @"http://www.investing.com/indices/" + ticker;

    //                    WebRequest request = WebRequest.Create(_url);
    //                    ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.12 (KHTML, like Gecko) Maxthon/3.0 Chrome/18.0.966.0 Safari/535.12";
    //                    WebResponse response = request.GetResponse();
    //                    Stream dataStream = response.GetResponseStream();
    //                    StreamReader reader = new StreamReader(dataStream);
    //                    string responseFromServer = reader.ReadToEnd();
    //                    reader.Close();
    //                    response.Close();

    //                    var pageDoc = responseFromServer;

    //                    HtmlDocument hdoc = new HtmlDocument();
    //                    hdoc.LoadHtml(pageDoc);

    //                    HtmlNode priceHtml = hdoc.DocumentNode.SelectNodes("//*[@id='last_last']")[0];
    //                    this.Price = Convert.ToDouble(priceHtml.InnerText);

    //                    HtmlNode changeHtml = hdoc.DocumentNode.SelectNodes("//*[@id='quotes_summary_current_data']/div/div[2]/div[1]/span[2]")[0];
    //                    this.Change = Convert.ToDouble(changeHtml.InnerText);

    //                    HtmlNode changePercentageHtml = hdoc.DocumentNode.SelectNodes("//*[@id='quotes_summary_current_data']/div/div[2]/div[1]/span[4]")[0];
    //                    this.PercentChange = Convert.ToDouble(changePercentageHtml.InnerText
    //                        .Replace("(", string.Empty)
    //                        .Replace(")", string.Empty)
    //                        .Replace("%", string.Empty)
    //                    );
    //                });

    //                NotifyPriceChanged();
    //            }
    //            catch (Exception)
    //            {
    //                //throw;
    //            }
    //        }, _cancellationTokenSource.Token);
    //    }
    //    public void Dispose()
    //    {
    //        _cancellationTokenSource.Cancel();
    //        //throw new NotImplementedException();
    //    }
    //}
}
