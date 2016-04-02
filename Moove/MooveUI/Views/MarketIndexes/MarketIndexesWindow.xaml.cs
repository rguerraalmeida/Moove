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

using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;
using System.Globalization;
using System.IO;
using MooveUI.Views.TaskbarIcon;

namespace MooveUI.Views.MarketIndexes
{
    /// <summary>
    /// Interaction logic for MarketIndexesWindow.xaml
    /// </summary>
    public partial class MarketIndexesWindow : Window
    {
        public MarketIndexesWindow()
        {
            InitializeComponent();
            
            //Should use prism but wahtever, im not going to mvvm test ui and stuff
            MarketIndexesViewModel vm = new MarketIndexesViewModel();
            DataContext = vm;

            //TaskbarIconViewModel vm2 = new TaskbarIconViewModel();

            //TaskbarIcon tbi = new TaskbarIcon();
            //tbi.Icon = Lib.IconGenerator.CreateIcon("D1.2");

            //var t1 = new ToolbarIcon.ToolbarIconWindow();
            //t1.Show();

            //var t2 = new ToolbarIcon.ToolbarIconWindow();
            //t2.Show();

            //var t3 = new ToolbarIcon.ToolbarIconWindow();
            //t3.Show();


            //System.Windows.Forms.NotifyIcon icon = new System.Windows.Forms.NotifyIcon();
            //icon.Icon = Lib.IconGenerator.CreateIcon("D1.2");
            
        }
    }
}
