using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AccountAddViewModel
    {
        public AccountsEnum accounttype { get; set; }
        public string username { get; set; }
        public string data { get; set; }
    }
    public class AccountViewModel
    {
        public string id { get; set; }
        public AccountsEnum accounttype { get; set; }
        public string username { get; set; }
        public string data { get; set; }
    }

    public class PostAddViewModel
    {
        public string type { get; set; }
        public string accountusername { get; set; }
        public string data { get; set; }
        public string date { get; set; }
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
    public class SPostReqViewModel
    {
        public string accountusername { get; set; }
    }
    public class SPostRReqViewModel
    {
        public string spid { get; set; }
        public string acname { get; set; }
    }
}