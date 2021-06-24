using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// AccountConfiguration.
    /// </summary>
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.Property(x => x.Cash)
                .HasColumnType("money")
                .HasDefaultValueSql("((0))");
        }
    }
}
