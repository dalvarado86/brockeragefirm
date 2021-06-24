using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Seed.
    /// </summary>
    public class Seed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Seed"/> class.
        /// </summary>
        protected Seed()
        {
        }

        /// <summary>
        /// Seeds the initial data.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <returns></returns>
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
