using LoanPlatform.Scoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanPlatform.Scoring.Infrastructure.Persistence.Configurations
{
    public sealed class CreditScoreConfiguration : IEntityTypeConfiguration<CreditScore>
    {
        private static readonly string CreditScoreKey = "CreditScore";
        private static readonly string DataBaseName = "ix_credit_scores_application_id";
        
        public void Configure(EntityTypeBuilder<CreditScore> builder)
        {
            builder.ToTable(CreditScoreKey);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.ApplicationId)
                .IsRequired();

            builder.Property(x => x.Score)
                .IsRequired();

            builder.Property(x => x.Decision)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CalculatedAt)
                .IsRequired();

            builder.HasIndex(x => x.ApplicationId)
                .HasDatabaseName(DataBaseName);
        }
    }
}