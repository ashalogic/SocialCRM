using SocialCRM_UWP.Instagram.Models;
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
using Windows.UI.Xaml.Navigation;
using SocialCRM_UWP.Utility;
using Windows.UI.Xaml.Media.Animation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.FileProperties;
using Windows.Media.Core;
using InstaSharper.Classes.Models;
using Windows.Storage.Streams;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SocialCRM_UWP.Instagram.Pages
{
    public sealed partial class MediaManagement : Page
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        StorageFolder accountsFolder;
        StorageFolder instagramFolder;
        //StorageFolder scheduledFolder;
        StorageFolder tempFolder;

        public MediaManagement()
        {
            this.InitializeComponent();

            DPicker.MinDate = DateTime.Now;
            DPicker.MaxDate = DateTime.Now.AddDays(60);
            DPicker.CalendarIdentifier = Windows.Globalization.CalendarIdentifiers.Persian;


            PrepareFolders();
            Load();
        }

        //Prepare Folders
        async void PrepareFolders()
        {
            try
            {
                accountsFolder = await localFolder.GetFolderAsync("Accounts");
                instagramFolder = await accountsFolder.GetFolderAsync("Instagram");
                //scheduledFolder = await instagramFolder.GetFolderAsync("Scheduled");
                tempFolder = await instagramFolder.GetFolderAsync("Temp");
                await Helper.DeleteTempFiles(tempFolder, "_tmpPic");
                await Helper.DeleteTempFiles(tempFolder, "_tmpVid");
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }
        //Load Data
        async void Load()
        {
            try
            {
                //UserMediasListPRing.IsActive = true;
                UserMediasList.Items.Clear();
                var userMedia = await Api.InstaApi.GetUserMediaAsync(Api.Username, InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(1));
                foreach (var m in userMedia.Value)
                {
                    if (m.Caption != null && !string.IsNullOrWhiteSpace(m.Caption.Text))
                    {
                        switch (m.MediaType)
                        {
                            case InstaSharper.Classes.Models.InstaMediaType.Image:
                                UserMediasList.Items.Add(new MediaViewModel() { IsCarousel = Helper.VisibiltyConverter(false), IsVideo = Helper.VisibiltyConverter(false), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Images = m.Images });
                                break;
                            case InstaSharper.Classes.Models.InstaMediaType.Video:
                                UserMediasList.Items.Add(new MediaViewModel() { IsCarousel = Helper.VisibiltyConverter(false), IsVideo = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), ViewsCount = m.ViewCount.ToString(), Videos = m.Videos });
                                break;
                            case InstaSharper.Classes.Models.InstaMediaType.Carousel:
                                UserMediasList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                break;
                            default:
                                UserMediasList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = m.Caption.Text, CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                break;
                        }
                    }
                    else
                    {
                        switch (m.MediaType)
                        {
                            case InstaSharper.Classes.Models.InstaMediaType.Image:
                                UserMediasList.Items.Add(new MediaViewModel() { IsCarousel = Helper.VisibiltyConverter(false), IsVideo = Helper.VisibiltyConverter(false), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Images = m.Images });
                                break;
                            case InstaSharper.Classes.Models.InstaMediaType.Video:
                                UserMediasList.Items.Add(new MediaViewModel() { IsCarousel = Helper.VisibiltyConverter(false), IsVideo = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), ViewsCount = m.ViewCount.ToString(), Videos = m.Videos });
                                break;
                            case InstaSharper.Classes.Models.InstaMediaType.Carousel:
                                UserMediasList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                break;
                            default:
                                UserMediasList.Items.Add(new MediaViewModel() { IsVideo = Helper.VisibiltyConverter(false), IsCarousel = Helper.VisibiltyConverter(true), HasLiked = m.HasLiked, Thumbnail = m.Carousel[0].Images[0].URI, IsOwner = true, InstaMediaType = m.MediaType, Id = m.InstaIdentifier, UserName = m.User.UserName, Caption = "", CommentsCount = m.CommentsCount, LikesCount = m.LikesCount.ToString(), Carousel = m.Carousel });
                                break;
                        }
                    }
                }
                //UserMediasListPRing.IsActive = false;
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }
        //Remove Item
        private void UserMedias_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _InstagramMediaModel = (MediaViewModel)e.ClickedItem;

            var container = UserMediasList.ContainerFromItem(e.ClickedItem) as GridViewItem;
            if (container != null)
            {
                //find the image
                var root = (FrameworkElement)container.ContentTemplateRoot;
                var image = (UIElement)root.FindName("MediaPicture");

                //prepare the animation
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", image);
            }

            Frame.Navigate(typeof(MediaDetails), _InstagramMediaModel, new EntranceNavigationTransitionInfo());
        }
        //Clear
        private async void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            await Helper.DeleteTempFiles(tempFolder, "_tmpPic");
            MediasforUploadList.Items.Clear();
        }
        //show date
        private void ClockHideBtn_Click(object sender, RoutedEventArgs e)
        {

            if (TPicker.Visibility == Visibility.Visible)
            {
                TPicker.Visibility = Visibility.Collapsed;
                DPicker.Visibility = Visibility.Collapsed;

            }
            else
            {
                TPicker.Visibility = Visibility.Visible;
                DPicker.Visibility = Visibility.Visible;
            }

        }
        //Add image to tempFolder for upload
        private async void AddUploadFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var cnow = DateTime.Now.ToFileTime();
                StorageFile _Target = await tempFolder.CreateFileAsync("_tmpPic" + cnow + file.FileType, CreationCollisionOption.ReplaceExisting);
                await file.CopyAndReplaceAsync(_Target);
                MediasforUploadList.Items.Add(new Image() { Tag = "_tmpPic" + cnow + file.FileType, Source = new BitmapImage(new Uri(_Target.Path)), Width = 92, Height = 92 });
            }
        }
        //Remove image on click
        private async void MediasforUploadList_ItemClick(object sender, ItemClickEventArgs e)
        {
            StorageFile _Target = await tempFolder.GetFileAsync(((Image)e.ClickedItem).Tag.ToString());
            MediasforUploadList.Items.Remove(e.ClickedItem);
            await _Target.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }
        //upload the pictures
        private async void UploadPostBtn_Click(object sender, RoutedEventArgs e)
        {
            switch ((FileTypePivot.SelectedItem as PivotItem).Tag)
            {
                case "Pic":
                    {
                        if (MediasforUploadList.Items.Count < 1)
                            return;

                        if (await new ContentDialog()
                        {
                            FlowDirection = FlowDirection.RightToLeft,
                            FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                            Content = "آیا مایل به ارسال تصاویر می باشید؟",
                            CloseButtonText = "خیر",
                            PrimaryButtonText = "بلی"
                        }.ShowAsync() == ContentDialogResult.Primary)
                        {
                            //AddPanelPRing.IsActive = true;
                            //AddPanel.Visibility = Visibility.Collapsed;
                            await UploadPictures();
                            //AddPanelPRing.IsActive = false;
                            //AddPanel.Visibility = Visibility.Visible;
                        }
                    }
                    break;
                case "Vid":
                    {
                        if (VideoPreview.Source == null)
                            return;

                        if (await new ContentDialog()
                        {
                            FlowDirection = FlowDirection.RightToLeft,
                            FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                            Content = "آیا مایل به ارسال ویدئو می باشید؟",
                            CloseButtonText = "خیر",
                            PrimaryButtonText = "بلی"
                        }.ShowAsync() == ContentDialogResult.Primary)
                        {
                            //AddPanelPRing.IsActive = true;
                            //AddPanel.Visibility = Visibility.Collapsed;
                            await UploadVideo();
                            //AddPanelPRing.IsActive = false;
                            //AddPanel.Visibility = Visibility.Visible;
                        }
                    }
                    break;
            }
            Load();
        }
        //Upload Pictures
        async Task UploadPictures()
        {
            if (TPicker.Visibility == Visibility.Collapsed)
            {
                var _Target = (await tempFolder.GetFilesAsync()).Where(x => x.DisplayName.StartsWith("_tmpPic")).ToList();
                if (_Target.Count > 1 && _Target.Count <= 10)
                {
                    InstaSharper.Classes.Models.InstaImage[] imgs = new InstaSharper.Classes.Models.InstaImage[_Target.Count];
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        ImageProperties imageProperties = await _Target[i].Properties.GetImagePropertiesAsync();
                        imgs[i] = new InstaSharper.Classes.Models.InstaImage();
                        imgs[i].URI = _Target[i].Path;
                        imgs[i].Width = int.Parse(imageProperties.Width.ToString());
                        imgs[i].Height = int.Parse(imageProperties.Height.ToString());
                    }
                    string PCap = "";
                    PostCaption.Document.GetText(Windows.UI.Text.TextGetOptions.None, out PCap);
                    var UpResult = await Api.InstaApi.UploadPhotosAlbumAsync(imgs, PCap);
                    ContentDialog End = new ContentDialog
                    {
                        FlowDirection = FlowDirection.RightToLeft,
                        FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                        Content = "گالری با موفقیت ارسال شد",
                        CloseButtonText = "باشه"
                    };
                    await End.ShowAsync();
                }
                else if (_Target.Count == 1)
                {
                    string PCap = "";
                    PostCaption.Document.GetText(Windows.UI.Text.TextGetOptions.None, out PCap);
                    ImageProperties imageProperties = await _Target[0].Properties.GetImagePropertiesAsync();
                    var UpResult = await Api.InstaApi.UploadPhotoAsync(new InstaSharper.Classes.Models.InstaImage(_Target[0].Path, int.Parse(imageProperties.Width.ToString()), int.Parse(imageProperties.Height.ToString())), PCap);
                    ContentDialog End = new ContentDialog
                    {
                        FlowDirection = FlowDirection.RightToLeft,
                        FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                        Content = "پست با موفقیت ارسال شد",
                        CloseButtonText = "باشه"
                    };
                    await End.ShowAsync();
                }
            }
            else
            {
                if (await WebApi.Check())
                {
                    var _Target = (await tempFolder.GetFilesAsync()).Where(x => x.DisplayName.StartsWith("_tmpPic")).ToList();
                    if (_Target.Count > 1 && _Target.Count <= 10)
                    {
                        string PCap = "";
                        PostCaption.Document.GetText(Windows.UI.Text.TextGetOptions.None, out PCap);
                        ImagePostViewModel imagePostViewModel = new ImagePostViewModel();
                        imagePostViewModel.caption = PCap;

                        foreach (var img in _Target)
                        {
                            var filestream = await img.OpenStreamForReadAsync();
                            var filebytes = new byte[(int)filestream.Length];
                            filestream.Read(filebytes, 0, (int)filestream.Length);
                            imagePostViewModel.images.Add(Convert.ToBase64String(filebytes));
                        }

                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", InstaMediaType.Carousel.ToString());
                        dic.Add("accountusername", Api.Username);
                        dic.Add("data", Convert.ToBase64String(ToByteArray(imagePostViewModel)));
                        dic.Add("date", (new DateTime(DPicker.Date.Value.Year, DPicker.Date.Value.Month, DPicker.Date.Value.Day, TPicker.Time.Hours, TPicker.Time.Minutes, TPicker.Time.Seconds)).ToBinary().ToString());

                        var WResult = await WebApi.Post("http://socialcrm.ir/api/uwp/Add_Post", new HttpFormUrlEncodedContent(dic), new HttpCredentialsHeaderValue("Bearer", WebApi.Token));
                        if (WResult.IsSuccessStatusCode)
                        {
                            await Helper.ShowMessage("پست به صف افزوده شد", "پست به صف افزوده شد");
                        }
                        else
                        {
                            await Helper.ShowMessage("خطا در ارسال پست", "خطا در ارسال پست");
                        }
                    }
                    else if (_Target.Count == 1)
                    {
                        string PCap = "";
                        PostCaption.Document.GetText(Windows.UI.Text.TextGetOptions.None, out PCap);
                        ImagePostViewModel imagePostViewModel = new ImagePostViewModel();
                        imagePostViewModel.caption = PCap;

                        var filestream = await _Target[0].OpenStreamForReadAsync();
                        var filebytes = new byte[(int)filestream.Length];
                        filestream.Read(filebytes, 0, (int)filestream.Length);
                        imagePostViewModel.images.Add(Convert.ToBase64String(filebytes));

                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", InstaMediaType.Image.ToString());
                        dic.Add("accountusername", Api.Username);
                        dic.Add("data", Convert.ToBase64String(ToByteArray(imagePostViewModel)));
                        dic.Add("date", (new DateTime(DPicker.Date.Value.Year, DPicker.Date.Value.Month, DPicker.Date.Value.Day, TPicker.Time.Hours, TPicker.Time.Minutes, TPicker.Time.Seconds)).ToBinary().ToString());

                        var WResult = await WebApi.Post("http://socialcrm.ir/api/uwp/Add_Post", new HttpFormUrlEncodedContent(dic), new HttpCredentialsHeaderValue("Bearer", WebApi.Token));
                        if (WResult.IsSuccessStatusCode)
                        {
                            await Helper.ShowMessage("پست به صف افزوده شد", "پست به صف افزوده شد");
                        }
                        else
                        {
                            await Helper.ShowMessage("خطا در ارسال پست", "خطا در ارسال پست");
                        }
                    }
                }
            }
        }
        //Upload Videos
        async Task UploadVideo()
        {
            if (TPicker.Visibility == Visibility.Collapsed)
            {
                var _Target = (await tempFolder.GetFilesAsync()).Where(x => x.DisplayName.StartsWith("_tmpVid")).ToList();
                if (_Target.Count == 1)
                {
                    string PCap = "";
                    PostCaption.Document.GetText(Windows.UI.Text.TextGetOptions.None, out PCap);
                    VideoProperties videoProperties = await _Target[0].Properties.GetVideoPropertiesAsync();

                    var thumb = await _Target[0].GetThumbnailAsync(ThumbnailMode.SingleItem);
                    var cnow = DateTime.Now.ToFileTime();
                    StorageFile _Target2 = await tempFolder.CreateFileAsync("_tmpVid" + cnow + ".jpg", CreationCollisionOption.ReplaceExisting);
                    IBuffer buf;
                    Windows.Storage.Streams.Buffer inputBuffer = new Windows.Storage.Streams.Buffer(1024);
                    using (var destFileStream = await _Target2.OpenAsync(FileAccessMode.ReadWrite))
                        while ((buf = (await thumb.ReadAsync(inputBuffer, inputBuffer.Capacity, Windows.Storage.Streams.InputStreamOptions.None))).Length > 0)
                            await destFileStream.WriteAsync(buf);
                    ImageProperties imageProperties = await _Target2.Properties.GetImagePropertiesAsync();

                    var UpResult = await Api.InstaApi.UploadVideoAsync(new InstaVideo(_Target[0].Path, int.Parse(videoProperties.Width.ToString()), int.Parse(videoProperties.Height.ToString()), 3), new InstaImage(_Target2.Path, int.Parse(imageProperties.Width.ToString()), int.Parse(imageProperties.Height.ToString())), PCap);
                    if (UpResult.Succeeded)
                    {
                        ContentDialog End = new ContentDialog
                        {
                            FlowDirection = FlowDirection.RightToLeft,
                            FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                            Content = "ویدئو با موفقیت ارسال شد",
                            CloseButtonText = "باشه"
                        };
                        await End.ShowAsync();
                    }
                    else
                    {
                        ContentDialog End = new ContentDialog
                        {
                            FlowDirection = FlowDirection.RightToLeft,
                            FontFamily = new FontFamily("/Assets/Fonts/IRANSansWeb(FaNum).ttf#IRANSansWeb(FaNum)"),
                            Content = "خطا",
                            CloseButtonText = "باشه"
                        };
                        await End.ShowAsync();
                    }
                }
            }
            {
                if (await WebApi.Check())
                {
                    var _Target = (await tempFolder.GetFilesAsync()).Where(x => x.DisplayName.StartsWith("_tmpVid")).ToList();
                    if (_Target.Count == 1)
                    {
                        string PCap = "";
                        PostCaption.Document.GetText(Windows.UI.Text.TextGetOptions.None, out PCap);
                        VideoPostViewModel videoPostViewModel = new VideoPostViewModel();
                        videoPostViewModel.caption = PCap;

                        var filestream = await _Target[0].OpenStreamForReadAsync();
                        var filebytes = new byte[(int)filestream.Length];
                        filestream.Read(filebytes, 0, (int)filestream.Length);
                        videoPostViewModel.video = (Convert.ToBase64String(filebytes));

                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", InstaMediaType.Video.ToString());
                        dic.Add("accountusername", Api.Username);
                        dic.Add("data", Convert.ToBase64String(ToByteArray(videoPostViewModel)));
                        dic.Add("date", (new DateTime(DPicker.Date.Value.Year, DPicker.Date.Value.Month, DPicker.Date.Value.Day, TPicker.Time.Hours, TPicker.Time.Minutes, TPicker.Time.Seconds)).ToBinary().ToString());

                        var WResult = await WebApi.Post("http://socialcrm.ir/api/uwp/Add_Post", new HttpFormUrlEncodedContent(dic), new HttpCredentialsHeaderValue("Bearer", WebApi.Token));
                        if (WResult.IsSuccessStatusCode)
                        {
                            await Helper.ShowMessage("پست به صف افزوده شد", "پست به صف افزوده شد");
                        }
                        else
                        {
                            await Helper.ShowMessage("خطا در ارسال پست", "خطا در ارسال پست");
                        }
                    }
                }
            }
        }
        //Add Video
        private async void AddVideo_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".mp4");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await Helper.DeleteTempFiles(tempFolder, "_tmpVid");
                var cnow = DateTime.Now.ToFileTime();
                StorageFile _Target = await tempFolder.CreateFileAsync("_tmpVid" + cnow + file.FileType, CreationCollisionOption.ReplaceExisting);
                await file.CopyAndReplaceAsync(_Target);
                VideoPreview.Source = MediaSource.CreateFromUri(new Uri(_Target.Path));
                //VideoPreview
                //MediasforUploadList.Items.Add(new Image() { Tag = "_tmpVid" + cnow + file.FileType, Source = new BitmapImage(new Uri(_Target.Path)), Width = 92, Height = 92 });
            }
        }

        private byte[] ToByteArray(object source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }
        private object FromByteArray(byte[] source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(source))
            {
                formatter.Deserialize(stream);
                return stream.ToArray();
            }
        }
    }
}
