using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Moove.PresentationFramework.Icons
{
    public static class IconGenerator
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }

        public static System.Drawing.Icon CreateIcon(string text)
        {
            //Create bitmap, kind of canvas
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(16, 16);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            //System.Drawing.Icon icon = new System.Drawing.Icon(@"Icons/trayicon.ico");
            //System.Drawing.SolidBrush backgroundBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            //graphics.DrawRectangle( new System.Drawing.Pen(backgroundBrush,1), new System.Drawing.Rectangle(0,0,16,16));

            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Regular);
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
