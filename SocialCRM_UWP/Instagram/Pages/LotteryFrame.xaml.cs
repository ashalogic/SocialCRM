using SocialCRM_UWP.Instagram.Models;
using SocialCRM_UWP.Utility;
using System;
using System.Collections;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Instagram.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LotteryFrame : Page
    {
        List<InstaSharper.Classes.Models.InstaComment> AllComments = new List<InstaSharper.Classes.Models.InstaComment>();
        List<InstaSharper.Classes.Models.InstaComment> ValidComments = new List<InstaSharper.Classes.Models.InstaComment>();
        List<InstaSharper.Classes.Models.InstaComment> ValidCommentsDp = new List<InstaSharper.Classes.Models.InstaComment>();
        InstaSharper.Classes.Models.InstaLikersList AllLikers = new InstaSharper.Classes.Models.InstaLikersList();
        MediaViewModel SelectedMedia;

        public LotteryFrame()
        {
            this.InitializeComponent();

            Load();
        }

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

        private List<InstaSharper.Classes.Models.InstaComment> RemoveSearchDuplicates(List<InstaSharper.Classes.Models.InstaComment> SearchResults)
        {
            List<InstaSharper.Classes.Models.InstaComment> TempList = new List<InstaSharper.Classes.Models.InstaComment>();

            foreach (InstaSharper.Classes.Models.InstaComment u1 in SearchResults)
            {
                bool duplicatefound = false;
                foreach (InstaSharper.Classes.Models.InstaComment u2 in TempList)
                    if (u1.UserId == u2.UserId)
                        duplicatefound = true;

                if (!duplicatefound)
                    TempList.Add(u1);
            }
            return TempList;
        }

        void Disable_Btns()
        {
            Do_Btn.IsEnabled = false;
            Get_Btn.IsEnabled = false;
            Filter_Btn.IsEnabled = false;
        }

        void Enable_Btns()
        {
            Do_Btn.IsEnabled = true;
            Get_Btn.IsEnabled = true;
            Filter_Btn.IsEnabled = true;
        }

        //A
        private async void Get_Btn_Click(object sender, RoutedEventArgs e)
        {
            Disable_Btns();
            if (UserMediasList.SelectedIndex != -1)
            {
                SelectedMedia = (MediaViewModel)UserMediasList.SelectedItem;
                if (int.Parse(SelectedMedia.CommentsCount) > 0)
                {
                    Get_Btn.Content = "درحال دریافت کامنت ها ...";
                    var GetMediaComments_Result = await Instagram.Api.InstaApi.GetMediaCommentsAsync(SelectedMedia.Id, InstaSharper.Classes.PaginationParameters.Empty);
                    if (GetMediaComments_Result.Succeeded)
                    {
                        AllComments = GetMediaComments_Result.Value.Comments;

                        Get_Btn.Content = "دریافت کامنت ها";
                        await Utility.Helper.ShowMessage("موفق", string.Format("تعداد {0} کامنت با موفقیت دریافت شدند", AllComments.Count));
                    }
                    else
                        await Utility.Helper.ShowMessage("خطا", "خطا در دریافت کامنت ها");
                }
                else
                {
                    await Utility.Helper.ShowMessage("خطا", "پست شامل حداقل یک کامنت نیست");
                }
            }
            else
            {
                await Utility.Helper.ShowMessage("خطا", "لطفا یک پست انتخاب کنید");
            }
            Enable_Btns();
        }

        //B
        private async void Filter_Btn_Click(object sender, RoutedEventArgs e)
        {
            Disable_Btns();
            Filter_Btn.Content = "درحال اعمال فیلتر ...";

            //Init Params
            bool Follow_Needed = FollowStatusElement.IsChecked.GetValueOrDefault();
            bool Like_Needed = FollowStatusElement.IsChecked.GetValueOrDefault();
            bool Remove_Duplicate = DuplicateStatusElement.IsChecked.GetValueOrDefault();
            bool Only_Private = PrivateStatusElement.IsChecked.GetValueOrDefault();
            int.TryParse(TagCountElement.Text, out int Tag_Count);
            string[] Answers = AnswersElement.Text.Split(';');

            //Statistics
            int Stat_Right_Answers_Count = 0;
            int Stat_Comments_Count = 0;
            int Stat_Unique_Comments_Count = 0;
            int Stat_Tag_Count = 0;
            int Stat_Err_Count = 0;
            int Stat_Private_Count = 0;
            int Stat_Acceptable_Count = 0;
            int Stat_Followed_Count = 0;
            int Stat_Liked_Count = 0;

            var Org = AllComments;

            // حذف کامنت های کاربر
            AllComments.RemoveAll(x => x.User.UserName == Api.Username);
            Stat_Comments_Count = AllComments.Count;

            //حذف تکراری
            AllComments = RemoveSearchDuplicates(AllComments);
            Stat_Unique_Comments_Count = AllComments.Count;

            if (Like_Needed && Follow_Needed)
            {
                var GetMediaLikers_Result = await Instagram.Api.InstaApi.GetMediaLikersAsync(SelectedMedia.Id);
                if (GetMediaLikers_Result.Succeeded)
                {
                    AllLikers = GetMediaLikers_Result.Value;

                    if (Only_Private)
                    {
                        int reportcount = 0;
                        foreach (var comment in AllComments)
                        {
                            try
                            {
                                var RelStatus = await Instagram.Api.InstaApi.GetFriendshipStatusAsync(comment.UserId);
                                bool LikeStatus = AllLikers.Exists(x => x.UserName == comment.User.UserName);
                                bool PrivateStatus = comment.User.IsPrivate;

                                //بررسی وضعیت فالو و لایک و پرایوت بودن
                                if (RelStatus.Value.FollowedBy == Follow_Needed && LikeStatus == Like_Needed && PrivateStatus == Only_Private)
                                {
                                    Stat_Followed_Count++;
                                    Stat_Private_Count++;
                                    Stat_Liked_Count++;

                                    //بررسی جواب صحیح
                                    if (Answers.Any(comment.Text.Trim().Contains))
                                    {
                                        Stat_Right_Answers_Count++;

                                        //بررسی تگ کردن
                                        var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                        var matches = regex.Matches(comment.Text.Trim());
                                        if (matches.Count() >= Tag_Count)
                                        {
                                            Stat_Tag_Count += 3;
                                            Stat_Acceptable_Count++;

                                            //افزودن
                                            ValidComments.Add(comment);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Stat_Err_Count++;
                            }

                            reportcount++;
                            Filter_Btn.Content = AllComments.Count() + " / " + reportcount;
                        }
                    }
                    else
                    {
                        int reportcount = 0;
                        foreach (var comment in AllComments)
                        {
                            try
                            {
                                var RelStatus = await Instagram.Api.InstaApi.GetFriendshipStatusAsync(comment.UserId);
                                bool LikeStatus = AllLikers.Exists(x => x.UserName == comment.User.UserName);
                                bool PrivateStatus = comment.User.IsPrivate;

                                //بررسی وضعیت فالو و لایک
                                if (RelStatus.Value.FollowedBy == Follow_Needed && LikeStatus == Like_Needed)
                                {
                                    Stat_Followed_Count++;
                                    Stat_Liked_Count++;

                                    //بررسی جواب صحیح
                                    if (Answers.Any(comment.Text.Trim().Contains))
                                    {
                                        Stat_Right_Answers_Count++;

                                        //بررسی تگ کردن
                                        var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                        var matches = regex.Matches(comment.Text.Trim());
                                        if (matches.Count() >= Tag_Count)
                                        {
                                            Stat_Tag_Count += Tag_Count;
                                            Stat_Acceptable_Count++;

                                            //افزودن
                                            ValidComments.Add(comment);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Stat_Err_Count++;
                            }

                            reportcount++;
                            Filter_Btn.Content = AllComments.Count() + " / " + reportcount;
                        }
                    }
                }
                else
                    await Utility.Helper.ShowMessage("خطا", "خطا در دریافت لایکر ها");
            }
            else if (Like_Needed)
            {
                var GetMediaLikers_Result = await Instagram.Api.InstaApi.GetMediaLikersAsync(SelectedMedia.Id);
                if (GetMediaLikers_Result.Succeeded)
                {
                    AllLikers = GetMediaLikers_Result.Value;

                    if (Only_Private)
                    {
                        int reportcount = 0;
                        foreach (var comment in AllComments)
                        {
                            try
                            {
                                bool LikeStatus = AllLikers.Exists(x => x.UserName == comment.User.UserName);
                                bool PrivateStatus = comment.User.IsPrivate;

                                //بررسی وضعیت فالو و لایک و پرایوت بودن
                                if (LikeStatus == Like_Needed && PrivateStatus == Only_Private)
                                {
                                    Stat_Private_Count++;
                                    Stat_Liked_Count++;

                                    //بررسی جواب صحیح
                                    if (Answers.Any(comment.Text.Trim().Contains))
                                    {
                                        Stat_Right_Answers_Count++;

                                        //بررسی تگ کردن
                                        var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                        var matches = regex.Matches(comment.Text.Trim());
                                        if (matches.Count() >= Tag_Count)
                                        {
                                            Stat_Tag_Count += Tag_Count;
                                            Stat_Acceptable_Count++;

                                            //افزودن
                                            ValidComments.Add(comment);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Stat_Err_Count++;
                            }
                            reportcount++;
                            Filter_Btn.Content = AllComments.Count() + " / " + reportcount;

                        }
                    }
                    else
                    {
                        int reportcount = 0;
                        foreach (var comment in AllComments)
                        {
                            try
                            {
                                var RelStatus = await Instagram.Api.InstaApi.GetFriendshipStatusAsync(comment.UserId);
                                bool LikeStatus = AllLikers.Exists(x => x.UserName == comment.User.UserName);
                                bool PrivateStatus = comment.User.IsPrivate;

                                //بررسی وضعیت فالو و لایک
                                if (LikeStatus == Like_Needed)
                                {
                                    Stat_Liked_Count++;

                                    //بررسی جواب صحیح
                                    if (Answers.Any(comment.Text.Trim().Contains))
                                    {
                                        Stat_Right_Answers_Count++;

                                        //بررسی تگ کردن
                                        var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                        var matches = regex.Matches(comment.Text.Trim());
                                        if (matches.Count() >= Tag_Count)
                                        {
                                            Stat_Tag_Count += Tag_Count;
                                            Stat_Acceptable_Count++;

                                            //افزودن
                                            ValidComments.Add(comment);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Stat_Err_Count++;
                            }
                            reportcount++;
                            Filter_Btn.Content = AllComments.Count() + " / " + reportcount;

                        }
                    }
                }
                else
                    await Utility.Helper.ShowMessage("خطا", "خطا در دریافت لایکر ها");
            }
            else if (Follow_Needed)
            {
                if (Only_Private)
                {
                    int reportcount = 0;
                    foreach (var comment in AllComments)
                    {
                        try
                        {
                            var RelStatus = await Instagram.Api.InstaApi.GetFriendshipStatusAsync(comment.UserId);
                            bool PrivateStatus = comment.User.IsPrivate;

                            //بررسی وضعیت فالو و پرایوت بودن
                            if (RelStatus.Value.FollowedBy == Follow_Needed && PrivateStatus == Only_Private)
                            {
                                Stat_Followed_Count++;
                                Stat_Private_Count++;

                                //بررسی جواب صحیح
                                if (Answers.Any(comment.Text.Trim().Contains))
                                {
                                    Stat_Right_Answers_Count++;

                                    //بررسی تگ کردن
                                    var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                    var matches = regex.Matches(comment.Text.Trim());
                                    if (matches.Count() >= Tag_Count)
                                    {
                                        Stat_Tag_Count += Tag_Count;
                                        Stat_Acceptable_Count++;

                                        //افزودن
                                        ValidComments.Add(comment);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Stat_Err_Count++;
                        }
                        reportcount++;
                        Filter_Btn.Content = AllComments.Count() + " / " + reportcount;

                    }
                }
                else
                {
                    int reportcount = 0;
                    foreach (var comment in AllComments)
                    {
                        try
                        {
                            var RelStatus = await Instagram.Api.InstaApi.GetFriendshipStatusAsync(comment.UserId);
                            bool PrivateStatus = comment.User.IsPrivate;

                            //بررسی وضعیت فالو و لایک
                            if (RelStatus.Value.FollowedBy == Follow_Needed)
                            {
                                Stat_Followed_Count++;

                                //بررسی جواب صحیح
                                if (Answers.Any(comment.Text.Trim().Contains))
                                {
                                    Stat_Right_Answers_Count++;

                                    //بررسی تگ کردن
                                    var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                    var matches = regex.Matches(comment.Text.Trim());
                                    if (matches.Count() >= Tag_Count)
                                    {
                                        Stat_Tag_Count += Tag_Count;
                                        Stat_Acceptable_Count++;

                                        //افزودن
                                        ValidComments.Add(comment);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Stat_Err_Count++;
                        }
                        reportcount++;
                        Filter_Btn.Content = AllComments.Count() + " / " + reportcount;

                    }
                }
            }
            else
            {
                if (Only_Private)
                {
                    int reportcount = 0;
                    foreach (var comment in AllComments)
                    {
                        try
                        {
                            bool PrivateStatus = comment.User.IsPrivate;

                            //بررسی وضعیت پرایوت بودن
                            if (PrivateStatus == Only_Private)
                            {
                                Stat_Private_Count++;

                                //بررسی جواب صحیح
                                if (Answers.Any(comment.Text.Trim().Contains))
                                {
                                    Stat_Right_Answers_Count++;

                                    //بررسی تگ کردن
                                    var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                    var matches = regex.Matches(comment.Text.Trim());
                                    if (matches.Count() >= Tag_Count)
                                    {
                                        Stat_Tag_Count += Tag_Count;
                                        Stat_Acceptable_Count++;

                                        //افزودن
                                        ValidComments.Add(comment);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Stat_Err_Count++;
                        }
                        reportcount++;
                        Filter_Btn.Content = AllComments.Count() + " / " + reportcount;
                    }
                }
                else
                {
                    int reportcount = 0;
                    foreach (var comment in AllComments)
                    {
                        try
                        {
                            bool PrivateStatus = comment.User.IsPrivate;

                            //بررسی جواب صحیح
                            if (Answers.Any(comment.Text.Trim().Contains))
                            {
                                Stat_Right_Answers_Count++;

                                //بررسی تگ کردن
                                var regex = new System.Text.RegularExpressions.Regex(@"(?<=@)\w+");
                                var matches = regex.Matches(comment.Text.Trim());
                                if (matches.Count() >= Tag_Count)
                                {
                                    Stat_Tag_Count += Tag_Count;
                                    Stat_Acceptable_Count++;

                                    //افزودن
                                    ValidComments.Add(comment);
                                }
                            }
                        }
                        catch
                        {
                            Stat_Err_Count++;
                        }
                        reportcount++;
                        Filter_Btn.Content = AllComments.Count() + " / " + reportcount;
                    }
                }
            }

            AllComments = Org;

            foreach (var c in AllComments)
            {
                if (ValidComments.Exists(x => x.UserId == c.UserId))
                {
                    ValidCommentsDp.Add(c);
                }
            }

            await Utility.Helper.ShowMessage("موفق", string.Format("اعمال فیلتر کامنت ها با موفقیت انجام شد تعداد خطا {0} می باشد", Stat_Err_Count.ToString()));

            CommentsCountTblock.Text = Stat_Comments_Count.ToString();
            UniqueCountTblock.Text = Stat_Unique_Comments_Count.ToString();
            RightAnswersCountTblock.Text = Stat_Right_Answers_Count.ToString();
            RightAnswersCountTblock.Text = Stat_Right_Answers_Count.ToString();
            ValidCountTblock.Text = Stat_Acceptable_Count.ToString();
            LikerCountTblock.Text = Stat_Liked_Count.ToString();
            PrivateCountTblock.Text = Stat_Private_Count.ToString();
            TagCountTblock.Text = Stat_Tag_Count.ToString();
            FollowerCountTblock.Text = Stat_Followed_Count.ToString();

            Filter_Btn.Content = "اعمال فیلتر";
            Enable_Btns();
        }

        //C
        private async void Do_Btn_Click(object sender, RoutedEventArgs e)
        {
            Disable_Btns();

            winners.Items.Clear();

            int.TryParse(WinnersCountElement.Text, out int winnercount);

            if (ValidComments.Count > winnercount)
                if (DuplicateStatusElement.IsChecked.GetValueOrDefault())
                {
                    for (int i = 0; i < winnercount;)
                    {
                        var winner = ValidComments.OrderBy(x => Guid.NewGuid()).Take(1).First();
                        if (winners.Items.IndexOf(winner.User) == -1)
                        {
                            winners.Items.Add(winner.User);
                            i++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < winnercount;)
                    {
                        var winner = ValidCommentsDp.OrderBy(x => Guid.NewGuid()).Take(1).First();
                        if (winners.Items.IndexOf(winner.User) == -1)
                        {
                            winners.Items.Add(winner.User);
                            i++;
                        }
                    }
                }
            else
                await Utility.Helper.ShowMessage("خطا", "مخزن کامنت خالی است");

            Enable_Btns();
        }
    }
}
