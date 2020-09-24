using Infrastructure;
using Newtonsoft.Json;
using Simple.Data;
using StockBenchmark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testes
{
    class Program
    {
       
       

        static void Main(string[] args)
        {
            //TaskRepeat.Interval(
            //   TimeSpan.FromSeconds(3),
            //   () => Download("AAPL"),
            //   _cancellationTokenSource.Token);

           // Download("AAPL");

            Cenas c = new Cenas();
            c.GetData();
            Console.ReadLine();
        }




    }

    public class Cenas
    {
        TaskScheduler _scheduler;
        CancellationTokenSource _cancellationTokenSource ;
        CancellationToken _cancellationToken;
        IWebProxy mProxy = WebRequest.DefaultWebProxy;

        private List<string> _ids;
        private List<string> ids
        {
            get
            {
                if (_ids == null)
                {
                    _ids = LoadTickers();
                }
                return _ids;
            }
        }

        private List<string> LoadTickers()
        {
            dynamic database = Database.OpenConnection(AppParameters.CNNSTRING);
            var symbols = database.Symbol.All();
            var tickers = new List<string>();
            foreach (var symbol in symbols)
            {
                tickers.Add(symbol.Ticker);
            }
            return tickers;
        }

        public void GetData()
        {
            _scheduler = TaskScheduler.Default;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            var maintask = Task.Factory.StartNew(() =>
            {
                List<Task<TodayPrice>> tasks = new List<Task<TodayPrice>>();
                List<TodayPrice> prices = new List<TodayPrice>();

                try
                {
                    ids.ForEach((string id) => tasks.Add(new Task<TodayPrice>(() => Download(id), TaskCreationOptions.AttachedToParent)));
                    Console.WriteLine(string.Format("Waiting for tasks to finish", new object[0]));
                    tasks.ForEach(t => t.Start());
                    Task.WaitAll((Task[])tasks.ToArray());
                    
                }
                catch (AggregateException aggregateException)
                {
                    Console.WriteLine(string.Format("Error executing tasks.err:{0}", aggregateException.GetBaseException()));

                }

                Task.Factory.ContinueWhenAll(tasks.ToArray(), results =>
                {
                    Console.WriteLine(string.Format("Returning tasks result.", new object[0]));
                    prices = results.Select(p => p.Result).Where(t => t.Ticker != null && !string.IsNullOrWhiteSpace(t.Ticker)).ToList();
                    Console.WriteLine(string.Format("Returned {0} companies data.", prices.Count));
                },
                _cancellationTokenSource.Token,
                TaskContinuationOptions.None,
                _scheduler);

                
            });
            
        }

        private TodayPrice Download(string id)
        {
            TodayPrice todayPrice;
            try
            {
                Tick tick = DownloadIt(id);
                if (tick == null) return null;
                if (tick.Ticker != id)
                {
                    //Just a threadsafe?? control clause
                    //throw new Exception("The returned ticker doesn't match the specified in the request, maybe a crossthread issue");
                    return new TodayPrice();
                }

                todayPrice = new TodayPrice()
                {
                    Ticker = id,
                    Last = tick.Price,
                    Open = tick.Open
                };
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("download error;err:{0}", exception.Message));
                return new TodayPrice();
            }
            return todayPrice;
        }

        private Tick DownloadIt(string id)
        {
            return ProcessResponse(CreateWebRequest(id));
        }

        private string CreateWebRequest(string id)
        {
            string str = string.Concat("http://dev.markitondemand.com/Api/Quote/json?symbol=", id);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(str);
            httpWebRequest.Proxy = mProxy;
            httpWebRequest.KeepAlive = true;
            string empty = string.Empty;
            using (WebResponse response = httpWebRequest.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.ASCII))
                    {
                        empty = streamReader.ReadToEnd();
                    }
                }
            }
            return empty;
        }

        private Tick ProcessResponse(string response)
        {
            return ProcessMarkitDevResponse(response);
        }

        private Tick ProcessMarkitDevResponse(string response)
        {
            Tick tick;
            try
            {
                dynamic obj = DeserializeJson(response);
                dynamic message = obj.Message;
                dynamic status = obj.Data.Status;


                //assumimos que se vieram estes dados não obtivemos dados correctos
                if (message != null)
                {
                    Console.WriteLine(string.Format("nodataretrived:{0}", obj.message));
                    return new Tick();
                }
                else if (status != null && status == "SUCCESS")
                {
                    dynamic obj1 = obj.Data.Symbol;
                    dynamic obj2 = double.Parse(obj.Data.LastPrice.ToString());
                    dynamic obj3 = double.Parse(obj.Data.Open.ToString());
                    dynamic obj4 = obj.Data.Timestamp.ToString();
                    DateTime dateTime = (DateTime)DateTime.ParseExact(obj4, "ddd MMM d HH:mm:ss UTC-05:00 yyyy", (dynamic)null);
                    tick = new Tick()
                    {
                        Ticker = (string)obj1,
                        TradeTime = dateTime,
                        Price = (double)obj2,
                        Open = (double)obj3
                    };
                }
                else
                {
                    Console.WriteLine(string.Format("response not in correct format. response={0}", response));
                    return new Tick();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("error TickDownload.ProcessMarkitDevResponse;err:{0}", exception.Message));
                return new Tick();
            }
            Console.WriteLine(string.Format("tick processed={0}", tick.Ticker));
            return tick;
        }

        private dynamic DeserializeJson(string response)
        {
            return (new JsonSerializer()).Deserialize(new JsonTextReader(new StringReader(response)));
        }


    }
}
