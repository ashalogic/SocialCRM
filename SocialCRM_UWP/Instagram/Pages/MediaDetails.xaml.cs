using InstaSharper.Classes.Models;
using SocialCRM_UWP.Instagram.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Instagram.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaDetails : Page
    {
        public MediaDetails()
        {
            this.InitializeComponent();

            KeyboardAccelerator GoBack = new KeyboardAccelerator
            {
                Key = VirtualKey.Back
            };
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left
            };
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);
            // ALT routes here
            //AltLeft.Modifiers = VirtualKeyModifiers.Menu;
        }
        public string MID = "";
        public InstaMediaType IMT;
        public string _username = "";
        MediaViewModel _InstagramMediaModel;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _InstagramMediaModel = (MediaViewModel)e.Parameter;

            if (!_InstagramMediaModel.IsOwner)
            {
                DeleteBtn.Visibility = Visibility.Collapsed;
                MediasCount.Visibility = Visibility.Collapsed;
                FollowersCount.Visibility = Visibility.Collapsed;
                FollowingsCount.Visibility = Visibility.Collapsed;
            }

            switch (_InstagramMediaModel.InstaMediaType)
            {
                case InstaMediaType.Image:
                    MediaImageView.Source = new BitmapImage(new Uri(_InstagramMediaModel.Images[0].URI));
                    {
                        ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
                        imageAnimation.TryStart(MediaImageView);
                    }
                    MediaPlayerView.Visibility = Visibility.Collapsed;
                    MediaFlipView.Visibility = Visibility.Collapsed;
                    break;
                case InstaMediaType.Video:
                    MediaPlayerView.Source = MediaSource.CreateFromUri(new Uri(_InstagramMediaModel.Videos[0].Url));
                    {
                        ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
                        imageAnimation.TryStart(MediaPlayerView);
                    }
                    MediaImageView.Visibility = Visibility.Collapsed;
                    MediaFlipView.Visibility = Visibility.Collapsed;
                    break;
                case InstaMediaType.Carousel:
                    foreach (var c in _InstagramMediaModel.Carousel)
                    {
                        switch (c.MediaType)
                        {
                            case InstaMediaType.Image:
                                MediaFlipView.Items.Add(new Image() { Height = double.NaN, HorizontalAlignment = HorizontalAlignment.Stretch, Source = new BitmapImage(new Uri(c.Images[0].URI)) });
                                break;
                            case InstaMediaType.Video:
                                MediaFlipView.Items.Add(new MediaPlayerElement() { Source = MediaSource.CreateFromUri(new Uri(_InstagramMediaModel.Videos[0].Url)), FlowDirection = FlowDirection.LeftToRight, AreTransportControlsEnabled = true, Height = double.NaN, HorizontalAlignment = HorizontalAlignment.Stretch });
                                break;
                        }
                    }
                    {
                        ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
                        imageAnimation.TryStart(MediaFlipView);
                    }
                    MediaImageView.Visibility = Visibility.Collapsed;
                    MediaPlayerView.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            Caption.Text = _InstagramMediaModel.UserName + " " + _InstagramMediaModel.Caption;
            LikesCount.Text = _InstagramMediaModel.LikesCount;
            CommentsCount.Text = _InstagramMediaModel.CommentsCount;
            UserName.Text = "@" + _InstagramMediaModel.UserName;
            _username = _InstagramMediaModel.UserName;
            MID = _InstagramMediaModel.Id;
            IMT = _InstagramMediaModel.InstaMediaType;
            //ViewCount.Text = "0";
            switch (_InstagramMediaModel.HasLiked)
            {
                case true:
                    Like.Visibility = Visibility.Collapsed;
                    UnLike.Visibility = Visibility.Visible;
                    break;
                case false:
                    Like.Visibility = Visibility.Visible;
                    UnLike.Visibility = Visibility.Collapsed;
                    break;
            }

            BackButton.IsEnabled = this.Frame.CanGoBack;
            LoadComments(_InstagramMediaModel.Id, _InstagramMediaModel.UserName);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        async Task LoadComments(string mediaid, string username)
        {
            MediasCommentsListPRing.IsActive = true;
            var _UserInfo = await Api.InstaApi.GetUserInfoByUsernameAsync(username);
            ProfielPic.ProfilePicture = new BitmapImage(new Uri(_UserInfo.Value.ProfilePicUrl));
            MediasCount.Text = _UserInfo.Value.MediaCount.ToString();
            FollowersCount.Text = _UserInfo.Value.FollowerCount.ToString();
            FollowingsCount.Text = _UserInfo.Value.FollowingCount.ToString();

            var _Result = await Api.InstaApi.GetMediaCommentsAsync(mediaid, InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(2));
            MediasCommentsList.Items.Clear();
            foreach (var c in _Result.Value.Comments)
            {
                MediasCommentsList.Items.Add(new CommentViewModel() { CommentId = c.Pk.ToString(), MediaId = mediaid, UserId = c.UserId.ToString(), Date = c.CreatedAt.ToShortDateString(), LikesCount = c.LikesCount.ToString(), UserName = c.User.UserName, ProfilePic = c.User.ProfilePicture, Text = c.Text });
            }
            MediasCommentsListPRing.IsActive = false;
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog locationPromptDialog = new ContentDialog
            {
                FlowDirection = FlowDirection.RightToLeft,
                FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                Title = "تایید حذف پست",
                Content = "آیا مایل به حذف این پست می باشید؟",
                CloseButtonText = "خیر",
                PrimaryButtonText = "بلی"
            };

            ContentDialogResult _result = await locationPromptDialog.ShowAsync();
            if (_result == ContentDialogResult.Primary)
            {
                DeleteBtn.IsEnabled = false;
                var Result = await Api.InstaApi.DeleteMediaAsync(MID, IMT);
                ContentDialog Result_Dialog = new ContentDialog
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                    Content = "پست با موفقیت پاک شد",
                    CloseButtonText = "باشه"
                };

                await Result_Dialog.ShowAsync();
                On_BackRequested();
            }
        }

        private async void Like_Click(object sender, RoutedEventArgs e)
        {
            Like.IsEnabled = false;
            var result = await Api.InstaApi.LikeMediaAsync(MID.ToString());
            if (result.Succeeded)
            {
                Like.Visibility = Visibility.Collapsed;
                UnLike.Visibility = Visibility.Visible;
            }
            Like.IsEnabled = true;
        }

        private async void UnLike_Click(object sender, RoutedEventArgs e)
        {
            UnLike.IsEnabled = false;
            var result = await Api.InstaApi.UnLikeMediaAsync(MID.ToString());
            if (result.Succeeded)
            {
                Like.Visibility = Visibility.Visible;
                UnLike.Visibility = Visibility.Collapsed;
            }
            UnLike.IsEnabled = true;
        }

        private async void SendComment_Click(object sender, RoutedEventArgs e)
        {
            SendComment.IsEnabled = false;
            string txt = "";
            CommentText.Document.GetText(Windows.UI.Text.TextGetOptions.None, out txt);
            if (!string.IsNullOrWhiteSpace(txt))
            {
                var result = await Api.InstaApi.CommentMediaAsync(MID, txt);
                if (result.Succeeded)
                {
                    await LoadComments(MID, _username);
                }
            }
            SendComment.IsEnabled = true;
        }

        private async void ReShare_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            string CaptionToSend = _InstagramMediaModel.Caption;
            switch (_InstagramMediaModel.InstaMediaType)
            {
                case InstaMediaType.Image:
                    {
                        Uri source = new Uri(_InstagramMediaModel.Images[0].URI);
                        StorageFile destinationFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(DateTime.Now.ToFileTime().ToString() + ".jpg", CreationCollisionOption.GenerateUniqueName);
                        BackgroundDownloader downloader = new BackgroundDownloader();
                        DownloadOperation download = downloader.CreateDownload(source, destinationFile);
                        var downloadresult = await download.StartAsync();
                        var Result = await Api.InstaApi.UploadPhotoAsync(new InstaImage(destinationFile.Path, _InstagramMediaModel.Images[0].Width, _InstagramMediaModel.Images[0].Height), CaptionToSend);
                        if (Result.Succeeded)
                        {
                            ContentDialog Result_Dialog = new ContentDialog
                            {
                                FlowDirection = FlowDirection.RightToLeft,
                                FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                                Content = "پست با موفقیت بازنشر یافت",
                                CloseButtonText = "باشه"
                            };
                            await Result_Dialog.ShowAsync();
                            On_BackRequested();
                        }
                        else
                        {
                            ContentDialog Result_Dialog = new ContentDialog
                            {
                                FlowDirection = FlowDirection.RightToLeft,
                                FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                                Content = "خطا در بازنشر پست",
                                CloseButtonText = "باشه"
                            };
                            await Result_Dialog.ShowAsync();
                        }
                        await destinationFile.DeleteAsync();
                    }
                    break;
                case InstaMediaType.Video:
                    {
                        Uri source = new Uri(_InstagramMediaModel.Videos[0].Url);
                        Uri source2 = new Uri(_InstagramMediaModel.Images[0].URI);
                        StorageFile destinationFile = await DownloadsFolder.CreateFileAsync(DateTime.Now.ToFileTime().ToString() + ".mp4", CreationCollisionOption.GenerateUniqueName);
                        StorageFile destinationFile2 = await DownloadsFolder.CreateFileAsync(DateTime.Now.ToFileTime().ToString() + ".jpg", CreationCollisionOption.GenerateUniqueName);
                        BackgroundDownloader downloader = new BackgroundDownloader();
                        DownloadOperation download = downloader.CreateDownload(source, destinationFile);
                        DownloadOperation download2 = downloader.CreateDownload(source2, destinationFile2);
                        var downloadresult = await download.StartAsync();
                        var downloadresult2 = await download2.StartAsync();

                        var Result = await Api.InstaApi.UploadVideoAsync(new InstaVideo(destinationFile.Path, _InstagramMediaModel.Videos[0].Width, _InstagramMediaModel.Videos[0].Height, _InstagramMediaModel.Videos[0].Type), new InstaImage(destinationFile2.Path, _InstagramMediaModel.Images[0].Width, _InstagramMediaModel.Images[0].Height), CaptionToSend);
                        if (Result.Succeeded)
                        {
                            ContentDialog Result_Dialog = new ContentDialog
                            {
                                FlowDirection = FlowDirection.RightToLeft,
                                FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                                Content = "پست با موفقیت بازنشر یافت",
                                CloseButtonText = "باشه"
                            };
                            await Result_Dialog.ShowAsync();
                            On_BackRequested();
                        }
                        else
                        {
                            ContentDialog Result_Dialog = new ContentDialog
                            {
                                FlowDirection = FlowDirection.RightToLeft,
                                FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                                Content = "خطا در بازنشر پست",
                                CloseButtonText = "باشه"
                            };
                            await Result_Dialog.ShowAsync();
                        }
                    }
                    break;
                case InstaMediaType.Carousel:
                    {
                        ContentDialog Result_Dialog = new ContentDialog
                        {
                            FlowDirection = FlowDirection.RightToLeft,
                            FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                            Content = "بازنشر گالری به زودی اضافه میگردد",
                            CloseButtonText = "باشه"
                        };
                        await Result_Dialog.ShowAsync();
                    }
                    break;
            }
            this.IsEnabled = true;
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            switch (_InstagramMediaModel.InstaMediaType)
            {
                case InstaMediaType.Image:
                    {
                        {
                            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                            savePicker.FileTypeChoices.Add("Picture", new List<string>() { ".jpg" });
                            savePicker.SuggestedFileName = DateTime.Now.ToFileTime().ToString();
                            var file = await savePicker.PickSaveFileAsync();
                            if (file != null)
                            {
                                Uri source = new Uri(_InstagramMediaModel.Images[0].URI);
                                BackgroundDownloader downloader = new BackgroundDownloader();
                                DownloadOperation download = downloader.CreateDownload(source, file);
                                var downloadresult = await download.StartAsync();
                            }
                        }
                    }
                    break;
                case InstaMediaType.Video:
                    {
                        var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                        savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                        savePicker.FileTypeChoices.Add("Video", new List<string>() { ".mp4" });
                        savePicker.SuggestedFileName = DateTime.Now.ToFileTime().ToString();
                        var file = await savePicker.PickSaveFileAsync();
                        if (file != null)
                        {
                            Uri source = new Uri(_InstagramMediaModel.Videos[0].Url);
                            BackgroundDownloader downloader = new BackgroundDownloader();
                            DownloadOperation download = downloader.CreateDownload(source, file);
                            var downloadresult = await download.StartAsync();
                        }
                    }
                    break;
                case InstaMediaType.Carousel:
                    break;
            }
            this.IsEnabled = true;
        }
    }
}
