using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web.Http;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class UWPController : ApiController
    {
        public IHttpActionResult Echo()
        {
            return Ok();
        }

        public async Task<IHttpActionResult> Add_Account(AccountAddViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.username))
            {
                var Db = new ApplicationDbContext();
                string uname = RequestContext.Principal.Identity.GetUserName();
                var user = Db.Users.Where(x => x.UserName == uname).FirstOrDefault();
                if (user != null)
                {
                    if (user.accounts.Count > user.AccountLimit)
                    {
                        return new System.Web.Http.Results.ExceptionResult(new Exception("Full"), this);
                    }
                    var accountmodel = new Account() { accounttype = model.accounttype, data = Convert.FromBase64String(model.data), id = Guid.NewGuid().ToString(), username = model.username, datetime = DateTime.Now };
                    var accountis = user.accounts.Where(x => x.username == accountmodel.username && x.accounttype == accountmodel.accounttype).FirstOrDefault();
                    if (accountis == null)
                    {
                        user.accounts.Add(accountmodel);
                        var result = await Db.SaveChangesAsync();
                        if (result == 1)
                            return Ok();
                        else
                            return new System.Web.Http.Results.ExceptionResult(new Exception(result.ToString()), this);
                    }
                    else
                    {
                        accountis.data = accountmodel.data;
                        var result = await Db.SaveChangesAsync();
                        if (result == 1)
                            return Ok();
                        else
                            return new System.Web.Http.Results.ExceptionResult(new Exception(result.ToString()), this);
                    }
                }
                return new System.Web.Http.Results.ExceptionResult(new Exception("No user"), this);
            }
            return new System.Web.Http.Results.ExceptionResult(new Exception("Err"), this);
        }
        public async Task<IHttpActionResult> Remove_Account(string id)
        {
            return null;
        }
        public IHttpActionResult Accounts_List()
        {
            List<AccountViewModel> result = new List<AccountViewModel>();
            var Db = new ApplicationDbContext();
            string uname = RequestContext.Principal.Identity.GetUserName();
            var user = Db.Users.Where(x => x.UserName == uname).FirstOrDefault();
            if (user != null)
            {
                foreach (var account in user.accounts)
                {
                    result.Add(new AccountViewModel() { id = account.id, accounttype = account.accounttype, data = Convert.ToBase64String(account.data), username = account.username });
                }
                return Ok(result);
            }
            return new System.Web.Http.Results.ExceptionResult(new Exception("No user"), this);
        }

        public async Task<IHttpActionResult> Add_Post(PostAddViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.accountusername))
            {
                var Db = new ApplicationDbContext();
                string uname = RequestContext.Principal.Identity.GetUserName();
                var user = Db.Users.Where(x => x.UserName == uname).FirstOrDefault();
                if (user != null)
                {
                    var account = user.accounts.Where(x => x.username == model.accountusername && x.accounttype == AccountsEnum.Instagram).FirstOrDefault();
                    if (account != null)
                    {
                        var postmodel = new ScheduledPost() { id = Guid.NewGuid().ToString(), type = model.type, datetime = DateTime.FromBinary(long.Parse(model.date)), post = Convert.FromBase64String(model.data) };
                        account.ScheduledPosts.Add(postmodel);
                        var result = await Db.SaveChangesAsync();
                        if (result == 1)
                            return Ok();
                        else
                            return new System.Web.Http.Results.ExceptionResult(new Exception(result.ToString()), this);
                    }
                    else
                        return new System.Web.Http.Results.ExceptionResult(new Exception("No Account"), this);
                }
                return new System.Web.Http.Results.ExceptionResult(new Exception("No user"), this);
            }
            return new System.Web.Http.Results.ExceptionResult(new Exception("No user"), this);
        }
        public async Task<IHttpActionResult> Remove_Post(SPostRReqViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.spid) && !string.IsNullOrWhiteSpace(model.acname))
            {
                var Db = new ApplicationDbContext();
                string uname = RequestContext.Principal.Identity.GetUserName();
                var user = Db.Users.Where(x => x.UserName == uname).FirstOrDefault();
                if (user != null)
                {
                    var account = user.accounts.Where(x => x.username == model.acname && x.accounttype == AccountsEnum.Instagram).FirstOrDefault();
                    if (account != null)
                    {
                        var sp = account.ScheduledPosts.Where(x => x.id == model.spid).FirstOrDefault();
                        if (sp != null)
                        {
                            account.ScheduledPosts.Remove(sp);
                            var result = await Db.SaveChangesAsync();
                            if (result == 1)
                                return Ok();
                            else
                                return new System.Web.Http.Results.ExceptionResult(new Exception(result.ToString()), this);
                        }
                    }
                    return new System.Web.Http.Results.ExceptionResult(new Exception(""), this);
                }
                return new System.Web.Http.Results.ExceptionResult(new Exception(""), this);
            }
            return new System.Web.Http.Results.ExceptionResult(new Exception(""), this);
        }

        public IHttpActionResult Posts_List(SPostReqViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.accountusername))
            {
                var Db = new ApplicationDbContext();
                string uname = RequestContext.Principal.Identity.GetUserName();
                var user = Db.Users.Where(x => x.UserName == uname).FirstOrDefault();
                if (user != null)
                {
                    var account = user.accounts.Where(x => x.username == model.accountusername && x.accounttype == AccountsEnum.Instagram).FirstOrDefault();
                    if (account != null)
                    {
                        var sp = account.ScheduledPosts;
                        List<SPostViewModel> modelresult = new List<SPostViewModel>();
                        foreach (var p in sp)
                        {
                            modelresult.Add(new SPostViewModel() { id = p.id, datetime = p.datetime, post = Convert.ToBase64String(p.post), status = p.status, statusdetails = p.statusdetails, type = p.type });
                        }
                        return Ok(modelresult);
                    }
                    return new System.Web.Http.Results.ExceptionResult(new Exception("No Account"), this);
                }
                return new System.Web.Http.Results.ExceptionResult(new Exception("No User"), this);
            }
            return new System.Web.Http.Results.ExceptionResult(new Exception("Err"), this);
        }

        private object FromByteArray(byte[] source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(source))
            {
                formatter.Deserialize(stream);
                return stream.ToArray();
            }
        }
    }
}
