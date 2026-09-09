using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LoanPlatform.Scoring.Infrastructure.Persistence
{
    public sealed class ScoringDbContextFactory : IDesignTimeDbContextFactory<ScoringDbContext>
    {
        public ScoringDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ScoringDbContext> optionsBuilder = new();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=loanplatform_scoring;Username=postgres;Password=postgres");

            return new ScoringDbContext(optionsBuilder.Options);
        }
    }
}