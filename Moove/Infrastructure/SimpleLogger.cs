using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class SimpleLogger
    {
        public static void Write(string message)
        {
            //if (Log != null)
            //{
            //    Log(message);
            //}
        }

        public static void NotifyUI(string message)
        {
            if (Log != null)
            {
                Log(message);
            }
        }

        public static event Action<string> Log;
    }
}
