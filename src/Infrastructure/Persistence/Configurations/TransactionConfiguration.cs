namespace Aerarium.Infrastructure.Persistence.Configurations;

using Aerarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2);

        builder.Property(t => t.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<int>();

        builder.Property(t => t.Category)
            .HasConversion<int>();

        builder.HasIndex(t => t.UserId);

        builder.HasIndex(t => new { t.UserId, t.Date });
    }
}
