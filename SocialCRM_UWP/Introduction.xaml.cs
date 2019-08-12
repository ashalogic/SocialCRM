using Newtonsoft.Json;
using SocialCRM_UWP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace SocialCRM_UWP
{
    public sealed partial class Introduction : Page
    {

        public Introduction()
        {
            this.InitializeComponent();
        }

        private async void startAppBtn_Click(object sender, RoutedEventArgs e)
        {
            //MainContent.Visibility = Visibility.Collapsed;
            //MainContentPRing.IsActive = true;
            ////ApplicationData/Accounts
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //await localFolder.CreateFolderAsync("Accounts");
            ////ApplicationData/Accounts/...
            //StorageFolder accountsFolder = await localFolder.GetFolderAsync("Accounts");
            //var instagramFolder = await accountsFolder.CreateFolderAsync("Instagram");
            //await instagramFolder.CreateFolderAsync("Logins");
            //await instagramFolder.CreateFolderAsync("Scheduled");
            //await instagramFolder.CreateFolderAsync("Temp");
            //var twitterFolder = await accountsFolder.CreateFolderAsync("Twitter");
            //await twitterFolder.CreateFolderAsync("Logins");
            //var aparatFolder = await accountsFolder.CreateFolderAsync("Aparat");
            //await aparatFolder.CreateFolderAsync("Logins");
            //var linkedinFolder = await accountsFolder.CreateFolderAsync("Linkedin");
            //await linkedinFolder.CreateFolderAsync("Logins");
            //var telegramFolder = await accountsFolder.CreateFolderAsync("Telegram");
            //await telegramFolder.CreateFolderAsync("Logins");

            //IPropertySet localProperties = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            //localProperties["HasBeenHereBefore"] = bool.TrueString; // Doesn't really matter what
            //MainContentPRing.IsActive = false;
            ////await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync("");
            //this.Frame.Navigate(typeof(MainPage));
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ProgressPanel.Visibility = Visibility.Visible;
                var Result = await WebApi.Login(UsernameBox.Text, PasswordBox.Password, RemembermeBox.IsChecked.GetValueOrDefault());
                switch (Result)
                {
                    case true:
                        Frame.Navigate(typeof(MainPage));
                        break;
                    case false:
                        {
                            await Utility.Helper.ShowMessage("خطا در ورود به سیستم", "نام کاربری یا رمز عبور خود را بررسی کنید یا با پشتیبانی تماس بگیرید");
                            LoginPanel.Visibility = Visibility.Visible;
                            ProgressPanel.Visibility = Visibility.Collapsed;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
                LoginPanel.Visibility = Visibility.Visible;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WebApi.IsFirstRun)
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    await localFolder.CreateFolderAsync("Accounts");
                    StorageFolder accountsFolder = await localFolder.GetFolderAsync("Accounts");
                    var instagramFolder = await accountsFolder.CreateFolderAsync("Instagram");
                    await instagramFolder.CreateFolderAsync("Logins");
                    await instagramFolder.CreateFolderAsync("Scheduled");
                    await instagramFolder.CreateFolderAsync("Temp");
                    WebApi.IsFirstRun = false;
                }

                foreach (var item in Frame.BackStack)
                    Frame.BackStack.Remove(item);

                if (!WebApi.RememberMe)
                    return;
                else
                {
                    LoginPanel.Visibility = Visibility.Collapsed;
                    ProgressPanel.Visibility = Visibility.Visible;
                    switch (await WebApi.Check())
                    {
                        case true:
                            Frame.Navigate(typeof(MainPage));
                            break;
                        case false:
                            {
                                await Utility.Helper.ShowMessage("خطا در ورود به سیستم", "نام کاربری یا رمز عبور خود را بررسی کنید یا با پشتیبانی تماس بگیرید");
                                LoginPanel.Visibility = Visibility.Visible;
                                ProgressPanel.Visibility = Visibility.Collapsed;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
                LoginPanel.Visibility = Visibility.Visible;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    LoginPanel.Visibility = Visibility.Collapsed;
                    ProgressPanel.Visibility = Visibility.Visible;
                    var Result = await WebApi.Login(UsernameBox.Text, PasswordBox.Password, RemembermeBox.IsChecked.GetValueOrDefault());
                    switch (Result)
                    {
                        case true:
                            Frame.Navigate(typeof(MainPage));
                            break;
                        case false:
                            {
                                await Utility.Helper.ShowMessage("خطا در ورود به سیستم", "نام کاربری یا رمز عبور خود را بررسی کنید یا با پشتیبانی تماس بگیرید");
                                LoginPanel.Visibility = Visibility.Visible;
                                ProgressPanel.Visibility = Visibility.Collapsed;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
                    LoginPanel.Visibility = Visibility.Visible;
                    ProgressPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
