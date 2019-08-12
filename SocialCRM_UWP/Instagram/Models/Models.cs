using InstaSharper.Classes.Models;
using SocialCRM_UWP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace SocialCRM_UWP.Instagram.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string data { get; set; }
        public string Profilepicture { get; set; }
        public string id { get; set; }
    }

    public class MediaViewModel
    {
        public bool IsOwner { get; set; }
        public string Id { get; set; }
        public string Caption { get; set; }
        public string LikesCount { get; set; }
        public string CommentsCount { get; set; }
        public string ViewsCount { get; set; }
        public string UserName { get; set; }
        public InstaMediaType InstaMediaType { get; set; }
        public List<InstaVideo> Videos { get; set; }
        public List<InstaImage> Images { get; set; }
        public InstaCarousel Carousel { get; set; }
        public string Thumbnail { get; set; }
        public bool HasLiked { get; set; }
        public Windows.UI.Xaml.Visibility IsCarousel { get; set; }
        public Windows.UI.Xaml.Visibility IsVideo { get; set; }

    }
    public class CommentViewModel
    {
        public string UserId { get; set; }
        public string MediaId { get; set; }
        public string CommentId { get; set; }
        public string Text { get; set; }
        public string ProfilePic { get; set; }
        public string UserName { get; set; }
        public string LikesCount { get; set; }
        public string Date { get; set; }

        public string ReplyText { get; set; }


        public ICommand Reply { get; set; }
        public ICommand Delete { get; set; }

        public CommentViewModel()
        {
            Reply = new Utility.DelegateCommand(ExecuteReplyCommand);
            Delete = new Utility.DelegateCommand(ExecuteDeleteCommand);
        }

        async void ExecuteReplyCommand(object param)
        {
            if (param.GetType().Equals(typeof(CommentViewModel)))
            {
                CommentViewModel _InstagramCommentModel = param as CommentViewModel;
                await Api.InstaApi.CommentMediaAsync(_InstagramCommentModel.MediaId, "@" + _InstagramCommentModel.UserName + " " + _InstagramCommentModel.ReplyText);
            }
        }
        async void ExecuteDeleteCommand(object param)
        {
            if (param.GetType().Equals(typeof(CommentViewModel)))
            {
                CommentViewModel _InstagramCommentModel = param as CommentViewModel;
                await Api.InstaApi.DeleteCommentAsync(_InstagramCommentModel.MediaId, _InstagramCommentModel.CommentId);
            }
        }
    }
    public class FollowerViewModel
    {
        public string Id { get; set; }
        public string ProfilePic { get; set; }
        public string FollowersCount { get; set; }
        public string FollowingsCount { get; set; }
        public string MediasCount { get; set; }
        public string UserName { get; set; }
        public Windows.UI.Xaml.Visibility IsPrivate { get; set; }
        public Windows.UI.Xaml.Visibility IsBusiness { get; set; }
        public Windows.UI.Xaml.Visibility IsVerified { get; set; }
        public Windows.UI.Xaml.Visibility IsFollowing { get; set; }
        public Windows.UI.Xaml.Visibility IsNotFollowing { get; set; }

        public ICommand UnFollow { set; get; }
        public ICommand Block { set; get; }

        public FollowerViewModel()
        {
            UnFollow = new DelegateCommand(ExecuteUnFollowCommand);
            Block = new DelegateCommand(ExecuteBlockCommand);
        }

        async void ExecuteUnFollowCommand(object param)
        {
            if (param.GetType().Equals(typeof(FollowerViewModel)))
            {
                FollowerViewModel _InstagramFollowerModel = param as FollowerViewModel;
                await Api.InstaApi.BlockUserAsync(long.Parse(_InstagramFollowerModel.Id));
                await Api.InstaApi.UnBlockUserAsync(long.Parse(_InstagramFollowerModel.Id));
            }
        }
        async void ExecuteBlockCommand(object param)
        {
            if (param.GetType().Equals(typeof(FollowerViewModel)))
            {
                FollowerViewModel _InstagramFollowerModel = param as FollowerViewModel;
                await Api.InstaApi.BlockUserAsync(long.Parse(_InstagramFollowerModel.Id));
            }
        }
    }
    public class InboxViewModel
    {
        public string Title { get; set; }
        public string ProfielPicture { get; set; }
        public string Username { get; set; }
        public string TTag { get; set; }
        public string UTag { get; set; }
        public string LastMessage { get; set; }
        public DateTime LMDate { get; set; }
        public Visibility Notify { get; set; }
    }
    public class ChatViewModel
    {
        public string Text { get; set; }
        public FlowDirection Dir { get; set; }
        public string Date { get; set; }
        public string ItemId { get; set; }
        public string Seen { get; set; }
        public string Liked { get; set; }
        public string Fill { get; set; }
        public string StrokeThickness { get; set; }
    }

    public class SPostViewModel
    {
        public string id { get; set; }
        public string type { get; set; }
        public string post { get; set; }
        public DateTime datetime { get; set; }
        public bool status { get; set; }
        public string statusdetails { get; set; }
    }
    public class SPListViewModel
    {
        public string id { get; set; }
        public string time { get; set; }
        public string caption { get; set; }
        public string date { get; set; }
        public BitmapImage image { get; set; }
        public byte[] video { get; set; }
        public string type { get; set; }
        public List<string> carousel { get; set; }
    }

    [Serializable]
    public class ImagePostViewModel
    {
        public ImagePostViewModel()
        {
            images = new List<string>();
        }
        public string caption { get; set; }
        public List<string> images { get; set; }
    }

    [Serializable]
    public class VideoPostViewModel
    {
        public string caption { get; set; }
        public string video { get; set; }
    }
}
