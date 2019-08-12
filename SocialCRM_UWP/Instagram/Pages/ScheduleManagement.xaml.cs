using InstaSharper.Classes.Models;
using Newtonsoft.Json;
using SocialCRM_UWP.Instagram.Models;
using SocialCRM_UWP.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Instagram.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScheduleManagement : Page
    {
        public ScheduleManagement()
        {
            this.InitializeComponent();
            //loadAsync();
        }
        string removeid = "";
        List<DateTime> dates = new List<DateTime>();
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ProgressPanel.Visibility = Visibility.Visible;
                await Load();
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }

        }
        public async static Task<BitmapImage> ImageFromBytes(Byte[] bytes)
        {
            BitmapImage image = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(bytes.AsBuffer());
                stream.Seek(0);
                await image.SetSourceAsync(stream);
            }
            return image;
        }
        private object FromByteArray(byte[] source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(source))
            {
                return formatter.Deserialize(stream);
            }
        }
        private void CView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);

            if (dates.Contains(args.Item.Date.Date))
                args.Item.Background = redBrush;
        }

        private void CView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (dates.Contains(args.AddedDates[0].Date))
                Frame.Navigate(typeof(SPDetails));
        }

        private async void SPLists_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SPListViewModel;
            removeid = item.id;
            switch (item.type)
            {
                case "Image":
                    {
                        MediaPlayerView.Visibility = Visibility.Collapsed;
                        MediaFlipView.Visibility = Visibility.Collapsed;
                        MediaImageView.Visibility = Visibility.Visible;
                        MediaImageView.Source = item.image;
                        Caption.Text = item.caption;
                    }
                    break;
                case "Video":
                    {
                        MediaPlayerView.Visibility = Visibility.Visible;
                        MediaFlipView.Visibility = Visibility.Collapsed;
                        MediaImageView.Visibility = Visibility.Collapsed;
                        MediaPlayerView.Source = MediaSource.CreateFromStream((new MemoryStream(item.video).AsRandomAccessStream()), "mp4");
                        Caption.Text = item.caption;
                    }
                    break;
                case "Carousel":
                    {
                        MediaPlayerView.Visibility = Visibility.Collapsed;
                        MediaFlipView.Visibility = Visibility.Visible;
                        MediaImageView.Visibility = Visibility.Collapsed;
                        foreach (var pic in item.carousel)
                        {
                            MediaFlipView.Items.Add(new Image() { Height = double.NaN, HorizontalAlignment = HorizontalAlignment.Stretch, Source = await ImageFromBytes(Convert.FromBase64String(pic)) });
                        }
                        Caption.Text = item.caption;
                    }
                    break;
            }
        }

        async Task Load()
        {
            if (await WebApi.Check())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("accountusername", Api.Username);
                var WResult = await WebApi.Post("http://socialcrm.ir/api/uwp/Posts_List", new HttpFormUrlEncodedContent(dic), new HttpCredentialsHeaderValue("Bearer", WebApi.Token));
                if (WResult.IsSuccessStatusCode)
                {
                    var DataResult = JsonConvert.DeserializeObject<IEnumerable<SPostViewModel>>(WResult.Content.ToString());
                    foreach (var sp in DataResult.OrderBy(x => x.datetime))
                    {
                        PersianCalendar pc = new PersianCalendar();
                        CultureInfo cultureInfo = new CultureInfo("fa-IR");
                        switch (sp.type)
                        {
                            case "Image":
                                {
                                    var post = (ImagePostViewModel)(FromByteArray(Convert.FromBase64String(sp.post)));
                                    var media = await ImageFromBytes(Convert.FromBase64String(post.images[0]));
                                    SPLists.Items.Add(new SPListViewModel()
                                    {
                                        type = sp.type,
                                        id = sp.id,
                                        image = media,
                                        caption = post.caption,
                                        date = (pc.GetYear(sp.datetime) + "/" + pc.GetMonth(sp.datetime) + "/" + pc.GetDayOfMonth(sp.datetime) + "  " + cultureInfo.DateTimeFormat.GetDayName(pc.GetDayOfWeek(sp.datetime))),
                                        time = sp.datetime.ToLongTimeString()
                                    });
                                }
                                break;
                            case "Video":
                                {
                                    var post = (VideoPostViewModel)(FromByteArray(Convert.FromBase64String(sp.post)));
                                    //var media = await ImageFromBytes(Convert.FromBase64String(post.video));
                                    SPLists.Items.Add(new SPListViewModel()
                                    {
                                        type = sp.type,
                                        id = sp.id,
                                        //image = media,
                                        video = Convert.FromBase64String(post.video),
                                        caption = post.caption,
                                        date = (pc.GetYear(sp.datetime) + "/" + pc.GetMonth(sp.datetime) + "/" + pc.GetDayOfMonth(sp.datetime) + "  " + cultureInfo.DateTimeFormat.GetDayName(pc.GetDayOfWeek(sp.datetime))),
                                        time = sp.datetime.ToLongTimeString()
                                    });
                                }
                                break;
                            case "Carousel":
                                {
                                    var post = (ImagePostViewModel)(FromByteArray(Convert.FromBase64String(sp.post)));
                                    var media = await ImageFromBytes(Convert.FromBase64String(post.images[0]));
                                    SPLists.Items.Add(new SPListViewModel()
                                    {
                                        carousel = post.images,
                                        type = sp.type,
                                        id = sp.id,
                                        image = media,
                                        caption = post.caption,
                                        date = (pc.GetYear(sp.datetime) + "/" + pc.GetMonth(sp.datetime) + "/" + pc.GetDayOfMonth(sp.datetime) + "  " + cultureInfo.DateTimeFormat.GetDayName(pc.GetDayOfWeek(sp.datetime))),
                                        time = sp.datetime.ToLongTimeString()
                                    });
                                }
                                break;
                        }
                    }



                }
                else
                {
                    await Helper.ShowMessage("خطا", "خطا");
                }
            }
            if (SPLists.Items.Count > 0)
            {
                await clicked();
                LoginPanel.Visibility = Visibility.Visible;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
        }
        async Task clicked()
        {
            var item = SPLists.Items[0] as SPListViewModel;
            removeid = item.id;
            switch (item.type)
            {
                case "Image":
                    {
                        MediaPlayerView.Visibility = Visibility.Collapsed;
                        MediaFlipView.Visibility = Visibility.Collapsed;
                        MediaImageView.Visibility = Visibility.Visible;
                        MediaImageView.Source = item.image;
                        Caption.Text = item.caption;
                    }
                    break;
                case "Video":
                    {
                        MediaPlayerView.Visibility = Visibility.Visible;
                        MediaFlipView.Visibility = Visibility.Collapsed;
                        MediaImageView.Visibility = Visibility.Collapsed;
                        MediaPlayerView.Source = MediaSource.CreateFromStream((new MemoryStream(item.video).AsRandomAccessStream()), "mp4");
                        Caption.Text = item.caption;
                    }
                    break;
                case "Carousel":
                    {
                        MediaPlayerView.Visibility = Visibility.Collapsed;
                        MediaFlipView.Visibility = Visibility.Visible;
                        MediaImageView.Visibility = Visibility.Collapsed;
                        foreach (var pic in item.carousel)
                        {
                            MediaFlipView.Items.Add(new Image() { Height = double.NaN, HorizontalAlignment = HorizontalAlignment.Stretch, Source = await ImageFromBytes(Convert.FromBase64String(pic)) });
                        }
                        Caption.Text = item.caption;
                    }
                    break;
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {

            if (await new ContentDialog()
            {
                FlowDirection = FlowDirection.RightToLeft,
                FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                Content = "آیا مایل به حذف پست می باشید؟",
                CloseButtonText = "خیر",
                PrimaryButtonText = "بلی"
            }.ShowAsync() == ContentDialogResult.Primary)
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ProgressPanel.Visibility = Visibility.Visible;
                if (await WebApi.Check())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("spid", removeid);
                    dic.Add("acname", Api.Username);

                    var WResult = await WebApi.Post("http://socialcrm.ir/api/uwp/Remove_Post", new HttpFormUrlEncodedContent(dic), new HttpCredentialsHeaderValue("Bearer", WebApi.Token));

                    if (WResult.IsSuccessStatusCode)
                    {
                        SPLists.Items.Clear();
                        await Load();

                        await Helper.ShowMessage("پست حذف شد", "پست با موفقیت حذف شد");
                    }
                    else
                    {
                        await Helper.ShowMessage("خطا", "خطا");
                        LoginPanel.Visibility = Visibility.Visible;
                        ProgressPanel.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    LoginPanel.Visibility = Visibility.Visible;
                    ProgressPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
