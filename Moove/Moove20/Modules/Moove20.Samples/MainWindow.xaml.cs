using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Moove20.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder stringBuilder = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
            this.WindowsList.ItemsSource = WindowNames;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (WindowsList.SelectedItem != null)
            {
                var window = WindowsList.SelectedItem;
                ShowWindowByName(window.ToString(), false);
            }

            DateTime dt = DateTime.UtcNow;
            //stringBuilder.AppendLine(dt.ToString());
        }

        public static IEnumerable<string> WindowNames
        {
            get
            {
                IEnumerable<string> ieWindowNames = null;
                Assembly asm = Assembly.GetExecutingAssembly();

                ieWindowNames =
                   from types in asm.GetTypes()
                   where types.BaseType.Name == "Window" || types.BaseType.Name == "MetroWindow" 
                   orderby types.Name
                   select types.Name;

                return ieWindowNames;
            }
        }

        public bool? ShowWindowByName(string strWindowName, bool fShowDialog)
        {
            if (string.IsNullOrEmpty(strWindowName)) return null;

            Assembly asm = Assembly.GetExecutingAssembly();
            string strFullyQualifiedName = asm.GetName().Name + "." + strWindowName;
            object obj = asm.CreateInstance(strFullyQualifiedName);

            Window win = obj as Window;
            if (win == null) return null;

            if (fShowDialog)
            {
                win.ShowDialog();
                return win.DialogResult;
            }
            else
            {
                win.Show();
                return null;
            }
        }
    }
}
