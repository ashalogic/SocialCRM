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
using Windows.UI.Xaml.Navigation;

namespace SocialCRM_UWP.Instagram
{
    public sealed partial class Main : Page
    {
        public Main()
        {
            this.InitializeComponent();

            MediaManagementFrame.Navigate(typeof(Instagram.Pages.MediaManagement));
            FollowerManagementFrame.Navigate(typeof(Instagram.Pages.FollowerManagement));
            ScheduleManagementFrame.Navigate(typeof(Instagram.Pages.ScheduleManagement));
            HashtagManagementFrame.Navigate(typeof(Instagram.Pages.HashtagManagement));
            DirectFrame.Navigate(typeof(Pages.DirectManagement));
            DashboardFrame.Navigate(typeof(Pages.Dashboard));
            CAnalyserFrame.Navigate(typeof(Pages.CAnalyser));
            LotteryFrame.Navigate(typeof(Pages.LotteryFrame));

            //CommentsManagementFrame.Navigate(typeof(Pages.CommentsManagement));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            MediaManagementFrame.Navigate(typeof(Instagram.Pages.MediaManagement));
            FollowerManagementFrame.Navigate(typeof(Instagram.Pages.FollowerManagement));
            ScheduleManagementFrame.Navigate(typeof(Instagram.Pages.ScheduleManagement));
            HashtagManagementFrame.Navigate(typeof(Instagram.Pages.HashtagManagement));
            DirectFrame.Navigate(typeof(Pages.DirectManagement));
            DashboardFrame.Navigate(typeof(Pages.Dashboard));
            CAnalyserFrame.Navigate(typeof(Pages.CAnalyser));
            LotteryFrame.Navigate(typeof(Pages.LotteryFrame));

            //CommentsManagementFrame.Navigate(typeof(Pages.CommentsManagement));
        }
    }
}
