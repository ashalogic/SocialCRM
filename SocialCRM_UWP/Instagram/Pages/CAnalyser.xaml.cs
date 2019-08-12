﻿using LiveCharts;
using LiveCharts.Uwp;
using SocialCRM_UWP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Instagram.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CAnalyser : Page
    {
        public CAnalyser()
        {
            this.InitializeComponent();
        }
        string uname = "";
        private async void HashtagSearchbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (!string.IsNullOrEmpty(HashtagSearchbox.Text))
                {
                    uname = HashtagSearchbox.Text;

                    //try
                    //{
                    LoginPanel.Visibility = Visibility.Collapsed;
                    ProgressPanel.Visibility = Visibility.Visible;
                    await LoadUserProfile();
                    await DrawChart();
                    LoginPanel.Visibility = Visibility.Visible;
                    ProgressPanel.Visibility = Visibility.Collapsed;
                    //}
                    //catch (Exception ex)
                    //{
                    //    await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
                    //}

                }
            }
        }
        async Task DrawChart()
        {
            //try
            //{
            var Medias_24 = (await Api.InstaApi.GetUserMediaAsync(uname, InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(2))).Value;
            List<string> CountLabels = new List<string>();
            if (Medias_24.Count >= 24)
            {
                try
                {
                    try
                    {
                        #region Calc Value

                        var Temp_TOTALLIKE = Medias_24.Sum(x => x.LikesCount);
                        var Temp_TOTAL_COMMENT = Medias_24.Sum(x => int.Parse(x.CommentsCount));
                        var Temp_TOTAL_POST = Medias_24.Count;
                        var Temp_FOLLOWER_COUNT = double.Parse(FollowersCount.Text ?? "0");
                        double Temp_ENGAGEMENT = (((Temp_TOTALLIKE + Temp_TOTAL_COMMENT) / Temp_TOTAL_POST) / Temp_FOLLOWER_COUNT) * 100;
                        double Temp_CPC = 0.01;
                        double Temp_EXTRA_CPC = 0;
                        double Temp_EARNINGS = 0;

                        if (Temp_FOLLOWER_COUNT > 100000)
                        {
                            var extra_user_followers = Temp_FOLLOWER_COUNT - 100000;
                            Temp_CPC = 450;
                            Temp_EXTRA_CPC = 0.002 * extra_user_followers;
                        }
                        else if (Temp_FOLLOWER_COUNT < 100000 && Temp_FOLLOWER_COUNT > 10000)
                        {
                            var extra_user_followers = Temp_FOLLOWER_COUNT - 10000;
                            Temp_CPC = 100;
                            Temp_EXTRA_CPC = 0.005 * extra_user_followers;
                        }
                        else
                        {
                            Temp_CPC = 0.01 * Temp_FOLLOWER_COUNT;
                            Temp_EXTRA_CPC = 0;
                        }

                        if (Temp_ENGAGEMENT > 1.5)
                        {
                            Temp_EARNINGS = 0.002 * Temp_FOLLOWER_COUNT + (1.5 * Math.Sqrt(Temp_ENGAGEMENT - 1.5));
                        }
                        else
                        {
                            Temp_EARNINGS = 0.002 * Temp_FOLLOWER_COUNT;
                        }

                        if (Temp_FOLLOWER_COUNT < 1000000 && Temp_FOLLOWER_COUNT > 100000)
                        {
                            Temp_EARNINGS = Temp_EARNINGS * 1.5;
                        }
                        else if (Temp_FOLLOWER_COUNT < 100000 && Temp_FOLLOWER_COUNT > 50000)
                        {
                            Temp_EARNINGS = Temp_EARNINGS * 2;
                        }
                        else if (Temp_FOLLOWER_COUNT < 50000 && Temp_FOLLOWER_COUNT > 25000)
                        {
                            Temp_EARNINGS = Temp_EARNINGS * 2.5;
                        }
                        else if (Temp_FOLLOWER_COUNT < 25000 && Temp_FOLLOWER_COUNT > 10000)
                        {
                            Temp_EARNINGS = Temp_EARNINGS * 3;
                        }
                        else if (Temp_FOLLOWER_COUNT < 10000)
                        {
                            Temp_EARNINGS = Temp_EARNINGS * 4;
                        }

                        ProfileValue.Text = (Math.Round(Temp_EARNINGS * 1000)).ToString("##,#") + " تومان ";
                        #endregion
                    }
                    finally
                    {

                    }
                    try
                    {
                        #region MediaEngagementChart
                        ChartValues<double> EngagementValues = new ChartValues<double>();
                        int i = 1;
                        foreach (var m in Medias_24)
                        {
                            EngagementValues.Add(Math.Round(((m.LikesCount * 0.4) + (double.Parse(m.CommentsCount ?? "0") * 0.6)) / (double.Parse(FollowersCount.Text ?? "0")) * 100, 3));
                            CountLabels.Add(i.ToString());
                            i++;
                        }
                        if (EngagementValues.Count > 0)
                        {
                            var EngagementSeries = new SeriesCollection
                {
                    new LineSeries{Title = "ضریب تاثیر پست",Values = EngagementValues}
                };
                            MediaEngagementChart.AxisX[0].Labels = CountLabels;
                            MediaEngagementChart.AxisX[0].Margin = new Thickness(5);
                            MediaEngagementChart.AxisX[0].Separator.IsEnabled = false;
                            MediaEngagementChart.AxisX[0].Separator.Step = 1;
                            MediaEngagementChart.AxisY[0].Position = AxisPosition.RightTop;
                            MediaEngagementChart.Series = EngagementSeries;
                        }
                        #endregion
                    }
                    finally
                    {

                    }
                    try
                    {
                        #region MediaTypeChart
                        ChartValues<double> TypeValues = new ChartValues<double>();
                        List<string> TypeLabels = new List<string>();
                        int Video = 0;
                        int Carousel = 0;
                        int Picture = 0;
                        foreach (var m in Medias_24)
                        {
                            switch (m.MediaType)
                            {
                                case InstaSharper.Classes.Models.InstaMediaType.Image:
                                    Picture++;
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Video:
                                    Video++;
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Carousel:
                                    Carousel++;
                                    break;
                                default:
                                    break;
                            }
                        }
                        var TypeSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "ویدئو",
                    Values = new ChartValues<int>(){Video}
                },
                new PieSeries
                {
                    Title = "تصویر",
                    Values = new ChartValues<int>(){ Picture }
                },
                new PieSeries
                {
                    Title = "گالری",
                    Values = new ChartValues<int>(){ Carousel }
                }
            };
                        MediaTypeChart.Series = TypeSeries;
                        #endregion
                    }
                    finally
                    {

                    }
                    try
                    {
                        #region MediaTypeFavChart
                        ChartValues<double> TypeFavValues = new ChartValues<double>();
                        List<double> LikedVideo = new List<double>();
                        List<double> LikedCarousel = new List<double>();
                        List<double> LikedPicture = new List<double>();
                        List<double> AllLiked = new List<double>();
                        foreach (var m in Medias_24)
                        {
                            switch (m.MediaType)
                            {
                                case InstaSharper.Classes.Models.InstaMediaType.Image:
                                    LikedPicture.Add(m.LikesCount);
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Video:
                                    LikedVideo.Add(m.LikesCount);
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Carousel:
                                    LikedCarousel.Add(m.LikesCount);
                                    break;
                                default:
                                    break;
                            }
                        }

                        var TypeFavSeries = new SeriesCollection
            {
                new RowSeries
                {
                    Title = "ویدئو",
                    Values = new ChartValues<double>(){ Math.Round(LikedVideo.Average(),2) }
                },
                new RowSeries
                {
                    Title = "تصویر",
                    Values = new ChartValues<double>(){ Math.Round(LikedPicture.Average(),2) }
                },
                new RowSeries
                {
                    Title = "گالری",
                    Values = new ChartValues<double>(){ Math.Round(LikedCarousel.Average(), 2) }
                }
            };
                        MediaTypeFavChart.AxisX[0].Title = "میانگین لایک";
                        MediaTypeFavChart.AxisX[0].Margin = new Thickness(5);
                        MediaTypeFavChart.AxisY.Clear();
                        MediaTypeFavChart.Hoverable = true;
                        MediaTypeFavChart.AxisX[0].Separator.IsEnabled = true;
                        MediaTypeFavChart.Series = TypeFavSeries;
                        #endregion
                    }
                    finally
                    {

                    }
                    try
                    {
                        #region MediaLikeCommentChart
                        ChartValues<double> CommentValues = new ChartValues<double>();
                        ChartValues<double> LikeValues = new ChartValues<double>();
                        foreach (var m in Medias_24)
                        {
                            LikeValues.Add(m.LikesCount);
                            CommentValues.Add(int.Parse(m.CommentsCount ?? "0"));
                        }
                        var MediaLikeCommentSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "لایک",
                    Values = LikeValues,
                    ScalesYAt=0
                },
                new LineSeries
                {
                    Title = "کامنت",
                    Values = CommentValues,
                    ScalesYAt=1
                }
            };
                        MediaLikeCommentChart.Series = MediaLikeCommentSeries;
                        MediaLikeCommentChart.AxisY.Clear();
                        MediaLikeCommentChart.AxisY.Add(new Axis() { Title = "تعداد لایک", Position = AxisPosition.LeftBottom });
                        MediaLikeCommentChart.AxisY.Add(new Axis() { Title = "تعداد نظرات", Position = AxisPosition.RightTop });
                        MediaLikeCommentChart.AxisY[0].Separator.IsEnabled = false;
                        MediaLikeCommentChart.AxisY[1].Separator.IsEnabled = false;
                        MediaLikeCommentChart.AxisX[0].Labels = CountLabels;
                        MediaLikeCommentChart.AxisX[0].Separator.IsEnabled = false;
                        MediaLikeCommentChart.AxisX[0].Separator.Step = 1;
                        #endregion
                    }
                    finally
                    {

                    }
                    try
                    {
                        #region PostCountChart
                        ChartValues<double> PostCountValues = new ChartValues<double>();
                        List<string> DateLabels = new List<string>();
                        for (int jasemindex = 0; jasemindex < 32; jasemindex++)
                        {
                            PostCountValues.Add(0);
                            DateLabels.Add((jasemindex + 1).ToString());
                        }
                        foreach (var m in Medias_24)
                        {
                            PostCountValues[int.Parse(m.TakenAt.Day.ToString())]++;
                        }
                        var PostCountSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "تعداد",
                    Values = PostCountValues,
                    ScalesYAt=0
                }
            };
                        PostCountChart.Series = PostCountSeries;
                        PostCountChart.AxisX[0].Labels = DateLabels;
                        PostCountChart.AxisX[0].Separator.IsEnabled = true;
                        PostCountChart.AxisX[0].Separator.Step = 1;
                        #endregion
                    }
                    finally
                    {

                    }

                }
                catch
                {

                }
                finally
                {

                }
            }
            else
            {
                await Helper.ShowMessage("خطا", "تعداد پست ها کافی نیست");
            }
            //Hashtag Feq
            //    List<string> _HashtagssList = new List<string>();
            //    foreach (var m in Medias_24)
            //    {
            //        var hlist = m.Caption.Text.Split('#');
            //        if (hlist.Count() > 1)
            //        {
            //            for (int hindex = 1; hindex < hlist.Count(); hindex++)
            //            {
            //                _HashtagssList.Add(hlist[hindex].Split(' ')[0]);
            //            }
            //        }
            //    }
            //    SeriesCollection _HashtagssSeriesCollection = new SeriesCollection
            //{
            //    new ScatterSeries
            //    {
            //        Values = new ChartValues<ScatterPoint>
            //        {
            //            new ScatterPoint(5, 5, 20),
            //            new ScatterPoint(3, 4, 80),
            //            new ScatterPoint(7, 2, 20),
            //            new ScatterPoint(2, 6, 60),
            //            new ScatterPoint(8, 2, 70)
            //        },
            //        MinPointShapeDiameter = 15,
            //        MaxPointShapeDiameter = 45
            //    }
            //};
            //}
            //catch (Exception ex)
            //{
            //    await Helper.ShowMessage("خطای پیش بینی نشده", ex.Message);
            //}
        }
        async Task LoadUserProfile()
        {
            //try
            //{
            var inf = await Api.InstaApi.GetUserInfoByUsernameAsync(uname);
            ProfileFullname.Text = inf.Value.FullName;
            ProfileUsername.Text = "@" + inf.Value.Username;
            ProfilePic.ProfilePicture = new BitmapImage(new Uri(inf.Value.ProfilePicUrl));
            MediasCount.Text = inf.Value.MediaCount.ToString();
            FollowersCount.Text = inf.Value.FollowerCount.ToString();
            FollowingsCount.Text = inf.Value.FollowingCount.ToString();



            //}
            //catch (Exception ex)
            //{
            //    await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            //}
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
        }
    }
}
