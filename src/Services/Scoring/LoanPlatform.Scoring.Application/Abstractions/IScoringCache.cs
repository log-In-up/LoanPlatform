namespace LoanPlatform.Scoring.Application.Abstractions
{
    public interface IScoringCache
    {
        Task<T?> GetAsync<T>(
            string key,
            CancellationToken cancellationToken);

        Task SetAsync<T>(
            string key,
            T value,
            TimeSpan expiration,
            CancellationToken cancellationToken);

        Task RemoveAsync(
            string key,
            CancellationToken cancellationToken);
    }
}
