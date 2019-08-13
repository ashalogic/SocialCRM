using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Web.Models;

[assembly: OwinStartup(typeof(Web.Startup))]

namespace Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateAdmin();
        }

        public async Task CreateAdmin()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (usermanager.FindByEmail("nomail@nomail.no") == null)
            {
                var applicationUser = new ApplicationUser()
                {
                    PhoneNumber = "+905360654778",
                    UserName = "etio0098",
                    datetime = DateTime.Now,
                    EmailConfirmed = true,
                    Email = "nomail@nomail.no",
                    AccountLimit = 10
                };
                var result = await usermanager.CreateAsync(applicationUser, "ezio0098");
            }
        }
    }
}
