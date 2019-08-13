using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstaSharper.API;
using InstaSharper.Classes;
using Windows.Storage;

namespace SocialCRM_UWP.Instagram
{
    public static class Api
    {
        public static IInstaApi InstaApi { get; private set; }

        public static async Task<bool> Initialization(Stream data, string uname = "Queen", string pword = "Saigon")
        {
            Err = "";
            InstaApi = InstaSharper.API.Builder.InstaApiBuilder.CreateBuilder()
                  .SetUser(new UserSessionData() { UserName = uname, Password = pword })
                  .SetRequestDelay(RequestDelay.FromSeconds(1, 2))
                  .Build();
            InstaApi.LoadStateDataFromStream(data);
            if (InstaApi.IsUserAuthenticated)
            {
                var Result = await InstaApi.GetCurrentUserAsync();
                if (Result.Succeeded)
                {
                    Initialized = true;
                    Userid = Result.Value.Pk;
                    Username = Result.Value.UserName;

                    return true;
                }
                else
                {
                    Err = "خطا در تایید هویت حساب لطفا دوباره سعی کنید یا دوباره وارد شوید";
                    return false;
                }
            }
            else
            {
                Err = "خطا در تایید هویت حساب لطفا دوباره سعی کنید یا دوباره وارد شوید";
                return false;
            }
        }

        public static async Task<bool> Login(string uname, string pword)
        {
            Err = "";
            InstaApi = InstaSharper.API.Builder.InstaApiBuilder.CreateBuilder()
                  .SetUser(new UserSessionData() { UserName = uname, Password = pword })
                  .SetRequestDelay(RequestDelay.FromSeconds(1, 2))
                  .Build();
            var loginResult = await InstaApi.LoginAsync();

            //if (loginResult.Info.Message == "Challenge is required")
            //{
            //    var resgv = await InstaApi.GetVerifyStep();

            //    var resvm = await InstaApi.ChooseVerifyMethod(1);

            //    var resvc = await InstaApi.SendVerifyCode("832549");

            //    if (resvc.Succeeded)
            //    {
            //        return true;
            //    }
            //}

            if (loginResult.Succeeded)
            {
                if (InstaApi.IsUserAuthenticated)
                {
                    //var StateData = InstaApi.GetStateDataAsStream();

                    //var accountsFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Accounts");
                    //var instagramFolder = await accountsFolder.GetFolderAsync("Instagram");
                    //var loginsFolder = await instagramFolder.GetFolderAsync("Logins");
                    //var loginFile = await loginsFolder.CreateFileAsync(uname + ".logininfo", CreationCollisionOption.ReplaceExisting);
                    //await FileIO.WriteBytesAsync(loginFile, (StateData as MemoryStream).ToArray());
                    return true;
                }
                else
                {
                    Err = "خطا در تایید هویت حساب شما لطفا از طریق نرم افزار موبایل یا وبسایت وارد شوید سپس دوباره امتحان کنید";
                    return false;
                }
            }
            else
            {
                Err = "نام کاربری یا کلمه عبور اشتباه است";
                return false;
            }
        }

        public static long Userid { get; private set; }
        public static string Username { get; private set; }
        public static string Err { get; private set; }
        public static bool Initialized { get; private set; }
    }
}
