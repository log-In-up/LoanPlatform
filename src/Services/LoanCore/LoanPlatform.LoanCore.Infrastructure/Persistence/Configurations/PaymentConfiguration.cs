using LoanPlatform.LoanCore.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanPlatform.LoanCore.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        private const string PaymentsTableName = "payments";

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable(PaymentsTableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LoanId)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PrincipalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.InterestAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PaidAt)
                .IsRequired();

            builder.HasIndex(x => new { x.LoanId, x.PaidAt });
        }
    }
}