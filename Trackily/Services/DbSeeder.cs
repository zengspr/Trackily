using Microsoft.AspNetCore.Identity;
using System;
using Trackily.Areas.Identity.Data;

namespace Trackily.Services
{
    public static class DbSeeder
    {
        public const string DevGuestName = "devguest";
        public const string DevGuestPassword = "devguest@trackily.ca";
        public const string ManagerGuestName = "manguest";
        public const string ManagerGuestPassword = "manguest@trackily.ca";

        public static void SeedUsers(UserManager<TrackilyUser> userManager)
        {
            if (userManager.FindByNameAsync(DevGuestName).Result == null)
            {
                var devGuest = new TrackilyUser
                {
                    Role = TrackilyUser.UserRole.Developer,
                    FirstName = "Developer",
                    LastName = "Guest",
                    UserName = DevGuestName,
                    Email = "devguest@trackily.ca"
                };

                var createResult = userManager.CreateAsync(devGuest, DevGuestPassword).Result;
                if (!createResult.Succeeded)
                {
                    throw new Exception("Error seeding database with devguest.");
                }
            }

            if (userManager.FindByNameAsync(ManagerGuestName).Result == null)
            {
                var manGuest = new TrackilyUser
                {
                    Role = TrackilyUser.UserRole.Manager,
                    FirstName = "Manager",
                    LastName = "Guest",
                    UserName = ManagerGuestName,
                    Email = "manguest@trackily.ca"
                };

                var createResult = userManager.CreateAsync(manGuest, ManagerGuestPassword).Result;
                if (!createResult.Succeeded)
                {
                    throw new Exception("Error seeding database with manguest.");
                }
            }
        }
    }
}
