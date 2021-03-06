﻿using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Sistrategia.Drive.Business
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class SecurityUserManager : UserManager<SecurityUser, int>
    {
        public SecurityUserManager(IUserStore<SecurityUser, int> store)
            : base(store) {
        }
        //public SecurityUserManager(IUserStore<SecurityUser> store)
        //    : base(store) {
        //}
        //public SecurityUserManager(UserStore<SecurityUser, SecurityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim> store)
        //    : base(store as IUserStore<SecurityUser>) {

        //}
        //public SecurityUserManager(SecurityUserStore store)
        //    : base(store)  {

        //}

        public static SecurityUserManager Create(IdentityFactoryOptions<SecurityUserManager> options, IOwinContext context) {
            //var manager = new SecurityUserManager(new UserStore<SecurityUser>(context.Get<ApplicationDbContext>()));
            //var manager = new SecurityUserManager(new UserStore<SecurityUser, SecurityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(context.Get<ApplicationDbContext>()));
            var manager = new SecurityUserManager(new SecurityUserStore(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<SecurityUser, int>(manager) {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false, //true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<SecurityUser, int> {
            //    MessageFormat = "Your security code is {0}"
            //});
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<SecurityUser, int> {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null) {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<SecurityUser, int>(dataProtectionProvider.Create("ASP.NET Identity")); // {
                //  TokenLifespan = TimeSpan.FromHours(3)
                //};
            }

            return manager;
        }
    }
}
