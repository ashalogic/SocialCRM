using Newtonsoft.Json;
using SocialCRM_UWP.Instagram.Models;
using SocialCRM_UWP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SocialCRM_UWP.Instagram
{
    public sealed partial class Accounts : Page
    {
        public Accounts()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ProgressPanel.Visibility = Visibility.Visible;
                await LoadLogins();
                foreach (var item in Frame.BackStack)
                    Frame.BackStack.Remove(item);
                LoginPanel.Visibility = Visibility.Visible;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }

        private async void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ProgressPanel.Visibility = Visibility.Visible;
                if (await WebApi.Check())
                {
                    var IResult = await Api.Login(usernameBox.Text, passwordBox.Password); /*true;*/
                    if (IResult)
                    {
                        var StateData = Api.InstaApi.GetStateDataAsStream();

                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("accounttype", AccountsEnum.Instagram.ToString());
                        dic.Add("username", usernameBox.Text);
                        dic.Add("data", Convert.ToBase64String((StateData as MemoryStream).ToArray()));

                        var WResult = await WebApi.Post("http://socialcrm.ir/api/uwp/Add_Account", new HttpFormUrlEncodedContent(dic), new HttpCredentialsHeaderValue("Bearer", WebApi.Token));
                        if (WResult.IsSuccessStatusCode)
                        {
                            await Helper.ShowMessage("حساب با موفقیت افزوده شد", "حساب موردنظر با موفقیت افزوده شد برای ورود روی آن کلیک کنید");
                            await LoadLogins();
                            LoginPanel.Visibility = Visibility.Visible;
                            ProgressPanel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            await Helper.ShowMessage("خطا در افزودن حساب", "خطا در افزودن حساب لطفا دوباره سعی کنید یا با پشتیبانی تماس بگیرید");
                            LoginPanel.Visibility = Visibility.Visible;
                            ProgressPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        await Helper.ShowMessage("خطا در رابطه به حساب", Api.Err);
                        LoginPanel.Visibility = Visibility.Visible;
                        ProgressPanel.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    //Exit App
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }

        private async void loginsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ProgressPanel.Visibility = Visibility.Visible;
                if (await WebApi.Check())
                {
                    var data = Convert.FromBase64String((e.ClickedItem as LoginViewModel).data);
                    var Result = await Api.Initialization(new MemoryStream(data));
                    if (Result)
                    {
                        LoginPanel.Visibility = Visibility.Visible;
                        ProgressPanel.Visibility = Visibility.Collapsed;
                        this.Frame.Navigate(typeof(Instagram.Main));
                    }
                    else
                    {
                        await Helper.ShowMessage("خطا در رابطه به حساب", Api.Err);
                        LoginPanel.Visibility = Visibility.Visible;
                        ProgressPanel.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    //App Exit
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }

        async Task LoadLogins()
        {
            try
            {
                loginsList.Items.Clear();
                if (await WebApi.Check())
                {
                    var Result = await WebApi.Post("http://socialcrm.ir/api/uwp/Accounts_List", null, new HttpCredentialsHeaderValue("Bearer", WebApi.Token));
                    if (Result.IsSuccessStatusCode)
                    {
                        var DataResult = JsonConvert.DeserializeObject<IEnumerable<AccountViewModel>>(Result.Content.ToString());

                        foreach (AccountViewModel Account in DataResult)
                        {
                            if (Account.accounttype == AccountsEnum.Instagram)
                            {
                                //dynamic stuff;
                                //using (System.Net.WebClient wc = new System.Net.WebClient())
                                //{
                                //    stuff = JsonConvert.DeserializeObject(await wc.DownloadStringTaskAsync("https://apinsta.herokuapp.com/u/" + Account.username));
                                //}
                                loginsList.Items.Add(new LoginViewModel() { id = Account.id, Username = Account.username, data = Account.data, Profilepicture = /*(string)stuff.graphql.user.profile_pic_url*/"https://static.licdn.com/scds/common/u/images/themes/katy/ghosts/person/ghost_person_200x200_v1.png" });
                            }
                        }
                    }
                }
                else
                {
                    //Exit App
                }
            }
            catch (Exception ex)
            {
                await Utility.Helper.ShowMessage("خطا پیش بینی نشده", ex.Message);
            }
        }
    }
}
