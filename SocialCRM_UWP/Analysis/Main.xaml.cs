using LiveCharts;
using LiveCharts.Uwp;
using Newtonsoft.Json;
using SocialCRM_UWP.Analysis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace SocialCRM_UWP.Analysis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Main : Page
    {
        //string hashtag = "ایرانسل";
        int limit = 60;

        //int instagramreach = 0;
        List<InstagramLightMediaModel> instagramlightmedias = new List<InstagramLightMediaModel>();
        List<InstagramLightMediaModel> topinstagramlightmedias = new List<InstagramLightMediaModel>();

        //int aparatreach = 0;
        List<AparatVideoModel> aparatvideos = new List<AparatVideoModel>();

        public Main()
        {
            this.InitializeComponent();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void GET_From_Instagram(string hashtag)
        {
            using (WebClient wc = new WebClient()) 
                {
                string url = "https://www.instagram.com/explore/tags/" + hashtag + "/?__a=1";
                var result = wc.DownloadString(url);
                dynamic stuff = JsonConvert.DeserializeObject(result);

                int step = 0;
                foreach (var media in stuff.graphql.hashtag.edge_hashtag_to_media.edges)
                {
                    url = "https://i.instagram.com/api/v1/users/" + media.node.owner.id + "/info/";
                    result = wc.DownloadString(url);
                    dynamic userstuff = JsonConvert.DeserializeObject(result);

                    var model = new InstagramLightMediaModel
                    {
                        display_url = media.node.display_url,
                        shortcode = media.node.shortcode,
                        thumbnail_src = media.node.thumbnail_src,
                        edge_liked_by_count = media.node.edge_liked_by.count,
                        edge_media_preview_like_count = media.node.edge_media_preview_like.count,
                        edge_media_to_caption_edges_node_text = media.node.edge_media_to_caption.edges[0].node.text,
                        taken_at_timestamp = media.node.taken_at_timestamp,
                        edge_media_to_comment_count = media.node.edge_media_to_comment.count,
                        is_video = media.node.is_video,
                        id = media.node.id,

                        owner_id = media.node.owner.id,

                        follower_count = userstuff.user.follower_count,
                        following_count = userstuff.user.following_count,
                        full_name = userstuff.user.full_name,
                        is_private = userstuff.user.is_private,
                        media_count = userstuff.user.media_count,
                        profile_pic_url = userstuff.user.profile_pic_url,
                        username = userstuff.user.username,
                        usertags_count = userstuff.user.usertags_count
                    };

                    try
                    {
                        if (model.is_video)
                            model.video_view_count = media.node.video_view_count;
                    }
                    catch
                    {

                    }

                    instagramlightmedias.Add(model);

                    step++;
                    if (step >= limit)
                        break;
                }

                #region Topmedia
                //foreach (var topmedia in stuff.graphql.hashtag.edge_hashtag_to_top_posts.edges)
                //{
                //    url = "https://i.instagram.com/api/v1/users/" + topmedia.node.owner.id + "/info/";
                //    result = wc.DownloadString(url);
                //    dynamic userstuff = JsonConvert.DeserializeObject(result);

                //    var model = new InstagramLightMediaModel()
                //    {
                //        display_url = topmedia.node.display_url,
                //        shortcode = topmedia.node.shortcode,
                //        thumbnail_src = topmedia.node.thumbnail_src,
                //        edge_liked_by_count = topmedia.node.edge_liked_by.count,
                //        edge_media_preview_like_count = topmedia.node.edge_media_preview_like.count,
                //        edge_media_to_caption_edges_node_text = topmedia.node.edge_media_to_caption.edges[0].node.text,
                //        taken_at_timestamp = topmedia.node.taken_at_timestamp,
                //        edge_media_to_comment_count = topmedia.node.edge_media_to_comment.count,
                //        is_video = topmedia.node.is_video,
                //        id = topmedia.node.id,

                //        owner_id = topmedia.node.owner_id,

                //        follower_count = userstuff.user.follower_count,
                //        following_count = userstuff.user.following_count,
                //        full_name = userstuff.user.full_name,
                //        is_private = userstuff.user.is_private,
                //        media_count = userstuff.user.media_count,
                //        profile_pic_url = userstuff.user.profile_pic_url,
                //        username = userstuff.user.username,
                //        usertags_count = userstuff.user.usertags_count
                //    };

                //    try
                //    {
                //        if (model.is_video)
                //            model.video_view_count = topmedia.node.video_view_count;
                //    }
                //    catch
                //    {

                //    }

                //    topinstagramlightmedias.Add(model);
                //}
                #endregion
            }
        }

        public void GET_From_Facebook(string hashtag)
        {

        }

        public void GET_From_Twitter(string hashtag)
        {

        }

        public void GET_From_Aparat(string hashtag)
        {
            using (WebClient wc = new WebClient())
            {
                string url = "https://www.aparat.com/etc/api/videoBySearch/text/" + hashtag + "/perpage/90";
                var result = wc.DownloadString(new Uri(url));
                dynamic stuff = JsonConvert.DeserializeObject(result);

                foreach (var video in stuff.videobysearch)
                {
                    url = "https://www.aparat.com/etc/api/profile/username/" + video.username;
                    result = wc.DownloadString(url);
                    dynamic userstuff = JsonConvert.DeserializeObject(result);


                    url = "https://www.aparat.com/etc/api/video/videohash/" + video.uid;
                    result = wc.DownloadString(url);
                    dynamic videostuff = JsonConvert.DeserializeObject(result);

                    var model = new AparatVideoModel();
                    model.big_poster = video.big_poster;
                    model.sdate = video.sdate;
                    model.sdate_timediff = video.sdate_timediff;
                    model.sender_name = video.sender_name;
                    model.small_poster = video.small_poster;
                    model.create_date = video.create_date;
                    model.duration = video.duration;
                    model.frame = video.frame;
                    model.id = video.id;
                    model.official = video.official;
                    model.title = video.title;
                    model.uid = video.uid;
                    model.profilePhoto = video.profilePhoto;
                    model.userid = video.userid;
                    model.username = video.username;
                    model.visit_cnt = video.visit_cnt;
                    model.followed_cnt = userstuff.profile.followed_cnt;
                    model.follower_cnt = userstuff.profile.follower_cnt;
                    model.video_cnt = userstuff.profile.video_cnt;
                    model.pic_m = userstuff.profile.pic_m;
                    model.url = "https://www.aparat.com/" + video.username;

                    model.like_cnt = videostuff.video.like_cnt;
                    aparatvideos.Add(model);
                }
            }
        }

        public void GET_From_Web(string hashtag)
        {

        }

        class datecount { public DateTime date { get; set; } public int count { get; set; } };
        public void DrawMentionsReachChart()
        {
            try
            {
                List<datecount> datecount = new List<datecount>();
                foreach (var post in instagramlightmedias)
                {

                    var key = UnixTimeStampToDateTime(double.Parse(post.taken_at_timestamp)).Date;
                    var result = datecount.Where(x => x.date == key).FirstOrDefault();
                    if (result != null)
                        result.count++;
                    else
                        datecount.Add(new Main.datecount() { count = 0, date = key });
                }
                ChartValues<double> Values = new ChartValues<double>();
                List<string> Labels = new List<string>();
                foreach (var item in datecount)
                {
                    Values.Add(item.count);
                    Labels.Add(item.date.ToShortDateString());
                }
                if (Values.Count > 0)
                {
                    var EngagementSeries = new SeriesCollection
                {
                    new LineSeries{Title = "تعداد منشن",Values = Values}
                };

                    MediaEngagementChart.AxisX.Clear();
                    MediaEngagementChart.AxisX.Add(new Axis());
                    MediaEngagementChart.AxisX[0].Labels = Labels;
                    MediaEngagementChart.AxisX[0].Margin = new Thickness(5);
                    MediaEngagementChart.AxisX[0].Separator.IsEnabled = false;
                    MediaEngagementChart.AxisX[0].Separator.Step = 1;
                    MediaEngagementChart.AxisY[0].Position = AxisPosition.RightTop;
                    MediaEngagementChart.Series = EngagementSeries;
                }
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            string s = "";
            //https://khabarfarsi.com/search


            //string html = await ldrweb.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });

            //var doc = new HtmlAgilityPack.HtmlDocument();
            //doc.LoadHtml(html);

            //var jorab = doc.DocumentNode.Descendants("script").ToList();
            //string s = jorab[6].InnerText;
            //var jsontoread = s.Split("window._sharedData = ", s.Length, StringSplitOptions.RemoveEmptyEntries);
            ////int result = HtmlAgilityPack.ev.Execute<int>("X + Y", new { X = 1, Y = 2 })


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //GET_From_Instagram(hashtag);
            //foreach (var post in instagramlightmedias)
            //{
            //    instagram_medias_listview.Items.Add(post);
            //}
            //DrawMentionsReachChart();
            //GET_From_Aparat(hashtag);
            //foreach (var video in aparatvideos)
            //{
            //    aparat_videos_listview.Items.Add(video);
            //}
        }
    }
}
