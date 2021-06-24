using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// ApplicationDbContext.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The database context options.</param>
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        
        /// <inheritdoc/>
        public DbSet<Account> Accounts { get; set; }

        /// <inheritdoc/>
        public DbSet<Order> Orders { get; set; }

        /// <inheritdoc/>
        public DbSet<Stock> Stocks { get; set; }

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Starts a migration of the database for the first time.
        /// </summary>
        public void MigrateDB()
        {
            Policy
                .Handle<Exception>()
                .WaitAndRetry(5, r => TimeSpan.FromSeconds(10))
                .Execute(() => Database.Migrate());
        }
    }
}
