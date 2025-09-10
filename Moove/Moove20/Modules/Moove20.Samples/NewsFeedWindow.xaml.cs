using MaasOne.RSS;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Moove20.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.FtpClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Moove20.Samples
{
    /// <summary>
    /// Interaction logic for NewsFeedWindow.xaml
    /// </summary>
    public partial class NewsFeedWindow : MahApps.Metro.Controls.MetroWindow
    {
        public NewsFeedWindow()
        {
            NewsFeedViewModel vm = new NewsFeedViewModel();
            DataContext = vm;

            InitializeComponent();
        }
    }

    public partial class NewsFeedViewModel : NotificationObject
    {
        private bool flyoutIsOpenState;
        public bool FlyoutIsOpenState
        {
            get { return flyoutIsOpenState; }
            set { flyoutIsOpenState = value; RaisePropertyChanged("FlyoutIsOpenState"); }
        }

        public DelegateCommand<object> ToggleFlyoutStateCommand { get; private set; }
        public DelegateCommand<object> SaveMetadataCommand { get; private set; }

        public List<Feed> RssFeeds { get; set; }
        public List<FeedItem> News { get; set; }

        private string _newRssFeed;
        public string NewRssFeed
        {
            get { return _newRssFeed; }
            set { _newRssFeed = value; RaisePropertyChanged("NewRssFeed"); }
        }



        public NewsFeedViewModel()
        {
            ToggleFlyoutStateCommand = new DelegateCommand<object>((args) => { FlyoutIsOpenState = !FlyoutIsOpenState; });
            SaveMetadataCommand = new DelegateCommand<object>((args) => SaveMetadata());

            var initFeeds = Task<List<Feed>>.Factory.StartNew(() => GetFeedsMetadata());
            News = initFeeds.ContinueWith<List<FeedItem>>((task) => GetFeeds(task.Result)).Result;
            RssFeeds = initFeeds.Result;
            //RaisePropertyChanged(() => News);
        }

        private List<FeedItem> GetFeeds(List<Feed> feeds)
        {
            return feeds.SelectMany((f) => GetFeedNews(f.Link.AbsoluteUri)).ToList();
        }

        private List<FeedItem> GetFeedNews(string url)
        {
            FeedDownload feedDownload = new FeedDownload();
            var result = feedDownload.Download(url);

            return result.Result.Feeds[0].Items.Select(x => new FeedItem()
            {
                 Author = x.Author,
                 Source = x.Source,
                 GUID = x.GUID,
                 Enclosure = x.Enclosure,
                 Title = x.Title,
                 PublishDate = x.PublishDate,
                 Category = x.Category,
                 Description = RemoveHtmlTags(x.Description),
                 Link = x.Link,
             }).ToList();


            //var cl = new RssClient();
            //var fd = cl.GetRssFeed(new Uri(url));

            //return fd.Items.Select(x => new FeedItem(){
            //     GUID = Guid.Parse(x.Link),
            //     Title = x.Title,
            //     PublishDate = x.PubDate.Value.DateTime,
            //     Description = RemoveHtmlTags(x.Description),
            //     Link = new Uri(x.Link),
            // }).ToList();
        }

        string RemoveHtmlTags(string html)
        {
            return Regex.Replace(html, "<.+?>", string.Empty);
        }


        private void SaveMetadata()
        {
            UriBuilder path = new UriBuilder(NewRssFeed);

            if (RssFeeds == null || RssFeeds.Count() == 0)
            {
                RssFeeds = new List<Feed>() { new Feed() { Link = path.Uri, Name = path.Host }};
                SaveFeedsMatadata(RssFeeds); return;
            }

            if (RssFeeds.Count(f=> f.Link != null) > 0 && RssFeeds.Count(f=>f.Link.AbsoluteUri == NewRssFeed) == 0)
            {
                RssFeeds.Add(new Feed() { Link = path.Uri, Name = path.Host });
                SaveFeedsMatadata(RssFeeds);
            }
        }

        private List<Feed> GetFeedsMetadata()
        {
            try
            {
                return DownloadData();
            }
            catch (Exception)
            {
                return new List<Feed>();
            }
        }

        private void SaveFeedsMatadata(List<Feed> feeds)
        {
            DeleteFile();
            UploadData(feeds);
        }


        #region FTP Wrapper
        /* 
         Management of Rss Feeds list in json and uploaded to an FTP Server 
        */

        List<Feed> DownloadData()
        {
            FtpClient ftpClient = CreateConnectedFtpClient();

            try
            {
                using (Stream s = ftpClient.OpenRead("rssfeeds.json"))
                {
                    // perform your transfer
                    //Movie m = JsonConvert.DeserializeObject<Movie>(json);
                    byte[] bytes = new byte[s.Length + 10];
                    int numBytesToRead = (int)s.Length;
                    int numBytesRead = 0;
                    do
                    {
                        // Read may return anything from 0 to 10. 
                        int n = s.Read(bytes, numBytesRead, 10);
                        numBytesRead += n;
                        numBytesToRead -= n;
                    } while (numBytesToRead > 0);
                    s.Close();

                    var json = Encoding.UTF8.GetString(bytes);
                    return JsonConvert.DeserializeObject<List<Feed>>(json, new JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.None });
                }
            }
            catch (Exception ex)
            {
                // Typical exceptions here are IOException, SocketException, or a FtpCommandException
                throw ex;
            }
            finally
            {
                ftpClient.Disconnect();
            }
        }

        void UploadData(List<Feed> feeds)
        {
            FtpClient ftpClient = CreateConnectedFtpClient();

            try
            {
                string json = JsonConvert.SerializeObject(feeds, new JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.None });

                using (Stream s = ftpClient.OpenWrite("rssfeeds.json"))
                {
                    // perform your transfer
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        foreach (char chr in json.ToCharArray())
                        {
                            sw.Write(chr);
                        }

                        sw.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                // Typical exceptions here are IOException, SocketException, or a FtpCommandException
                throw ex;
            }
            finally
            {
                ftpClient.Disconnect();
            }
        }

        void DeleteFile()
        {
            FtpClient ftpClient = CreateConnectedFtpClient();
            try
            {
                if (ftpClient.GetNameListing().Count(fx => fx == "rssfeeds.json") > 0)
                {
                    ftpClient.DeleteFile("rssfeeds.json");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ftpClient.Disconnect();
            }

        }

        static FtpClient CreateConnectedFtpClient()
        {
            FtpClient ftpClient = new FtpClient()
            {
                Host = ConfigurationManager.AppSettings["ftp_host"],   // "ftp.moove20.url.ph",
                Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["ftp_user"] ?? "ftp_user not found", ConfigurationManager.AppSettings["ftp_pass"] ?? "ftp_pass not found"),
                SocketKeepAlive = false
            };

            ftpClient.Connect();
            return ftpClient;
        }
        #endregion
    }
}
