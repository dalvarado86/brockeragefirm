using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// IApplicationDbContext.
    /// </summary>
    public interface IApplicationDbContext
    {   
        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets the stocks.
        /// </summary>
        DbSet<Stock> Stocks { get; set; }

        /// <summary>
        /// Saves the changes in the database asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The canellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
