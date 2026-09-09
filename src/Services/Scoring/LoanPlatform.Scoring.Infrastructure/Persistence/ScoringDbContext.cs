using LoanPlatform.Scoring.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.Scoring.Infrastructure.Persistence
{
    public sealed class ScoringDbContext : DbContext
    {
        public DbSet<CreditApplication> CreditApplications => Set<CreditApplication>();
        public DbSet<CreditScore> CreditScores => Set<CreditScore>();
        
        public ScoringDbContext(DbContextOptions<ScoringDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScoringDbContext).Assembly);
        }
    }
}