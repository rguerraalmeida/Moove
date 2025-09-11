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
using StocksAnalysis.YahooEngine.API;
using StocksAnalysis.YahooEngine.Database;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace StocksAnalysis.WindowsUI
{
    /// <summary>
    /// Interaction logic for DataImporterView.xaml
    /// </summary>
    public partial class DataImporterView : Window
    {
        

        public DataImporterView()
        {
            InitializeComponent();
        }

        private List<HistoricalQuoteDownload> Quotes = new List<HistoricalQuoteDownload>();
        private BackgroundWorker downloadWorker = new BackgroundWorker();

        private object _syncLock = new object();
        private int _itemCount;
        private int _processItemCount;
        
        //BackgroundWorker saveWorker = new BackgroundWorker();
       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            this.SaveOptionsComboBox.ItemsSource = Enum.GetValues(typeof(SaveOptions));
            // Set up the Background Worker Events
            downloadWorker.DoWork += downloadWorker_DoWork;
            downloadWorker.RunWorkerCompleted += downloadWorker_RunWorkerCompleted;
            downloadWorker.ProgressChanged += downloadWorker_ProgressChanged;
            downloadWorker.WorkerReportsProgress = true;

            //saveWorker.DoWork += saveWorker_DoWork;
            //saveWorker.RunWorkerCompleted += saveWorker_RunWorkerCompleted;
            //saveWorker.ProgressChanged += saveWorker_ProgressChanged;
            //saveWorker.WorkerReportsProgress = true;
        }

    


        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SaveOptionsComboBox.SelectedItem == null)
            {
                MessageBox.Show("Escolha o destino dos dados");
                return;
            }

            SaveOptions SelectedOption = (SaveOptions)this.SaveOptionsComboBox.SelectedItem;
            if (SelectedOption != SaveOptions.Database)
            {
                MessageBox.Show("Operação não suportada");
                return;
            }

            //saveWorker.RunWorkerAsync();

            ItemProgressStatus.Content = string.Format("Downloading from Yahoo ");
            StatusTextBlock.Text = string.Format("Downloading...");
            _itemCount = 0;
            _processItemCount = 0;

            downloadWorker.RunWorkerAsync();
        }

        // Worker Method
        void downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> ids = new SymbolLookupDal().GetSymbols("NASDAQ").ToList();
            //IDS = new string[]{"AAPL","GOOG", "MSFT", "DELL"};

            _itemCount = ids.Count();
 
            ids.AsParallel().ForAll(
                (item)=>
                    {
                        
                        HistoricalQuoteDownload Hist = new HistoricalQuoteDownload();
                        Hist.Download(item, 1990, "d");
                        Hist.SaveDb();

                        IncrementProcessItemCount();
                        (sender as BackgroundWorker).ReportProgress(GetProcessItemCount() / GetItemCount(), item.ToString());

                });
        }

        private void IncrementProcessItemCount ()
        {
            lock (_syncLock )
            {
                _processItemCount ++;
            }
        }

        private int GetProcessItemCount()
        {
            lock (_syncLock)
            {
                return _processItemCount;
            }

        }

        private int GetItemCount()
        {
            lock (_syncLock)
            {
                return _itemCount;
            }

        }

        // Progress Method
        void downloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //CountProgressLabel.Content = string.Format("Found {0} records", e.ProgressPercentage);
            ItemProgressStatus.Content = string.Format("Downloading Yahoo Ticker: {0}", e.UserState.ToString());
            StatusTextBlock.Text = string.Format("Downloading...");
            DownloadProgressBar.Value = (double)e.ProgressPercentage;
        }

        // Completed Method
        void downloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                StatusTextBlock.Text = "Cancelled";
            }
            else if (e.Error != null)
            {
                StatusTextBlock.Text = "Exception Thrown";
            }
            else
            {
                StatusTextBlock.Text = "Completed";
            }
        }


    


        //// Worker Method
        //void saveWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    int i = 0;
        //    foreach (var quote in Quotes)
        //    {
        //        quote.SaveDb();
        //        (sender as BackgroundWorker).ReportProgress((int)(100 * i) / Quotes.Count(), quote.Ticker);
        //    }
        //}

        //// Progress Method
        //void saveWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    ItemProgressStatus.Content = string.Format("Saving Yahoo Ticker: {0}", e.UserState.ToString());
        //    StatusTextBlock.Text = string.Format("Saving...");
        //    DownloadProgressBar.Value = (double)e.ProgressPercentage;
        //}

        //// Completed Method
        //void saveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (e.Cancelled)
        //    {
        //        StatusTextBlock.Text = "Cancelled";
        //    }
        //    else if (e.Error != null)
        //    {
        //        StatusTextBlock.Text = "Exception Thrown";
        //    }
        //    else
        //    {
        //        StatusTextBlock.Text = "Completed";
        //    }
        //}

        //private void SaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.SaveOptionsComboBox.SelectedItem == null) 
        //    {
        //        MessageBox.Show("Escolha o destino dos dados");
        //        return;
        //    }

        //    SaveOptions SelectedOption =  (SaveOptions)this.SaveOptionsComboBox.SelectedItem;
        //    if (SelectedOption != SaveOptions.Database)
        //    {
        //        MessageBox.Show("Operação não suportada");
        //        return;
        //    }

        //    if (Quotes == null || Quotes.Count() <= 0) 
        //    {
        //        MessageBox.Show("Não existem dados. Por favor faça download dos dados");
        //        return;
        //    }

        //    saveWorker.RunWorkerAsync();
        //}
    }
}
