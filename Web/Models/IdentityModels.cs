using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        #region Properties
        public string companyname { get; set; }
        public DateTime datetime { get; set; }
        public bool Blocked { get; set; }
        public int AccountLimit { get; set; }
        #endregion

        public virtual ICollection<Account> accounts { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    [Table("Accounts")]
    public class Account
    {
        #region Properties
        [Key]
        public string id { get; set; }
        public AccountsEnum accounttype { get; set; }
        public string username { get; set; }
        public byte[] data { get; set; }
        public DateTime datetime { get; set; }
        #endregion

        public string OwnerID { get; set; }
        [ForeignKey("OwnerID")]
        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<ScheduledPost> ScheduledPosts { get; set; }
    }

    [Table("ScheduledPosts")]
    public class ScheduledPost
    {
        #region Properties
        [Key]
        public string id { get; set; }
        public string type { get; set; }
        public byte[] post { get; set; }
        public DateTime datetime { get; set; }
        public bool status { get; set; }
        public string statusdetails { get; set; }
        #endregion

        public string OwnerID { get; set; }
        [ForeignKey("OwnerID")]
        public virtual Account Owner { get; set; }
    }

    public enum AccountsEnum
    {
        Instagram, Telegram, Twitter, Linkedin, Aparat
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("SocialMindCRMDbConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<ScheduledPost> ScheduledPosts { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}