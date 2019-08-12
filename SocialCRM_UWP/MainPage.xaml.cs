using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SocialCRM_UWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //set the initial SelectedItem
                foreach (NavigationViewItemBase item in NavView.MenuItems)
                {
                    if (item is NavigationViewItem && item.Tag.ToString() == "Instagram")
                    {
                        NavView.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            switch (args.InvokedItem)
            {
                //case "Dashboard":
                //    ContentFrame.Navigate(typeof(MainPages.Dashboard));
                //    break;
                case "Instagram":
                    if (Instagram.Api.Initialized)
                        ContentFrame.Navigate(typeof(Instagram.Main));
                    else
                        ContentFrame.Navigate(typeof(Instagram.Accounts));
                    break;
                case "Twitter":
                    ContentFrame.Navigate(typeof(Twitter.Accounts));
                    break;
                case "Telegram":
                    ContentFrame.Navigate(typeof(Telegram.Accounts));
                    break;
                case "Linkedin":
                    ContentFrame.Navigate(typeof(Linkedin.Accounts));
                    break;
                case "Rss":
                    ContentFrame.Navigate(typeof(Rss.Main));
                    break;
                case "Analysis":
                    ContentFrame.Navigate(typeof(Analysis.Main));
                    break;
            }

            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(Settings));
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;

            switch (item.Tag)
            {
                case "Instagram":
                    if (Instagram.Api.Initialized)
                        ContentFrame.Navigate(typeof(Instagram.Main));
                    else
                        ContentFrame.Navigate(typeof(Instagram.Accounts));
                    break;
                case "Twitter":
                    ContentFrame.Navigate(typeof(Twitter.Accounts));
                    break;
                case "Telegram":
                    ContentFrame.Navigate(typeof(Telegram.Accounts));
                    break;
                case "Linkedin":
                    ContentFrame.Navigate(typeof(Linkedin.Accounts));
                    break;
                case "Rss":
                    ContentFrame.Navigate(typeof(Rss.Main));
                    break;
                case "Analysis":
                    ContentFrame.Navigate(typeof(Analysis.Main));
                    break;
            }

            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(Settings));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Foundation.Collections.IPropertySet localProperties = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            localProperties["Rememberme"] = bool.FalseString;
            Frame.Navigate(typeof(Introduction));
        }
    }
}
