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

namespace MooveUI
{
    /// <summary>
    /// Interaction logic for MarketIndexesWindow.xaml
    /// </summary>
    public partial class MarketIndexesWindow : Window
    {
        public MarketIndexesWindow()
        {
            InitializeComponent();
            //Should use prism but wahtever, im not going to mvvm test ui and shi......
            MarketIndexesViewModel vm = new MarketIndexesViewModel();
            DataContext = vm;

            //NotifyIcon ni = new NotifyIcon();
            ////ni.BalloonTipText = "BaloonTipText";
            ////ni.Text = "Text";
            //ni.Visible = true;
            ////ni.ShowBalloonTip(15500);
            ////ni.Icon = new System.Drawing.Icon("Icons/Software_win7.ico");
            //ni.Icon = GetIcon("D:0");

            TaskbarIcon tbi = new TaskbarIcon();
            ////tbi.Icon = new System.Drawing.Icon("Icons/Software_win7.ico");
            ////tbi.IconSource = RenderBitmap("Dax:0.90%");
            ////tbi.Icon = new System.Drawing.Icon(ConvertBitmapToMemoryStream(UpdateBitmap("Dax:0.90%"))); 
            tbi.Icon = GetIcon("122");
            
            //tbi.ToolTipText = "";



            //NotifyIcon ni2 = new NotifyIcon();
            //ni2.Icon = GetIcon(".95");
            //ni2.Visible = true;
        }

        public static System.Drawing.Icon GetIcon(string text)
        {
            //Create bitmap, kind of canvas
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(16, 16);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            //System.Drawing.Icon icon = new System.Drawing.Icon(@"Icons/trayicon.ico");
            //System.Drawing.SolidBrush backgroundBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            //graphics.DrawRectangle( new System.Drawing.Pen(backgroundBrush,1), new System.Drawing.Rectangle(0,0,16,16));

            System.Drawing.Font drawFont = new System.Drawing.Font("Arial",8, System.Drawing.FontStyle.Regular);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.GreenYellow);


            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            //graphics.DrawIcon(icon, 0, 0);
            //graphics.DrawString(text, drawFont, drawBrush, 1, 2);
            graphics.DrawString(text, drawFont, drawBrush, 0, 0);

            //To Save icon to disk
            //bitmap.Save("icon.ico", System.Drawing.Imaging.ImageFormat.Icon);

            System.Drawing.Icon createdIcon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());

            drawFont.Dispose();
            drawBrush.Dispose();
            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }
    }
}
