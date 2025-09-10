using HtmlAgilityPack;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Moove20.Samples
{
    /// <summary>
    /// Interaction logic for MarketIndexesWindow.xaml
    /// </summary>
    public partial class MarketIndexesWindow : Window
    {
        public MarketIndexesWindow()
        {
            InitializeComponent();

            MarketIndexesViewModel vm = new MarketIndexesViewModel();
            DataContext = vm;

            //List<MarketIndexDownload> euItems = new List<MarketIndexDownload>();
            //euItems.Add(new MarketIndexDownload("us-spx-500", "S&P500"));
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");
            ////euItems.Add("USA_EU_ASIA_MIXED");

            //this.listBox3.ItemsSource = euItems;

            //List<string> sectorsItems = new List<string>();
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");
            //sectorsItems.Add("Sector");

            //this.listBox2.ItemsSource = sectorsItems;
        }



        //List<Object> theobjects;
        //private void listBox3_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    ListBox ctrl = (ListBox)sender;
        //    var t = ctrl.InputHitTest(e.GetPosition(ctrl));
        //    var tc = (FrameworkElement)t;
        //    var obj = tc.DataContext;

        //    if (obj == null) return;
        //    theobjects = new List<Object>();

        //    foreach (var o in ctrl.SelectedItems)
        //    {
        //        theobjects.Add(o);
        //    }

        //    if (ctrl.SelectedItems.Contains(obj))
        //    {
        //        theobjects.Remove(obj);
        //    }
        //    else
        //    {
        //        theobjects.Add(obj);
        //    }
        //    e.Handled = false;
           
        //}
    
    }
    
    public class MarketIndexesViewModel : NotificationObject
    {
        public ObservableCollection<SingleIndexViewModel> MarketIndexesColletion { get; set; }

        public MarketIndexesViewModel()
        {
            MarketIndexesColletion = new ObservableCollection<SingleIndexViewModel>();

            //EU
            MarketIndexesColletion.Add(new SingleIndexViewModel("germany-30", "DAX"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("spain-35", "IBEX"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("psi-20", "PSI 20"));
           
            
            //USA
            MarketIndexesColletion.Add(new SingleIndexViewModel("us-30", "Dow Jones"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("nasdaq-composite", "Nasdaq"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("us-spx-500", "SP 500"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("nq-100", "Nasdaq 100"));

            //EU
            MarketIndexesColletion.Add(new SingleIndexViewModel("france-40", "CAC"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("uk-100", "FTSE UK"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("it-mib-40", "ITA MIB"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("eu-stoxx50", "EU-Stoxx50"));

            //RUSSIA
            MarketIndexesColletion.Add(new SingleIndexViewModel("mcx", "RUS MICEX"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("rtsi", "RUS RTSI"));
        }
    }

    public class SingleIndexViewModel : NotificationObject, IDisposable
    {
        /* Ticker deve ser a sigla que identifica o activo na Internet/Provider (ex: ^DJIA é o Dow Jones no YAhoo então o ticker deve se ^DJIA, se fosse na bloomber e fosse
            IB00035 então seria esse o ticker) */
        /* Symbol deverá ser a sigla que queremos mostrar no ecrã (ex: Dow Jones seria DOW ou outro seria NAS100 ou NASDAQ100 ou seja é um descritivo curto) */

        public string Ticker { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double Change { get; set; }
        public double PercentChange { get; set; }

        public void SetId(string ticker, string symbol)
        {
            Ticker = ticker;
            Symbol = symbol;
            RaisePropertyChanged(new string[] { "Ticker", "Symbol" });
        }

        public void NotifyPriceChanged()
        {
            RaisePropertyChanged(new string[] { "Price", "Change", "PercentChange" });
        }

        private string _url;
        private CancellationTokenSource _cancellationTokenSource;

        public SingleIndexViewModel(string ticker, string symbol)
        {
            SetId(ticker, symbol);

            _cancellationTokenSource = new CancellationTokenSource();
            var continuousTask = TaskRepeat.Interval(TimeSpan.FromSeconds(30), () =>
                {
                    try
                    {
                        WebClient client = new WebClient();
                        _url = @"http://www.investing.com/indices/" + ticker;

                        WebRequest request = WebRequest.Create(_url);
                        ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.12 (KHTML, like Gecko) Maxthon/3.0 Chrome/18.0.966.0 Safari/535.12";
                        WebResponse response = request.GetResponse();
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        reader.Close();
                        response.Close();

                        var pageDoc = responseFromServer;

                        HtmlDocument hdoc = new HtmlDocument();
                        hdoc.LoadHtml(pageDoc);
                        
                        HtmlNode priceHtml = hdoc.DocumentNode.SelectNodes("//*[@id='last_last']")[0];
                        this.Price = Convert.ToDouble(priceHtml.InnerText);

                        HtmlNode changeHtml = hdoc.DocumentNode.SelectNodes("//*[@id='quotes_summary_current_data']/div/div[2]/div[1]/span[2]")[0];
                        this.Change = Convert.ToDouble(changeHtml.InnerText);

                        HtmlNode changePercentageHtml = hdoc.DocumentNode.SelectNodes("//*[@id='quotes_summary_current_data']/div/div[2]/div[1]/span[4]")[0];
                        this.PercentChange = Convert.ToDouble(changePercentageHtml.InnerText
                            .Replace("(", string.Empty)
                            .Replace(")", string.Empty)
                            .Replace("%", string.Empty)
                        );

                        NotifyPriceChanged();
                    }
                    catch (Exception)
                    {
                        //throw;
                    }


                }, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            //throw new NotImplementedException();
        }
    }

    //[ValueConversion(typeof(double), typeof(Brush))]
    public class ValueToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            double convertedValue =  (double)value;

            try
            {
                if (convertedValue.Between(-0.1, 0.1, true))
                {
                    return Application.Current.FindResource("IndexUnchanged");
                }

                //negative values
                if (convertedValue.Between(-1, -0.1))
                {
                    return Application.Current.FindResource("IndexNegativeLowChange");
                }

                if (convertedValue <= -1)
                {
                    return Application.Current.FindResource("IndexNegativeHighChange");
                }


                if (convertedValue.Between(0.1, 1))
                {
                   return Application.Current.FindResource("IndexPositiveLowChange");
                }

                if (convertedValue >= 1)
                {
                    return Application.Current.FindResource("IndexPositiveHighChange");
                }

                return new SolidColorBrush(Colors.Yellow); 
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Yellow); 
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}