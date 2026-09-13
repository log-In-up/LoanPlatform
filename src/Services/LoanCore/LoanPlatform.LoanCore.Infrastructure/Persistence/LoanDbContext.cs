using LoanPlatform.LoanCore.Domain.Loans;
using LoanPlatform.LoanCore.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.LoanCore.Infrastructure.Persistence
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
        {
        }

        public DbSet<Loan> Loans => Set<Loan>();

        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}