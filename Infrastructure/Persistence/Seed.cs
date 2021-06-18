using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class Seed
    {
        protected Seed()
        {
        }

        public static async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Seeding test users
            if (!userManager.Users.Any())
            {
                var users = new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "jose",
                        Email = "jose@test.com"
                    },
                    new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "maria",
                        Email = "maria@test.com"
                    }
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd1");
                }
            }
        }
    }
}
