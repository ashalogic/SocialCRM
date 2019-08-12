using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Users = "amiribabak")]
    public class AdminController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(RegisterUserViewModel model)
        {
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!ModelState.IsValid)
                return new System.Web.Http.Results.ExceptionResult(new Exception("Model Not Valid"), this);
            var result = await usermanager.CreateAsync(new ApplicationUser() { PhoneNumber = model.phonenumber, AccountLimit = model.accountlimit, datetime = DateTime.Now, UserName = model.username, Email = model.email, companyname = model.companyname }, model.password);
            if (result.Succeeded)
            {
                return Ok(model.username);
            }
            else
            {
                return new System.Web.Http.Results.ExceptionResult(new Exception(result.Errors.ToList()[0]), this);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> BlockUser(BlockUnBlockReqViewModel model)
        {
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = await usermanager.FindByNameAsync(model.username);
            if (user != null)
            {
                user.Blocked = true;
                var result = await usermanager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(user.UserName);
                }
                else
                {
                    return new System.Web.Http.Results.ExceptionResult(new Exception(result.Errors.ToList()[0]), this);
                }
            }
            else
            {
                return new System.Web.Http.Results.ExceptionResult(new Exception("Nothing Here Baby :("), this);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UnBlockUser(BlockUnBlockReqViewModel model)
        {
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = await usermanager.FindByNameAsync(model.username);
            if (user != null)
            {
                user.Blocked = false;
                var result = await usermanager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(user.UserName);
                }
                else
                {
                    return new System.Web.Http.Results.ExceptionResult(new Exception(result.Errors.ToList()[0]), this);
                }
            }
            else
            {
                return new System.Web.Http.Results.ExceptionResult(new Exception("Nothing Here Baby :("), this);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> RemoveUser(RemoveReqViewModel model)
        {
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = await usermanager.FindByNameAsync(model.username);
            if (user != null)
            {
                var result = await usermanager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(user.UserName);
                }
                else
                {
                    return new System.Web.Http.Results.ExceptionResult(new Exception(result.Errors.ToList()[0]), this);
                }
            }
            else
            {
                return new System.Web.Http.Results.ExceptionResult(new Exception("Nothing Here Baby :("), this);
            }
        }

        [HttpPost]
        public IHttpActionResult UsersList()
        {
            var applicationUsers = context.Users.ToList();
            UsersViewModel model = new UsersViewModel();
            foreach (var u in applicationUsers)
            {
                int c = u.accounts.Count;
                model.users.Add(new UserViewModel() { companyname = u.companyname, email = u.Email, username = u.UserName, blocked = u.Blocked, accountcount = c, date = u.datetime, accountlimit = u.AccountLimit, phonenumber = u.PhoneNumber });
            }
            return Ok(model);
        }
    }
}
