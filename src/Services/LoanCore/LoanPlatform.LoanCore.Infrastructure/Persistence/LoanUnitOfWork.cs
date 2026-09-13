using LoanPlatform.LoanCore.Application.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace LoanPlatform.LoanCore.Infrastructure.Persistence
{
    public sealed class LoanUnitOfWork : IUnitOfWork
    {
        private readonly LoanDbContext _dbContext;

        public LoanUnitOfWork(LoanDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> action,
            CancellationToken cancellationToken = default)
        {
            await using IDbContextTransaction transaction =
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await action(cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}