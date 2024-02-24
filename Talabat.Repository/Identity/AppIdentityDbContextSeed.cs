using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identitiy;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Ahmed Nasser",
                    Email = "ahmed.nasser@gmail.com",
                    PhoneNumber = "01234567890",
                    UserName = "ahmed.nasser"
                };
                await userManager.CreateAsync(User, "P@ssw0rd");
            }
        }
    }
}
