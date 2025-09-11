using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Net;
using Rxx;
using StocksAnalysis.YahooEngine;
using StocksAnalysis.YahooEngine.API;
using StocksAnalysis.YahooEngine.Database;
using System.Data.SqlClient;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;


namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            SetEnglishCulture();
        //    string indexesTickers = "^DJI+^IXIC+^GSPC+^AMZI+^GDAXI+^FCHI+^PSI20+";


        //    var timer = Observable.Interval(TimeSpan.FromSeconds(1));
        //    var lowNums = from n in oneNumberPerSecond
        //                  select n;

        //    Console.WriteLine("Numbers < 5:");

        //    lowNums.Subscribe(lowNum =>
        //    {
        //        Console.WriteLine(lowNum);
        //    });

        //    Console.ReadKey();


        //    //for (int i = 0; i < 1000; i++)
        //    //{
        //    //    Stopwatch watch = Stopwatch.StartNew();
        //    //    string webResponse;

        //    //    webResponse = WebClientDownload(indexesTickers);
                
        //    //    //WebClient downloader = new WebClient();
        //    //    //downloader.Proxy = null;
        //    //    ////LastTrade = c1; 
        //    //    //var result = downloader.DownloadString("http://finance.yahoo.com/d/quotes.csv?s=MSFT+AAPL&f=sl1c1opva2n&e=.csv");



        //    //    ParseResult(webResponse);
        //    //    //Console.WriteLine(result);

        //    //    watch.Stop();

        //    //    Console.WriteLine(watch.Elapsed.ToString());
        //    //}

           


        //    //var source = new Subject<string>();

        //    //var timer = Observable.Interval(TimeSpan.FromSeconds(1));
        //    //var buffer = source.Buffer(() => timer);
        //    //buffer.Subscribe(Console.WriteLine);


         

          
        //    //IObservable<Quote> source =  Observable.Timer(TimeSpan.FromTicks(0),    TimeSpan.FromSeconds(2))
        //    //    .Subscribe(x => return  {
        //    //        ParseResult(WebClientDownload(indexesTickers));
        //    //    });
                    

                    

        //    ////source.Subscribe(x => ParseResult(x));
        //    //source.Subscribe(x => ParseResult(x));

        //    ////IObservable<string> source1 =  source.Concat(ObservableWebClient
        //    ////        .DownloadString(new Uri("http://finance.yahoo.com/d/quotes.csv?s=MSFT&f=sl1c1opva2n&e=.csv")));

        //    ////IObservable<string> source2 =  source1.Concat(ObservableWebClient
        //    ////        .DownloadString(new Uri("http://finance.yahoo.com/d/quotes.csv?s=MSFT&f=sl1c1opva2n&e=.csv")));


        //    ////source2.Subscribe(x=>Console.WriteLine(x));


        //    ////TestDownloadAndIndicatorsCalculation();
        //    Console.WriteLine("\n Press any key to exit");
        //    Console.ReadKey();
        //}

        //private static string WebClientDownload(string indexesTickers)
        //{
        //    string webResponse;
        //    using (WebClient client = new WebClient())
        //    {
        //        client.Proxy = null;
        //        // Add a user agent header in case the 
        //        // requested URI contains a query.
        //        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        //        client.Headers.Add("Accept-Encoding: gzip");

        //        Stream data = client.OpenRead("http://finance.yahoo.com/d/quotes.csv?s=" + indexesTickers + "&f=sl1c1opva2n&e=.csv");

        //        //AAC+ AACC+ AACOU+ AACOW+ AAME+ AAON+ AAPL+ AATI+ AAWW+ ABAT+ ABAX+ ABBC+ ABCB+ ABCD+ ABCO+ ABCW+ ABFS+ ABIO+ ABMD+ ABTL+ ABVA+ ACAD+ ACAS+ ACAT+ ACCL+ ACET+ ACFC+ ACFN+ ACGL+ ACHN+ ACIW+ ACLS+ ACMR+ ACNB+ ACOM+ ACOR+ ACPW+ ACRX+ ACTG+ ACTS+ ACUR+ ACXM+ ADAT+ ADBE+ ADEP+ ADES+ ADGF+ ADLR+ ADP+ ADPI+ ADSK+ ADTN+ ADUS+ ADVS+ AEGR+ AEHR+ AEIS+ AEPI+ AERG+ AERL+ AETI+ AEY+ AEZS+ AFAM+ AFCB+ AFCE+ AFFM+ AFFX+ AFFY+ AFOP+ AFSI+ AGEN+ AGEND+ AGII+ AGNC+ AGYS+ AHCI+ AHGP+ AHPI+ AIMC+ AINV+ AIRM+ AIRT+ AIXG+ AKAM+ AKRX+ ALCO+ ALGN+ ALGT+ ALIM+ ALKS+ ALLB+ ALLT+ ALNC+ ALNY+ ALOG+ ALOT+ ALRN+ ALSK+ ALTE+ ALTH+ ALTI+ ALTR+ ALVR+ ALXA+ ALXN+ AMAC+ AMAG+ AMAP+ AMAT+ AMBT+ AMCC+ AMCF+ AMCN+ AMCX+ AMED+ AMGN+ AMIC+ AMKR+ AMLN+ AMNB+ AMOT+ AMOV+ AMPE+ AMPL+ AMRB+ AMRI+ AMRN+ AMRS+ AMSC+ AMSF+ AMSG+ AMSWA+ AMTC+ AMTCP+ AMTD+ AMZN+ ANAC+ ANAD+ ANAT+ ANCB+ ANCI+ ANCX+ ANDE+ ANDS+ ANEN+ ANGN+ ANGO+ ANIK+ ANLY+ ANNB+ ANSS+ ANTH+ ANTP+ AONE+ AOSL+ APAC+ APAGF+ APEI+ APFC+ APKT+ APOG+ APOL+ APPY+ APRI+ APSA+ APWC+ ARAY+ ARBA+ ARCC+ ARCI+ ARCL+ ARCP+ ARDNA+ AREX+ ARGN+ ARIA+ ARII+ ARKR+ ARLP+ ARMH+ ARNA+ AROW+ ARQL+ ARRS+ ARRY+ ARSD+ ARTC+ ARTNA+ ARTW+ ARTX+ ARUN+ ARWR+ ASBC+ ASBI+ ASCA+ ASCMA+ ASEI+ ASFI+ ASGN+
        //        //Stream data = client.OpenRead("http://finance.yahoo.com/d/quotes.csv?s=AAC+ AACC+ AACOU+ AACOW+ AAME+ AAON+ AAPL+ AATI+ AAWW+ ABAT+ ABAX+ ABBC+ ABCB+ ABCD+ ABCO+ ABCW+ ABFS+ ABIO+ ABMD+ ABTL+ ABVA+ ACAD+ ACAS+ ACAT+ ACCL+ ACET+ ACFC+ ACFN+ ACGL+ ACHN+ ACIW+ ACLS+ ACMR+ ACNB+ ACOM+ ACOR+ ACPW+ ACRX+ ACTG+ ACTS+ ACUR+ ACXM+ ADAT+ ADBE+ ADEP+ ADES+ ADGF+ ADLR+ ADP+ ADPI+ ADSK+ ADTN+ ADUS+ ADVS+ AEGR+ AEHR+ AEIS+ AEPI+ AERG+ AERL+ AETI+ AEY+ AEZS+ AFAM+ AFCB+ AFCE+ AFFM+ AFFX+ AFFY+ AFOP+ AFSI+ AGEN+ AGEND+ AGII+ AGNC+ AGYS+ AHCI+ AHGP+ AHPI+ AIMC+ AINV+ AIRM+ AIRT+ AIXG+ AKAM+ AKRX+ ALCO+ ALGN+ ALGT+ ALIM+ ALKS+ ALLB+ ALLT+ ALNC+ ALNY+ ALOG+ ALOT+ ALRN+ ALSK+ ALTE+ ALTH+ ALTI+ ALTR+ ALVR+ ALXA+ ALXN+ AMAC+ AMAG+ AMAP+ AMAT+ AMBT+ AMCC+ AMCF+ AMCN+ AMCX+ AMED+ AMGN+ AMIC+ AMKR+ AMLN+ AMNB+ AMOT+ AMOV+ AMPE+ AMPL+ AMRB+ AMRI+ AMRN+ AMRS+ AMSC+ AMSF+ AMSG+ AMSWA+ AMTC+ AMTCP+ AMTD+ AMZN+ ANAC+ ANAD+ ANAT+ ANCB+ ANCI+ ANCX+ ANDE+ ANDS+ ANEN+ ANGN+ ANGO+ ANIK+ ANLY+ ANNB+ ANSS+ ANTH+ ANTP+ AONE+ AOSL+ APAC+ APAGF+ APEI+ APFC+ APKT+ APOG+ APOL+ APPY+ APRI+ APSA+ APWC+ ARAY+ ARBA+ ARCC+ ARCI+ ARCL+ ARCP+ ARDNA+ AREX+ ARGN+ ARIA+ ARII+ ARKR+ ARLP+ ARMH+ ARNA+ AROW+ ARQL+ ARRS+ ARRY+ ARSD+ ARTC+ ARTNA+ ARTW+ ARTX+ ARUN+ ARWR+ ASBC+ ASBI+ ASCA+ ASCMA+ ASEI+ ASFI+ ASGN+&f=sl1c1opva2n&e=.csv");
        //        //Stream data = client.OpenRead("http://finance.yahoo.com/d/quotes.csv?s=MSFT&f=sl1c1opva2n&e=.csv");
        //        StreamReader reader = new StreamReader(data);
        //        webResponse = reader.ReadToEnd();
        //        data.Close();
        //        reader.Close();
        //    }
        //    return webResponse;
        }

        static List<Quote> ParseResult(string result)
        {
            var quotes = new List<Quote>();
            result = result.Replace('\r',' ').Trim();

            string[] rows = result.Split(new Char[] {  '\n' });
                
            foreach (string row in rows)
            {
                string[] fields = row.Split(new Char[] { ',' });

                var quote = new Quote()
                {
                    Ticker = fields[0],
                    Last = TryToConvert.ToDouble(fields[1]),
                    Change = TryToConvert.ToDouble(fields[2]),
                    Open = TryToConvert.ToDouble(fields[3]),
                    Close = TryToConvert.ToDouble(fields[4]),
                    Volume = TryToConvert.ToInt64(fields[5]),
                    AvgVolume = TryToConvert.ToInt64(fields[6]),
                    Name = fields[7]
                };

                quotes.Add(quote);
            }

            Console.WriteLine(quotes.Count() + " Entities parsed.");
            return quotes;
        }

        static void WebClientDownloadString()
        {
            string url = "http://finance.yahoo.com/d/quotes.csv?s=MSFT&f=snd1l1yr";

            using (var client = new WebClient())
            {
                Console.WriteLine("Connecting: " + url);
                string src = client.DownloadString(url);
                Console.WriteLine("Connected ..");
            }
        }

        static void MakeHttpRequest()
        {
            Random r = new Random();
            //WTF is going on here? Takes 30s on Windows 7, <100ms on Windows Vista
            string url = "http://finance.yahoo.com/d/quotes.csv?s=MSFT&f=snd1l1yr";


            Console.WriteLine("Connecting: " + url);
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Proxy = null;
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            Console.WriteLine("Connected ..");
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                // Dummy
                Console.WriteLine("Getting data ..");
                httpResponse.GetResponseStream();



            } // if
            else
                Console.WriteLine("Failed to get any data ..");

            Console.WriteLine("Done ..");



        }

        static void TestDownloadAndIndicatorsCalculation()
        {
            SetEnglishCulture();

            Console.WriteLine("Starting To Process \n");

            List<HistoricalQuoteDownload> Quotes = new List<HistoricalQuoteDownload>();
            Quotes = new List<HistoricalQuoteDownload>();


            //IEnumerable<string> IDS = new SymbolLookupDal().GetSymbols("NASDAQ");

            //foreach (var id in IDS)
            //{
            //    Console.WriteLine("Starting To Download {0}", id);
            //    HistoricalQuoteDownload Hist = new HistoricalQuoteDownload();
            //    Hist.Download(id, 1900, "d");
            //    //Hist.Download(id, 2011, "w");
            //    //Hist.Download(id, 2011, "m");
            //    Console.WriteLine("sAVING {0}", id);
            //    Hist.SaveDb();
            //    Quotes.Add(Hist);
            //}

            HistoricalQuoteDal HistoricalQuoteDal = new HistoricalQuoteDal();

            IEnumerable<string> IDS = new SymbolLookupDal().GetSymbols("NASDAQ");

            foreach (var id in IDS)
            {
                //Console.WriteLine("Starting To Download {0}", id);
                HistoricalQuoteDownload Hist = new HistoricalQuoteDownload();
                Hist.Download(id, 1900, "d");
                //Hist.Download(id, 2011, "w");
                //Hist.Download(id, 2011, "m");
                //Console.WriteLine("sAVING {0}", id);
                //Hist.SaveDb();
                Quotes.Add(Hist);
            }


            IEnumerable<HistoricalQuote> ExchangeQuotes = HistoricalQuoteDal.GetHistoricalQuotes("NASDAQ");
            foreach (var tickerHistQuotes in ExchangeQuotes.GroupBy(x => x.Symbol))
            {

                Console.WriteLine("Starting To Download {0} from Database", tickerHistQuotes.ToList()[0].Symbol);

                HistoricalQuoteDownload Hist = new HistoricalQuoteDownload();
                Hist.Ticker = tickerHistQuotes.ToList()[0].Symbol;
                Hist.HistoricalQuotes = tickerHistQuotes.ToList();

                Hist.CalculateMovingAverages();
                Quotes.Add(Hist);
            }



            Console.WriteLine("Download Completed, Starting Cross Analysis");

            var crosses = Quotes.Where(x => x.CrossOccurred == true);
            var twokaysixHigh = Quotes.Where(x => x.TwoKaySixYearAscending == true);
            var twokaySevenHigh = Quotes.Where(x => x.TwoKaySevenYearAscending == true);
            var twokayEitghHigh = Quotes.Where(x => x.TwoKayEigthYearAscending == true);
            var FiftyTowWeeks = Quotes.Where(x => x.FiftyTwoWeeksAscending == true);


            string filesuffix = DateTime.Now.Year.ToString()
                                    + DateTime.Now.Month.ToString()
                                    + DateTime.Now.Day.ToString()
                                    + DateTime.Now.Hour.ToString()
                                    + DateTime.Now.Minute.ToString();

            string filename = string.Format(@"C:\Bolsa\StocksAnalysis\StocksAnalisys_{0}.txt", filesuffix);

            FileStream stream;
            FileMode fileMode;
            if (File.Exists(filename))
            {
                // it already exists, let's append to it
                fileMode = FileMode.Create;
            }
            else
            {
                // it does not exist, let's create it
                fileMode = FileMode.CreateNew;
            }

            stream = File.Open(filename, fileMode, FileAccess.Write, FileShare.None);
            StreamWriter writer = new StreamWriter(stream);



            foreach (var item in crosses)
            {
                var message = string.Format("The Ticker {0} has a Cross:\n {1}", item.Ticker, item.TrendType.ToString());
                Console.WriteLine(message);
                writer.Write(message);
            }

            Console.WriteLine("\n\n\n");
            writer.Write("\n\n\n");

            foreach (var item in FiftyTowWeeks)
            {
                var message = string.Format("The Ticker {0} has a 52 Weeks High \n", item.Ticker);
                Console.WriteLine(message);
                writer.Write(message);
            }


            Console.WriteLine("\n\n\n");
            writer.Write("\n\n\n");

            foreach (var item in twokaysixHigh)
            {
                var message = string.Format("The Ticker {0} is in a 2006 High \n", item.Ticker);
                Console.WriteLine(message);
                writer.Write(message);
            }


            Console.WriteLine("\n\n\n");
            writer.Write("\n\n\n");

            foreach (var item in twokaySevenHigh)
            {
                var message = string.Format("The Ticker {0} is in a 2007 Weeks High \n", item.Ticker);
                Console.WriteLine(message);
                writer.Write(message);
            }


            Console.WriteLine("\n\n\n");
            writer.Write("\n\n\n");

            foreach (var item in twokayEitghHigh)
            {
                var message = string.Format("The Ticker {0} is in a 2008 Weeks High \n", item.Ticker);
                Console.WriteLine(message);
                writer.Write(message);
            }

            writer.Flush();
            writer.Close();

            Console.WriteLine("Done");
        }

        private static void SetEnglishCulture()
        {
            Console.WriteLine(CultureInfo.CurrentCulture.ToString());
            Console.WriteLine(CultureInfo.CurrentUICulture.ToString());

            Console.WriteLine("Changing Language to English");

            CultureInfo CurrentCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo CurrentUICulture = Thread.CurrentThread.CurrentUICulture;


            CultureInfo EnglishCultureInfo = new CultureInfo("en-US");

            Thread.CurrentThread.CurrentCulture = EnglishCultureInfo;
            Thread.CurrentThread.CurrentUICulture = EnglishCultureInfo;

            Console.WriteLine(CultureInfo.CurrentCulture.ToString());
            Console.WriteLine(CultureInfo.CurrentUICulture.ToString());
        }

        static List<string> GetSymbols(string Exchange)
        {


            using (SqlConnection connection = new SqlConnection(@"Server=RUIGUERRAP\\SQLSERVER2008R2;Database=StocksDB;User ID=sa;Password=dev2008r2;Trusted_Connection=False;"))
            {
                // Create the command and set its properties.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = string.Format(@"SELECT [Symbol] FROM [SymbolLookup] 
                    WHERE [State] != 'ER' 
                    AND [Type] ='S' 
                    AND [Symbol] is not null 
                    AND len([SYMBOL]) >0 
                    AND [exchDisp] IN ('{0}') ", Exchange);
                command.CommandType = System.Data.CommandType.Text;

                // Open the connection and execute the reader.
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<string> result = new List<string>();


                if (!reader.HasRows)
                {
                    Console.WriteLine("No rows found.");
                }
                else
                {
                    while (reader.Read())
                    {
                        result.Add(reader[0].ToString());
                    }
                }

                reader.Close();

                return result;
            }
        }
    }
}
