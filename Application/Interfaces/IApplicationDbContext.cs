using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {     
        DbSet<Account> Accounts { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<Stock> Stocks { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
