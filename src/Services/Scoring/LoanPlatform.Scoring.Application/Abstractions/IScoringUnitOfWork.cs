namespace LoanPlatform.Scoring.Application.Abstractions
{
    public interface IScoringUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}