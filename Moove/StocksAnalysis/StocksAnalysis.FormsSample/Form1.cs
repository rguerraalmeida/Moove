using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reactive.Linq;

namespace StocksAnalysis.FormsSample
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            // Configure the chart
            var series = chart1.Series[0];
            series.ChartType = SeriesChartType.Stock;
            series.XValueType = ChartValueType.Time;
            var area = chart1.ChartAreas[0];
            area.Axes[0].Title = "Time";
            area.AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Seconds;
            area.AxisX.LabelStyle.Format = "T";

           

            // Test prices
            var rand = new Random();
            var prices = Observable.Generate(5d, i => i > 0, i => i + rand.NextDouble() - 0.5, i => i, i => TimeSpan.FromSeconds(1));

            // OHLC query
            var query =
                from window in prices.Window(TimeSpan.FromSeconds(0.1))
                from ohlc in window.Aggregate(new OHLC(), Accumulate)
                select ohlc;

            // Subscribe & display results
            query.ObserveOn(this).Subscribe(x => series.Points.AddXY(DateTime.Now, x.High, x.Low, x.Open, x.Close));

        }

        class OHLC
        {
            public double? Open;
            public double? High;
            public double? Low;
            public double Close;
        }

        static OHLC Accumulate(OHLC current, double price)
        {
            current.Open = current.Open ?? price;
            current.High = current.High.HasValue ? current.High > price ? current.High : price : price;
            current.Low = current.Low.HasValue ? current.Low < price ? current.Low : price : price;
            current.Close = price;
            return current;
        }
    }
}