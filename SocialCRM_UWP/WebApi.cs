using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Web.Http;

namespace SocialCRM_UWP
{
    public static class WebApi
    {
        private static IPropertySet localProperties = ApplicationData.Current.LocalSettings.Values;

        public static string Token
        {
            get
            {
                try
                {
                    return localProperties["Token"].ToString() ?? "";
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string Username
        {
            get
            {
                try
                {
                    return localProperties["Username"].ToString();
                }
                catch
                {
                    return null;
                }
            }
        }
        public static string Password
        {
            get
            {
                try
                {
                    return localProperties["Password"].ToString() ?? "";
                }
                catch
                {
                    return null;
                }
            }
        }
        public static bool RememberMe
        {
            get
            {
                try
                {
                    return bool.Parse(localProperties["Rememberme"].ToString());
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool IsFirstRun
        {
            get
            {
                try
                {
                    return bool.Parse(localProperties["IsFirstRun"].ToString());
                }
                catch
                {
                    return true;
                }
            }
            set
            {
                localProperties["IsFirstRun"] = value.ToString();
            }
        }
        public static async Task<bool> Check()
        {
            if (Token != null && Username != null && Password != null)
            {
                //Test Token
                var checktoken = await Post("http://socialcrm.ir/api/uwp/Echo", null, new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", Token));
                if (checktoken.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    //Try to refresh token
                    return (await Login(Username, Password, RememberMe));
                }
            }
            else
            {
                return false;
            }
        }
        public static async Task<bool> Login(string uname, string pword, bool rememberme)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", uname },
                { "password", pword }
            };
            var Result = await Post("http://socialcrm.ir/Token", new HttpFormUrlEncodedContent(Dic));
            if (Result.IsSuccessStatusCode)
            {

                dynamic DataResult = JsonConvert.DeserializeObject(Result.Content.ToString());
                localProperties["Token"] = (string)DataResult.access_token;
                localProperties["Username"] = uname;
                localProperties["Password"] = pword;

                if (rememberme)
                {
                    localProperties["Rememberme"] = bool.TrueString;
                }
                else
                {
                    localProperties["Rememberme"] = bool.FalseString;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<HttpResponseMessage> Post(string path, IHttpContent content = null, Windows.Web.Http.Headers.HttpCredentialsHeaderValue httpCredentialsHeaderValue = null)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = httpCredentialsHeaderValue;
            return (await httpClient.PostAsync(new Uri(path), content));
        }
    }
}
