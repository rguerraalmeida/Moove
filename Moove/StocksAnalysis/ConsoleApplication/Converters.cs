using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
    static class TryToConvert
    {
        public static double ToDouble(string value)
        {
            try
            {
                if (value == null || value == string.Empty ||  value == "")
                    return 0.0;
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
        }

        public static long ToInt64(string value)
        {
            try
            {
                if (value == null || value == string.Empty || value == "")
                    return 0;
                return Convert.ToInt64(value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
