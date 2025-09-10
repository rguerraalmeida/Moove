using Microsoft.Practices.Unity;
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

namespace Moove.Shell20.Modules.Earnings
{
    /// <summary>
    /// Interaction logic for EarningsWindow.xaml
    /// </summary>
    public partial class EarningsWindow : Window
    {
        [Dependency]
        public EarningsViewModel ViewModel
        {
            set { this.DataContext = value; }
        }
        
        public EarningsWindow()
        {
            InitializeComponent();
        }
    }
}
