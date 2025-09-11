using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace StocksAnalysis.QuoteProvider
{
    public interface IDownload
    {
        IWebProxy Proxy { get; set; }   
        int Timeout { get; set; }   
    }
}
