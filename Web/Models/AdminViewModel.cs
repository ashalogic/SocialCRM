using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class RegisterUserViewModel
    {
        [Required]
        public string phonenumber { get; set; }
        [Required]
        public string companyname { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public int accountlimit { get; set; }
    }

    public class BlockUnBlockReqViewModel
    {
        public string username { get; set; }
    }

    public class RemoveReqViewModel
    {
        public string username { get; set; }
    }

    public class UsersViewModel
    {
        public UsersViewModel()
        {
            users = new List<UserViewModel>();
        }
        public List<UserViewModel> users { get; set; } = new List<UserViewModel>();
    }

    public class UserViewModel
    {
        public string phonenumber { get; set; }
        public string companyname { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool blocked { get; set; }
        public DateTime date { get; set; }
        public int accountcount { get; set; }
        public int accountlimit { get; set; }
    }
} 