using MooveUI.Extensions;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MooveUI.Converters
{
    //[ValueConversion(typeof(double), typeof(Brush))]
    public class ValueToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            double convertedValue = (double)value;

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
