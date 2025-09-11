using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Controls.Primitives;

using MaasOne.Base;
using MaasOne.YahooManaged;
using MaasOne.YahooManaged.Finance;
using MaasOne.YahooManaged.Finance.API;

using System.Collections.ObjectModel;
using StocksAnalysis.QuoteProvider;



namespace StocksAnalysis.WindowsUI
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ObservableCollection<Symbol> Symbols { get; set; }


        public ShellWindow()
        {
            InitializeComponent();
            
            InitializeVariables();
            InitializeIndexesDownload();
        }

        private void InitializeVariables()
        {
            Symbols = new ObservableCollection<Symbol>();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Clicked");

            TaskBarIcon.IndexesNotification balloon = new TaskBarIcon.IndexesNotification();
            balloon.BalloonText = "Custom Balloon";
            balloon.Indexes = Symbols;

            //show balloon and close it after 10 seconds
            IndexesNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 10000);

            ////string title = "WPF NotifyIcon";
            ////string text = "This is a standard balloon";

            ////IndexesNotifyIcon.ShowBalloonTip(title, text, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        private void InitializeIndexesDownload()
        {

            IEnumerable<string> ids = new string[] { "^DJI", "^IXIC", "^GSPC", "^AMZI", "^GDAXI", "^FCHI", "^PSI20" };

            //var idsSource = ids.ToObservable();
            

            //Subscrime 
            //var source = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(3))
            //    .Subscribe(l => DownloadIndexes());



            //IObservable<long> observable = Observable.Interval(TimeSpan.FromSeconds(3),Scheduler.CurrentThread);

            //using (observable.Subscribe(x =>
            //{
            //    QuotesBaseDownload dl = new QuotesBaseDownload();
            //    QuotesBaseResponse resp = dl.Download(ids);

            //    resp.Result.ToList().ForEach(item =>
            //        Symbols.Add(new Symbol() { Ticker = item.ID, Change = item.Change, ChangePercentage = item.ChangeInPercent })
            //        );
            //}))
            //{


            //}



            //Observable.Timer(TimeSpan.FromSeconds(0) , TimeSpan.FromSeconds(2), Scheduler.TaskPool)
            //    .Subscribe(x =>
            //    {
            //        System.Diagnostics.Debug.WriteLine("Tickes");
            //        QuotesBaseDownload dl = new QuotesBaseDownload();
            //        QuotesBaseResponse resp = dl.Download(ids);

            //        resp.Result.ToList().ForEach(item =>
            //            Symbols.Add(new Symbol() { Ticker = item.ID, Change = item.Change, ChangePercentage = item.ChangeInPercent })
            //        );
            //    });


            

           
        }

        //private void DownloadIndexes()
        //{
        //    IEnumerable<string> ids = new string[] { "^DJI", "^IXIC", "^GSPC", "^AMZI", "^GDAXI", "^FCHI", "^PSI20" };
        //    QuotesBaseDownload dl = new QuotesBaseDownload();
        //    QuotesBaseResponse resp = dl.Download(ids);

        //    resp.Result.ToList().ForEach(item =>
        //        Symbols.Add(new Symbol() { Ticker = item.ID, Change = item.Change, ChangePercentage = item.ChangeInPercent })
        //    );
        //}


    }
}
