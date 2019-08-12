using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM_UWP
{
    public class AccountViewModel
    {
        public string id { get; set; }
        public AccountsEnum accounttype { get; set; }
        public string username { get; set; }
        public string data { get; set; }
    }

    public enum AccountsEnum
    {
        Instagram, Telegram, Twitter, Linkedin, Aparat
    }
}
