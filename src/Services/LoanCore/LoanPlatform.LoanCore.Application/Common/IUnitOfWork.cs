namespace LoanPlatform.LoanCore.Application.Common
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> action,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}