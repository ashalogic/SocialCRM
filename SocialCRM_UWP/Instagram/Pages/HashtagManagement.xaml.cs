using SocialCRM_UWP.Instagram.Models;
using SocialCRM_UWP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Instagram.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HashtagManagement : Page
    {
        public HashtagManagement()
        {
            this.InitializeComponent();
        }

        private async void HashtagSearchbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                HashtagList.Items.Clear();
                HashtagListPRing.IsActive = true;
                if (!string.IsNullOrEmpty(HashtagSearchbox.Text))
                {
                    var result = await Api.InstaApi.GetTagFeedAsync(HashtagSearchbox.Text, InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(2));
                    HashtagList.Items.Clear();
                    foreach (var m in result.Value.Medias)
                    {
                        if (m.Caption != null && !string.IsNullOrWhiteSpace(m.Caption.Text))
                        {
                            switch (m.MediaType)
                            {
                                case InstaSharper.Classes.Models.InstaMediaType.Image:
                                    HashtagList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(false), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Images = m.Images });
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Video:
                                    HashtagList.Items.Add(new MediaViewModel() { IsCarousel = Helper.VisibiltyConverter(false), IsVideo = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), ViewsCount = m.ViewCount.ToString(), Videos = m.Videos });
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Carousel:
                                    HashtagList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                    break;
                                default:
                                    HashtagList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                    break;
                            }
                        }
                        else
                        {
                            switch (m.MediaType)
                            {
                                case InstaSharper.Classes.Models.InstaMediaType.Image:
                                    HashtagList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(false), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Images = m.Images });
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Video:
                                    HashtagList.Items.Add(new MediaViewModel() { IsCarousel = Helper.VisibiltyConverter(false), IsVideo = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), ViewsCount = m.ViewCount.ToString(), Videos = m.Videos });
                                    break;
                                case InstaSharper.Classes.Models.InstaMediaType.Carousel:
                                    HashtagList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                    break;
                                default:
                                    HashtagList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                    break;
                            }
                        }
                    }
                }
                HashtagListPRing.IsActive = false;
            }
        }

        private void HashtagList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _InstagramMediaModel = (MediaViewModel)e.ClickedItem;

            var container = HashtagList.ContainerFromItem(e.ClickedItem) as GridViewItem;
            if (container != null)
            {
                //find the image
                var root = (FrameworkElement)container.ContentTemplateRoot;
                var image = (UIElement)root.FindName("HshtagMediaPicture");

                //prepare the animation
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", image);
            }

            Frame.Navigate(typeof(MediaDetails), _InstagramMediaModel, new EntranceNavigationTransitionInfo());
        }
    }
}
