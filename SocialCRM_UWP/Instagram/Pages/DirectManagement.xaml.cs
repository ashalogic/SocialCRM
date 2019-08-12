using SocialCRM_UWP.Instagram.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
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
    public sealed partial class DirectManagement : Page
    {
        DispatcherTimer DispatcherTimer = new DispatcherTimer();
        public DirectManagement()
        {
            this.InitializeComponent();
            DispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            DispatcherTimer.Tick += DispatcherTimer_Tick;
            //DispatcherTimer.Start();
            Load();
        }

        private async void DispatcherTimer_Tick(object sender, object e)
        {
            try
            {
                var inbox = await Api.InstaApi.GetDirectInboxAsync();
                foreach (var innerinbox in InboxList.Items)
                {
                    var element = innerinbox as InboxViewModel;
                    var newelement = inbox.Value.Inbox.Threads.Where(x => x.ThreadId == element.TTag).First();
                    try
                    {
                        if (newelement.Items[0].TimeStamp != element.LMDate)
                        {
                            //element.LastMessage = inbox.Value.Inbox.Threads.Where(x => x.ThreadId == element.TTag).First().Items[0].Text ?? "";
                            //element.Notify = Visibility.Visible;
                            (InboxList.Items.Where(x => (x as InboxViewModel).TTag == element.TTag).First() as InboxViewModel).LastMessage = newelement.Items[0].Text;
                            (InboxList.Items.Where(x => (x as InboxViewModel).TTag == element.TTag).First() as InboxViewModel).Notify = Visibility.Visible;
                        }
                    }
                    catch
                    {

                    }
                }

                if (ChatDetails.Items.Count > 0)
                {
                    var ClickedItem = InboxList.SelectedItem as InboxViewModel;
                    var Chats = await Api.InstaApi.GetDirectInboxThreadAsync(ClickedItem.TTag);

                    ChatDetails.Items.Clear();
                    foreach (var chat in Chats.Value.Items.OrderBy(x => x.TimeStamp).ToList())
                    {
                        if (chat.UserId == Api.Userid)
                            ChatDetails.Items.Add(new ChatViewModel() { Fill = "#e3e3e3", StrokeThickness = "0", ItemId = chat.ItemId, Date = chat.TimeStamp.ToShortDateString(), Text = chat.Text, Dir = FlowDirection.RightToLeft });
                        else
                            ChatDetails.Items.Add(new ChatViewModel() { Fill = "#fff", StrokeThickness = "1", ItemId = chat.ItemId, Date = chat.TimeStamp.ToShortDateString(), Text = chat.Text, Dir = FlowDirection.LeftToRight });
                    }
                    ChatDetails.SelectedIndex = ChatDetails.Items.Count - 1;
                    ChatDetails.ScrollIntoView(ChatDetails.SelectedItem);
                }
            }
            catch
            {

            }
        }

        async Task Load()
        {
            try
            {
                InboxList.Items.Clear();
                var inbox = await Api.InstaApi.GetDirectInboxAsync();
                foreach (var t in inbox.Value.Inbox.Threads)
                {
                    if (t.Items.Count > 0 && !string.IsNullOrWhiteSpace(t.Items[0].Text))
                    {
                        string Text = t.Items[0].Text.Replace(Environment.NewLine, " ");
                        if (Text.Length > 20)
                            Text = Text.Substring(0, 20).ToString();
                        InboxList.Items.Add(new InboxViewModel() { Notify = Visibility.Collapsed, LMDate = t.Items[0].TimeStamp, UTag = t.Users[0].Pk.ToString(), TTag = t.ThreadId, Title = t.Title, ProfielPicture = t.Users[0].ProfilePicture, LastMessage = Text, Username = t.Title });
                    }
                    else
                        InboxList.Items.Add(new InboxViewModel() { Notify = Visibility.Collapsed, LMDate = t.Items[0].TimeStamp, UTag = t.Users[0].Pk.ToString(), TTag = t.ThreadId, Title = t.Title, ProfielPicture = t.Users[0].ProfilePicture, LastMessage = t.LastActivity.ToShortDateString(), Username = t.Title });
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextToSend.Text))
            {
                SendBtn.IsEnabled = false;
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                var result = await Api.InstaApi.SendDirectMessage((InboxList.SelectedItem as InboxViewModel).UTag, (InboxList.SelectedItem as InboxViewModel).TTag, TextToSend.Text);
                if (result.Succeeded)
                {
                    var ClickedItem = InboxList.SelectedItem as InboxViewModel;

                    var Chats = await Api.InstaApi.GetDirectInboxThreadAsync(ClickedItem.TTag);

                    ChatDetails.Items.Clear();
                    foreach (var chat in Chats.Value.Items.OrderBy(x => x.TimeStamp).ToList())
                    {
                        if (chat.UserId == Api.Userid)
                            ChatDetails.Items.Add(new ChatViewModel() { Fill = "#e3e3e3", StrokeThickness = "0", ItemId = chat.ItemId, Date = chat.TimeStamp.ToShortDateString() + " " + chat.TimeStamp.ToShortTimeString(), Text = chat.Text, Dir = FlowDirection.RightToLeft });
                        else
                            ChatDetails.Items.Add(new ChatViewModel() { Fill = "#fff", StrokeThickness = "1", ItemId = chat.ItemId, Date = chat.TimeStamp.ToShortDateString() + " " + chat.TimeStamp.ToShortTimeString(), Text = chat.Text, Dir = FlowDirection.LeftToRight });
                    }
                }
                //stopwatch.Stop();
                ChatDetails.SelectedIndex = ChatDetails.Items.Count - 1;
                ChatDetails.ScrollIntoView(ChatDetails.SelectedItem);
                TextToSend.Text = "";
                SendBtn.IsEnabled = true;
            }
        }

        private async void InboxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ClickedItem = InboxList.SelectedItem as InboxViewModel;
            var Chats = await Api.InstaApi.GetDirectInboxThreadAsync(ClickedItem.TTag);

            ChatDetails.Items.Clear();
            foreach (var chat in Chats.Value.Items.OrderBy(x => x.TimeStamp).ToList())
            {
                if (chat.UserId == Api.Userid)
                    ChatDetails.Items.Add(new ChatViewModel() { Fill = "#e3e3e3", StrokeThickness = "0", ItemId = chat.ItemId, Date = chat.TimeStamp.ToShortDateString(), Text = chat.Text, Dir = FlowDirection.RightToLeft });
                else
                    ChatDetails.Items.Add(new ChatViewModel() { Fill = "#fff", StrokeThickness = "1", ItemId = chat.ItemId, Date = chat.TimeStamp.ToShortDateString(), Text = chat.Text, Dir = FlowDirection.LeftToRight });
            }
            ChatDetails.SelectedIndex = ChatDetails.Items.Count - 1;
            ChatDetails.ScrollIntoView(ChatDetails.SelectedItem);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
                ChatDetails.Items.Clear();
        }
    }
}
