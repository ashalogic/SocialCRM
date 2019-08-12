using Microsoft.Toolkit.Parsers.Rss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Rss
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Main : Page
    {
        Dictionary<string, string> RssE = new Dictionary<string, string>()
        {

        };
        Dictionary<string, string> RssT = new Dictionary<string, string>()
        {

        };
        Dictionary<string, string> RssV = new Dictionary<string, string>()
        {

        };
        Dictionary<string, string> RssS = new Dictionary<string, string>()
        {

        };
        Dictionary<string, string> RssO = new Dictionary<string, string>()
        {

        };

        Dictionary<string, string> RssNews = new Dictionary<string, string>()
        {
            { "aftabnews","http://aftabnews.ir/fa/rss/allnews" },
            { "tasnimnews","https://www.tasnimnews.com/fa/rss/feed/0/7/0/%D8%A2%D8%AE%D8%B1%DB%8C%D9%86-%D8%A7%D8%AE%D8%A8%D8%A7%D8%B1-%D8%A7%D8%AE%D8%A8%D8%A7%D8%B1-%D8%B1%D9%88%D8%B2" },
            //{ "farsnews","https://www.farsnews.com/RSS" },
            { "mehrnews","https://www.mehrnews.com/rss-homepage" },
            { "isna","https://www.isna.ir/rss" },
            //{ "irna","http://www.irna.ir/fa/rss.aspx?kind=-1&area=0" },
            { "mashreghnews","https://www.mashreghnews.ir/rss" },
            { "khabaronline","http://khabaronline.ir/RSS" },
            { "tabnak","http://www.tabnak.ir/fa/rss/allnews" },
            //{ "yjc","https://www.yjc.ir/fa/rss/allnews" },
        };

        public Main()
        {
            this.InitializeComponent();



        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string feed = null;
            try
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        feed = Encoding.UTF8.GetString(await client.GetByteArrayAsync(RssNews[(e.ClickedItem as TextBlock).Tag.ToString()]));
                    }
                    catch
                    {

                    }
                }

                if (feed != null)
                {
                    var parser = new RssParser();
                    var rss = parser.Parse(feed);
                    RSSList.Items.Clear();


                    foreach (var element in rss)
                    {


                        RSSList.Items.Add(new Models.RSSViewModel() { Title = element.Title, Summary = element.Summary, Date = element.PublishDate.ToShortDateString(), Link = element.FeedUrl });
                        Console.WriteLine($"Title: {element.Title}");
                        Console.WriteLine($"Summary: {element.Summary}");
                    }
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا", ex.Message);
            }
        }

        private void RSSList_ItemClick(object sender, ItemClickEventArgs e)
        {
            website.Navigate(new Uri((e.ClickedItem as Models.RSSViewModel).Link));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string apikey = "e9363afddb35ee7d69cbc86b3ac554e1";
            string alturl = string.Empty;

            rosette_api.CAPI NewCAPI = string.IsNullOrEmpty(alturl) ? new rosette_api.CAPI(apikey) : new rosette_api.CAPI(apikey, alturl);



            var d = NewCAPI.Sentiment("ترامپ بسیار فحاش می باشد");

            string jasem = "";

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string texttovec = "";
            texttoset.Document.GetText(Windows.UI.Text.TextGetOptions.None, out texttovec);

            string functionString = String.Format("document.getElementById('text').innerText = '{0}';", texttovec);
            await webView.InvokeScriptAsync("eval", new string[] { functionString });
            string functionString2 = String.Format("eventFire(document.getElementById('go'), 'click');");
            await webView.InvokeScriptAsync("eval", new string[] { functionString2 });
        }
    }
}
