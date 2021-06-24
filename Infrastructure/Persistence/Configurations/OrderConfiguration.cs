using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// OrderConfiguration.
    /// </summary>
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.SharePrice)
                .HasColumnType("money")
                .HasDefaultValueSql("((0))");
        }
    }
}
