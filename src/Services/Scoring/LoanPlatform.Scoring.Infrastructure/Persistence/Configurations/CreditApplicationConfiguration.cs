using LoanPlatform.Scoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanPlatform.Scoring.Infrastructure.Persistence.Configurations
{
    public sealed class CreditApplicationConfiguration : IEntityTypeConfiguration<CreditApplication>
    {
        private const string CreditApplicationsKey = "credit_applications";
        private const string DataBaseName = "ix_credit_applications_applicant_identifier";

        public void Configure(EntityTypeBuilder<CreditApplication> builder)
        {
            builder.ToTable(CreditApplicationsKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.ApplicantIdentifier)
                .HasConversion(
                    identifier => identifier.Value,
                    value => new Domain.ValueObjects.ApplicantIdentifier(value))
                .HasMaxLength(12)
                .IsRequired();

            builder.Property(x => x.ApplicantType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RequestedAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.RequestedTermMonths)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Decision)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.ApplicantIdentifier)
                .HasDatabaseName(DataBaseName);
        }
    }
}