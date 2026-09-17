using System.Text.Json;
using LoanPlatform.Scoring.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace LoanPlatform.Scoring.Infrastructure.Caching
{
    public sealed class RedisScoringCache : IScoringCache
    {
        private readonly IDistributedCache _cache;

        public RedisScoringCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            byte[]? cachedData = await _cache.GetAsync(key, cancellationToken);

            return cachedData is null
                ? default
                : JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
        {
            byte[] data = JsonSerializer.SerializeToUtf8Bytes(value);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };

            await _cache.SetAsync(key, data, options, cancellationToken);
        }

        public Task RemoveAsync(
            string key,
            CancellationToken cancellationToken)
        {
            return _cache.RemoveAsync(key, cancellationToken);
        }
    }
}