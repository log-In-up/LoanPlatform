using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LoanPlatform.Scoring.Infrastructure.Persistence
{
    public sealed class ScoringDbContextFactory : IDesignTimeDbContextFactory<ScoringDbContext>
    {
        public ScoringDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ScoringDbContext> optionsBuilder = new();

            string connectionString =
                Environment.GetEnvironmentVariable(
                    "ConnectionStrings__ScoringDatabase")
                ?? "Host=localhost;Port=5432;Database=loanplatform_scoring;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new ScoringDbContext(optionsBuilder.Options);
        }
    }
}