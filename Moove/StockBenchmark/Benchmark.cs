using Infrastructure;
using Newtonsoft.Json;
using Simple.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockBenchmark
{
    public delegate List<Task<TResult>> CreateTasksMethod<TResult>(List<string> ids);
    public delegate List<TodayPrice> ReturnResultMethod<TResult>(Task<TResult>[] task);

    public class Benchmark
    {
        private bool _running;
        private TaskScheduler _scheduler;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private List<TodayPrice> _todayPrices;

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

        public void Start()
        {
            if (_running) { SimpleLogger.NotifyUI("clica só uma vez sff. obg"); return; }

            SimpleLogger.NotifyUI("Bench.start");
            _running = true;
            _scheduler = TaskScheduler.Default;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _todayPrices = new List<TodayPrice>();


            ServicePointManager.SetTcpKeepAlive(true, 240000, 240000);
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 10000;

            SimpleLogger.Write(string.Format("ServicePointManager.UseNagleAlgorithm={0}", ServicePointManager.UseNagleAlgorithm.ToString()));
            SimpleLogger.Write(string.Format("ServicePointManager.Expect100Continue={0}", ServicePointManager.Expect100Continue.ToString()));
            SimpleLogger.Write(string.Format("ServicePointManager.DefaultConnectionLimit={0}", ServicePointManager.DefaultConnectionLimit.ToString()));

            //Work();
            TaskRepeat.Interval(
                TimeSpan.FromSeconds(5),
                () => Work(),
                _cancellationTokenSource.Token);

            //var task = Task.Factory.StartNew(() =>
            //{
            //    SimpleLogger.Write("Bench.start");
            //    SimpleLogger.Write("");
            //    ServicePointManager.SetTcpKeepAlive(true, 240000, 240000);
            //    ServicePointManager.UseNagleAlgorithm = false;
            //    ServicePointManager.Expect100Continue = false;
            //    ServicePointManager.DefaultConnectionLimit = 10000;

            //    SimpleLogger.Write(string.Format("ServicePointManager.UseNagleAlgorithm={0}", ServicePointManager.UseNagleAlgorithm.ToString()));
            //    SimpleLogger.Write(string.Format("ServicePointManager.Expect100Continue={0}", ServicePointManager.Expect100Continue.ToString()));
            //    SimpleLogger.Write(string.Format("ServicePointManager.DefaultConnectionLimit={0}", ServicePointManager.DefaultConnectionLimit.ToString()));


            //    //this.GetYahooData();
            //    this.GetTickData();
            //    //this.RaiseDataUpdated(this._todayPrices);
            //    SimpleLogger.NotifyUI("Loop complete");
            //});
            //task.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel(true);
            _running = false;
        }


        private void Work()
        {
            var watch = Stopwatch.StartNew();
            SimpleLogger.NotifyUI(string.Format("executing - {0}", DateTime.Now));

            GetTickData();


            SimpleLogger.NotifyUI("Loop complete");
            watch.Stop();
            SimpleLogger.NotifyUI(string.Format("minutos gastos- {0}:{1}::{2}",
                watch.Elapsed.Minutes, 
                watch.Elapsed.Seconds,
                watch.Elapsed.Milliseconds));
        }

        //private void GetYahooData()
        //{
        //    SimpleLogger.Write("Downloading TodayPrice YahooData");
        //    List<TodayPrice> data = EquitiesServiceBase.GetData<List<TodayPrice>>( this._cancellationTokenSource, this._scheduler, this.ids, new CreateTasksMethod<List<TodayPrice>>(EquitiesYahooService.CreateTasks), new ReturnResultMethod<List<TodayPrice>>(EquitiesYahooService.ReturnResult), false);
        //    MergeYahooData(data);
        //}

        //private void MergeYahooData(List<TodayPrice> source)
        //{
        //    SimpleLogger.Write("Merging TodayPrice YahooData");
        //    try
        //    {
        //        (from item in source
        //            where !string.IsNullOrWhiteSpace(item.Ticker)
        //            select item).ToList<TodayPrice>().ForEach((TodayPrice item) =>
        //            {
        //                try
        //                {
        //                    int volume = this._todayPrices.FindIndex((TodayPrice x) => x.Ticker == item.Ticker);
        //                    if (volume >= 0)
        //                    {
        //                        this._todayPrices[volume].Volume = item.Volume;
        //                        this._todayPrices[volume].AvgVolume = item.AvgVolume;
        //                        this._todayPrices[volume].RelativeVolume = Calculations.CalculateRelativeVolume(this._todayPrices[volume].AvgVolume, this._todayPrices[volume].Volume);
        //                    }
        //                }
        //                catch (Exception exception)
        //                {
        //                    SimpleLogger.Write(string.Format("Error merging item {0};err:{1}", item.Ticker, exception.GetBaseException()));
        //                }
        //            });
        //    }
        //    catch (AggregateException aggregateException)
        //    {
        //        aggregateException.Flatten().Handle((Exception e) =>
        //        {
        //            SimpleLogger.Write(string.Format("Error merging collections;err{0}", e.GetBaseException()));
        //            return true;
        //        });
        //    }
        //}
        private void GetTickData()
        {
            SimpleLogger.Write("Downloading TodayPrice TickData");
            EquitiesServiceBase equitiesServiceBase = new EquitiesServiceBase();
            EquitiesLevelIIService equitiesLevelIIService = new EquitiesLevelIIService();

            List<TodayPrice> data = equitiesServiceBase.GetData<TodayPrice>(this._cancellationTokenSource, this._scheduler, this.ids, new CreateTasksMethod<TodayPrice>(equitiesLevelIIService.CreateTasks), new ReturnResultMethod<TodayPrice>(equitiesLevelIIService.ReturnResult), false).Where(t => t != null && t.Ticker != null).ToList();
            int count = data.Where(d => d != null && !string.IsNullOrWhiteSpace(d.Ticker)).ToList().Count;
            SimpleLogger.NotifyUI(count.ToString());
            //this.MergeTickData(data);

        }
        private void MergeTickData(List<TodayPrice> source)
        {
            SimpleLogger.Write("Merging TodayPrices TickData");
            try
            {
                (
                    from item in source
                    where !string.IsNullOrWhiteSpace(item.Ticker)
                    select item).ToList<TodayPrice>().ForEach((TodayPrice item) =>
                    {
                        try
                        {
                            int last = this._todayPrices.FindIndex((TodayPrice x) => x.Ticker == item.Ticker);
                            if (last >= 0)
                            {
                                this._todayPrices[last].Last = item.Last;
                                this._todayPrices[last].Open = item.Open;
                                this._todayPrices[last].IntradayChange = Calculations.CalculateIntraDayChange(this._todayPrices[last].Open, this._todayPrices[last].Last);
                                this._todayPrices[last].IntradayChangePercent = Calculations.CalculateIntraDayChangePercentage(this._todayPrices[last].Open, this._todayPrices[last].Last);
                                this._todayPrices[last].Change = Calculations.CalculateChange(this._todayPrices[last].PreviousClose, this._todayPrices[last].Last);
                                this._todayPrices[last].ChangePercent = Calculations.CalculateChangePercentage(this._todayPrices[last].PreviousClose, this._todayPrices[last].Last);
                            }
                        }
                        catch (Exception exception)
                        {
                            SimpleLogger.Write(string.Format("Error merging item {0};err:{1}", item.Ticker, exception.GetBaseException()));
                        }
                    });
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Flatten().Handle((Exception e) =>
                {
                    SimpleLogger.Write(string.Format("Error merging collections;err:{0}", e.GetBaseException()));
                    return true;
                });
            }
        }

        public List<Symbol> LoadActiveSymbols()
        {
            List<Symbol> symbols;
            try
            {
                List<Symbol> symbols1 = new List<Symbol>();
                foreach (dynamic obj in (IEnumerable)Database.Open().GetActiveTickers())
                {
                    List<Symbol> symbols2 = symbols1;
                    Symbol symbol = new Symbol()
                    {
                        Ticker = (string)obj.Ticker,
                        Name = (string)obj.Name,
                        Exchange = (string)obj.Exchange,
                        Sector = (string)obj.Sector,
                        Industry = (string)obj.Industry,
                        Enabled = (bool)obj.Enabled,
                        Attempts = (short)obj.Attempts
                    };
                    symbols2.Add(symbol);
                }
                symbols = symbols1;
            }
            catch (Exception exception)
            {
                SimpleLogger.Write(string.Format("Error loading active symbols from database;err:{0}", exception));
                throw;
            }
            return symbols;
        }

    }


    public class EquitiesLevelIIService
    {
        public List<Task<TodayPrice>> CreateTasks(List<string> ids)
        {
            List<Task<TodayPrice>> tasks = new List<Task<TodayPrice>>();
            ids.ForEach((string id) => tasks.Add(new Task<TodayPrice>(() => Download(id), TaskCreationOptions.AttachedToParent)));
            return tasks;
        }

        private TodayPrice Download(string id)
        {
            TodayPrice todayPrice;
            try
            {
                Tick tick = (new TickDownload()).Download(id);
                if (tick == null) return null;
                if (tick.Ticker != id)
                {
                    return new TodayPrice();
                    //Just a threadsafe?? control clause 
                    //Its not a crossthread issue,
                    //throw new Exception("The returned ticker doesn't match the specified in the request, maybe a crossthread issue");
                    
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
                SimpleLogger.Write(string.Format("download error;err:{0}", exception.Message));
                //Infelizmente tenho de retornar um objecto nao "null", pq se o fizer dá erro de runtime binding
                return new TodayPrice();
            }
            return todayPrice;
        }

        public  List<TodayPrice> ReturnResult(Task<TodayPrice>[] tasks)
        {
            return (
                from t in (IEnumerable<Task<TodayPrice>>)tasks
                select t.Result).ToList<TodayPrice>();
                //.Where(t=> t != null && t.Ticker != null).ToList();
        }
    }
    public class TickDownload
    {
        private IWebProxy mProxy = WebRequest.DefaultWebProxy;

        private Dictionary<string, string> _urls = new Dictionary<string, string>();

        private Queue<Action> _webActions = new Queue<Action>();

        private List<Tick> _ticks = new List<Tick>();

        public TickDownload(string id)
        {
            this._urls.Add(string.Concat("http://dev.markitondemand.com/Api/Quote/json?symbol=", id), TickDownload.Provider.MarkitDev.ToString());
            this._urls.ToList<KeyValuePair<string, string>>().ForEach((KeyValuePair<string, string> dic) => this._webActions.Enqueue(new Action(() =>
            {
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.webclient_DownloadStringCompleted);
                webClient.DownloadStringAsync(new Uri(dic.Key), dic.Value);
            })));
        }

        public TickDownload()
        {
        }

        private void CheckChainAndPublishEvent()
        {
            if (this._ticks.Count<Tick>() == this._urls.Count<KeyValuePair<string, string>>())
            {
                this.RaiseDownloadComplete((
                    from t in this._ticks
                    orderby t.TradeTime descending
                    select t).FirstOrDefault<Tick>());
            }
        }

        private string CreateWebClient(string id)
        {
            return (new WebClient()).DownloadString(string.Concat("http://dev.markitondemand.com/Api/Quote/json?symbol=", id));
        }

        private string CreateWebRequest(string id)
        {
            string str = string.Concat("http://dev.markitondemand.com/Api/Quote/json?symbol=", id);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(str);
            httpWebRequest.Proxy = this.mProxy;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
            httpWebRequest.Timeout = 5000;
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

        private dynamic DeserializeJson(string response)
        {
            return (new JsonSerializer()).Deserialize(new JsonTextReader(new StringReader(response)));
        }

        public Tick Download(string id)
        {
            return this.ProcessResponse(this.CreateWebRequest(id), TickDownload.Provider.MarkitDev.ToString());
        }

        public void DownloadAsync()
        {
            try
            {
                while (this._webActions.Count > 0)
                {
                    try
                    {
                        this._webActions.Dequeue()();
                    }
                    catch (Exception exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception exception1)
            {
                throw;
            }
        }

        private Tick ProcessBatsResponse(string response)
        {
            Tick tick;
            try
            {
                dynamic obj = this.DeserializeJson(response);
                dynamic obj1 = obj.data.trades[0];
                string[] strArrays = (string[])obj1[0].ToString().Split(':');
                Tick tick1 = new Tick();
                DateTime today = DateTime.Today;
                DateTime dateTime = today.AddHours((double)int.Parse(strArrays[0]));
                DateTime dateTime1 = dateTime.AddMinutes((double)int.Parse(strArrays[1]));
                tick1.TradeTime = dateTime1.AddSeconds((double)int.Parse(strArrays[2]));
                tick1.Size = int.Parse(obj1[1].ToString());
                tick1.Price = double.Parse(obj1[2].ToString());
                tick = tick1;
            }
            catch (Exception exception)
            {
                SimpleLogger.Write(string.Format("error TickDownload.ProcessBatsResponse;err:{0}", exception.Message));
                return null;
            }
            return tick;
        }

        private Tick ProcessMarkitDevResponse(string response)
        {
            Tick tick;
            try
            {
                dynamic obj = this.DeserializeJson(response);
                dynamic message = obj.Message;
                dynamic status = obj.Data.Status;


                //assumimos que se vieram estes dados não obtivemos dados correctos
                if (message != null)
                {
                    SimpleLogger.Write(string.Format("nodataretrived:{0}", obj.message));
                    return new Tick();
                }
                else if (status != null)
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
                    SimpleLogger.Write(string.Format("response not in correct format. response=", response));
                    return new Tick();
                }
            }
            catch (Exception exception)
            {
                SimpleLogger.Write(string.Format("error TickDownload.ProcessMarkitDevResponse;err:{0}", exception.Message));
                return new Tick();
            }
            return tick;
        }

        private Tick ProcessMarkitResponse(string response)
        {
            this.DeserializeJson(response);
            return new Tick();
        }

        private Tick ProcessResponse(string response, object state)
        {
            if (state as string == TickDownload.Provider.Bats.ToString())
            {
                return this.ProcessBatsResponse(response);
            }
            if (state as string == TickDownload.Provider.Markit.ToString())
            {
                return this.ProcessMarkitResponse(response);
            }
            if (state as string != TickDownload.Provider.MarkitDev.ToString())
            {
                return new Tick();
            }
            return this.ProcessMarkitDevResponse(response);
        }

        private void RaiseDownloadComplete(Tick tick)
        {
            if (this.DownloadComplete != null)
            {
                this.DownloadComplete(tick);
            }
        }

        private void webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Tick tick = new Tick();
            if (e != null)
            {
                if (e.Error != null)
                {
                    throw new Exception(e.Error.Message, e.Error);
                }
                if (e.Cancelled)
                {
                    return;
                }
                if (e.Result != null)
                {
                    tick = this.ProcessResponse(e.Result, e.UserState);
                }
            }
            this._ticks.Add(tick);
            this.CheckChainAndPublishEvent();
        }

        public event Action<Tick> DownloadComplete;

        private enum Provider
        {
            Bats,
            Markit,
            MarkitDev,
            YahooScraper
        }
    }
    public class EquitiesServiceBase
    {
        public List<TodayPrice> GetData<TResult>(
            CancellationTokenSource cancellationTokenSource,
            TaskScheduler scheduler,
            List<string> ids,
            CreateTasksMethod<TResult> createTasksMethod,
            ReturnResultMethod<TResult> returnResultMethod,
            bool synchronous = false)
        {
            List<TodayPrice> list;
            Action<Task<TResult>> action = null;
            Action<Task<TResult>> action1 = null;
            Func<Exception, bool> func = null;
            try
            {
                list = Task<List<TodayPrice>>.Factory.StartNew(() =>
                {
                    List<Task<TResult>> tasks = createTasksMethod(ids);
                    if (synchronous)
                    {
                        List<Task<TResult>> tasks1 = tasks;
                        if (action1 == null)
                        {
                            action1 = (Task<TResult> t) => t.RunSynchronously();
                        }
                        tasks1.ForEach(action1);
                    }
                    else
                    {
                        List<Task<TResult>> tasks2 = tasks;
                        if (action == null)
                        {
                            action = (Task<TResult> t) => t.Start();
                        }
                        tasks2.ForEach(action);
                    }
                    try
                    {
                        SimpleLogger.Write(string.Format("Waiting for tasks to finish", new object[0]));
                        Task.WaitAll((Task[])tasks.ToArray());
                    }
                    catch (AggregateException aggregateException2)
                    {
                        AggregateException aggregateException = aggregateException2;
                        SimpleLogger.Write(string.Format("Error executing tasks.err:{0}", aggregateException.GetBaseException()));
                        AggregateException aggregateException1 = aggregateException.Flatten();
                        if (func == null)
                        {
                            func = (Exception e) => true;
                        }
                        aggregateException1.Handle(func);
                    }
                    return Task.Factory.ContinueWhenAll<TResult, List<TodayPrice>>(tasks.ToArray(), (Task<TResult>[] tsk) =>
                    {
                        SimpleLogger.Write(string.Format("Returning tasks result.", new object[0]));
                        return returnResultMethod(tsk);
                    }, cancellationTokenSource.Token, TaskContinuationOptions.None, scheduler).Result.ToList<TodayPrice>();
                }, cancellationTokenSource.Token, TaskCreationOptions.None, scheduler).Result.ToList<TodayPrice>();
            }
            catch (Exception exception)
            {
                SimpleLogger.Write(string.Format("EquitiesService;err:{0}", exception.GetBaseException()));
                throw;
            }
            return list;
        }
    }
    public class Calculations
    {
        public Calculations()
        {
        }

        public static long CalculateAverageVolume(int periods, List<long> orderedVolumeData)
        {
            if (orderedVolumeData.Count < periods)
            {
                return (long)0;
            }
            return Convert.ToInt64(orderedVolumeData.Take<long>(periods).Average());
        }

        public static double CalculateChange(double prevclose, double lasttrade)
        {
            if (prevclose == 0 || lasttrade == 0)
            {
                return 0;
            }
            return (lasttrade - prevclose).RoundTwoDecimalPlaces();
        }

        public static double CalculateChangePercentage(double prevclose, double lasttrade)
        {
            if (prevclose == 0 || lasttrade == 0)
            {
                return 0;
            }
            return (lasttrade * 100 / prevclose - 100).RoundTwoDecimalPlaces();
        }

        public static double CalculateGap(double prevclose, double open)
        {
            if (prevclose == 0 || open == 0)
            {
                return 0;
            }
            return (open - prevclose).RoundTwoDecimalPlaces();
        }

        public static double CalculateGapPercentage(double prevclose, double open)
        {
            if (prevclose == 0 || open == 0)
            {
                return 0;
            }
            return (prevclose * 100 / open - 100).RoundTwoDecimalPlaces();
        }

        public static double CalculateIntraDayChange(double open, double lasttrade)
        {
            if (lasttrade == 0 || open == 0)
            {
                return 0;
            }
            return (lasttrade - open).RoundTwoDecimalPlaces();
        }

        public static double CalculateIntraDayChangePercentage(double open, double lasttrade)
        {
            if (lasttrade == 0 || open == 0)
            {
                return 0;
            }
            return (lasttrade * 100 / open - 100).RoundTwoDecimalPlaces();
        }

        public static long CalculateMaxVoume(long avgVolume, long volume)
        {
            if (avgVolume < volume)
            {
                return volume;
            }
            return avgVolume;
        }

        public static double CalculateRelativeVolume(long avgVolume, long volume)
        {
            if (avgVolume == (long)0 || volume == (long)0)
            {
                return 0;
            }
            return (DataTypeConverters.TryConvertToDouble(volume) / DataTypeConverters.TryConvertToDouble(avgVolume)).RoundTwoDecimalPlaces();
        }

        public static bool CalculateVolumeSpike(long avgVolume, long volume)
        {
            if (avgVolume == (long)0 || volume == (long)0)
            {
                return false;
            }
            return volume >= avgVolume;
        }
    }
    public static class MathHelpers
    {
        public static double RoundTwoDecimalPlaces(this double value)
        {
            return Math.Round(value, 2);
        }
    }
    public static class DataTypeConverters
    {
        [DebuggerStepThrough]
        public static object StringToObject(string str)
        {
            string str1 = str.Replace("%", "").Replace("\"", "").Replace("<b>", "").Replace("</b>", "").Replace("N/A", "").Trim();
            if (str1 == string.Empty)
            {
                return string.Empty;
            }
            if (str1 == "-")
            {
                return string.Empty;
            }
            if (!str1.Contains(" - "))
            {
                double num = 0;
                if (double.TryParse(str1, out num))
                {
                    return num;
                }
                long num1 = (long)0;
                if (long.TryParse(str1, out num1))
                {
                    return num1;
                }
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(str1, out dateTime))
                {
                    return str1;
                }
                return dateTime;
            }
            string[] strArrays = new string[] { " - " };
            string[] strArrays1 = str1.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
            List<object> objs = new List<object>();
            string[] strArrays2 = strArrays1;
            for (int i = 0; i < (int)strArrays2.Length; i++)
            {
                objs.Add(DataTypeConverters.StringToObject(strArrays2[i]));
            }
            if (objs.Count == 0)
            {
                return string.Empty;
            }
            if (objs.Count != 0)
            {
                return objs.ToArray();
            }
            return objs[0];
        }

        public static Nullable<T> ToNullable<T>(this string s)
        where T : struct
        {
            Nullable<T> nullable = null;
            try
            {
                if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    nullable = new Nullable<T>((T)converter.ConvertFrom(s));
                }
            }
            catch
            {
            }
            return nullable;
        }

        [DebuggerStepThrough]
        public static double TryConvertToDouble(object value)
        {
            double num;
            try
            {
                num = Convert.ToDouble(value);
            }
            catch
            {
                num = 0;
            }
            return num;
        }

        [DebuggerStepThrough]
        public static long TryConvertToLong(this object value)
        {
            long num;
            try
            {
                num = Convert.ToInt64(value);
            }
            catch
            {
                num = (long)0;
            }
            return num;
        }

        [DebuggerStepThrough]
        public static double TryParseChangePercentage(string value)
        {
            double num;
            try
            {
                num = DataTypeConverters.TryConvertToDouble(DataTypeConverters.StringToObject(value));
            }
            catch
            {
                num = 0;
            }
            return num;
        }
    }

}
