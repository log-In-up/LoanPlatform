using LoanPlatform.LoanCore.Domain.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanPlatform.LoanCore.Infrastructure.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    private const string LoansTableName = "loans";

    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable(LoansTableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApplicantIdentifier)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.PrincipalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PaidPrincipalAmount)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(x => x.InterestRate)
            .HasPrecision(8, 4)
            .IsRequired();

        builder.Property(x => x.TermMonths)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.PaymentType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}