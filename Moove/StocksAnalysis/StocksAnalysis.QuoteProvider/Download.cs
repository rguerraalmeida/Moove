using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StocksAnalysis.QuoteProvider
{
    public class Download : IDownload
    {


        public System.Net.IWebProxy Proxy
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Timeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
