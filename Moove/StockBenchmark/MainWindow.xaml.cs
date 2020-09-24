using Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockBenchmark
{
    public delegate void UpdateCallback(string message);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Benchmark bench = new Benchmark();
        ObservableCollection<string> logconsole = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            ConsoleLogListbox.ItemsSource = logconsole;
            SimpleLogger.Log += (string s) => { Dispatcher.Invoke(() => logconsole.Insert(0,s)); };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bench.Start();
        }

    }
}
