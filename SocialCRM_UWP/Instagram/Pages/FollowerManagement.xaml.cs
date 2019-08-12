using SocialCRM_UWP.Instagram.Models;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Instagram.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FollowerManagement : Page
    {
        public FollowerManagement()
        {
            this.InitializeComponent();
            LoadUserFollowers();
        }

        async Task LoadUserFollowers()
        {
            try
            {
                UserFollowersListPRing.IsActive = true;
                UserFollowersList.Items.Clear();
                var _UserFollowers = (await Api.InstaApi.GetCurrentUserFollowersAsync(InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(1))).Value;

                for (int i = 0; i < _UserFollowers.Count; i++)
                {
                    //var _fInfo = (await Api.Instagram.InstaApi.GetUserInfoByUsernameAsync(_UserFollowers[i].UserName)).Value;
                    //var _FriendshipStatus = await Api.Instagram.InstaApi.GetFriendshipStatusAsync(_UserFollowers[i].Pk);

                    UserFollowersList.Items.Add(new FollowerViewModel()
                    {
                        //IsFollowing = LitybitVisibiltyConverter(_FriendshipStatus.Value.Following),
                        //IsNotFollowing = LitybitVisibiltyConverter(!_FriendshipStatus.Value.Following),
                        Id = _UserFollowers[i].Pk.ToString(),
                        IsVerified = Helper.VisibiltyConverter(_UserFollowers[i].IsVerified),
                        //IsBusiness = LitybitVisibiltyConverter(_fInfo.IsBusiness),
                        IsPrivate = Helper.VisibiltyConverter(_UserFollowers[i].IsPrivate),
                        UserName = _UserFollowers[i].UserName,
                        //FollowersCount = _fInfo.FollowerCount.ToString(),
                        //FollowingsCount = _fInfo.FollowingCount.ToString(),
                        //MediasCount = _fInfo.MediaCount.ToString(),
                        ProfilePic = _UserFollowers[i].ProfilePicture
                    });
                }
                UserFollowersListPRing.IsActive = false;
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }

        private async void FollowersSearchbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                UserFollowersListPRing.IsActive = true;
                UserFollowersList.Items.Clear();
                var _User = await Api.InstaApi.GetCurrentUserAsync();
                var _UserFollowers = (await Api.InstaApi.GetUserFollowersAsync(_User.Value.UserName, InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(1), FollowersSearchbox.Text)).Value;

                for (int i = 0; i < _UserFollowers.Count; i++)
                {
                    //var _fInfo = (await Api.Instagram.InstaApi.GetUserInfoByUsernameAsync(_UserFollowers[i].UserName)).Value;
                    //var _FriendshipStatus = await Api.Instagram.InstaApi.GetFriendshipStatusAsync(_UserFollowers[i].Pk);

                    UserFollowersList.Items.Add(new FollowerViewModel()
                    {
                        //IsFollowing = LitybitVisibiltyConverter(_FriendshipStatus.Value.Following),
                        //IsNotFollowing = LitybitVisibiltyConverter(!_FriendshipStatus.Value.Following),
                        Id = _UserFollowers[i].Pk.ToString(),
                        IsVerified = Helper.VisibiltyConverter(_UserFollowers[i].IsVerified),
                        //IsBusiness = LitybitVisibiltyConverter(_fInfo.IsBusiness),
                        IsPrivate = Helper.VisibiltyConverter(_UserFollowers[i].IsPrivate),
                        UserName = _UserFollowers[i].UserName,
                        //FollowersCount = _fInfo.FollowerCount.ToString(),
                        //FollowingsCount = _fInfo.FollowingCount.ToString(),
                        //MediasCount = _fInfo.MediaCount.ToString(),
                        ProfilePic = _UserFollowers[i].ProfilePicture
                    });
                }
                UserFollowersListPRing.IsActive = false;
            }
        }
    }
}
