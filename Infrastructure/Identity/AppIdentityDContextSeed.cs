using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "John",
                    Email = "john@gmail.com",
                    UserName = "john@gmail.com",
                    Address = new Address()
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Street = "10 st",
                        City = "NewYork",
                        State = "NY",
                        ZipCode = "12345"
                    }
                };
                await userManager.CreateAsync(user,"Password123!");
            }
        }
    }
}
