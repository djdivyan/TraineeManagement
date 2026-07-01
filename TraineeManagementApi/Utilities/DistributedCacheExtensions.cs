using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;

namespace TraineeManagementApi.Utilities
{
    public static class DistributedCacheExtensions
    {
        private readonly static TimeSpan _slidingExpiration = TimeSpan.FromMinutes(30);
        private readonly static TimeSpan _absoluteExpiration = TimeSpan.FromHours(1);
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = null,
            WriteIndented = false,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static Task SetAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            CancellationToken cancellationToken = default)
        {
            return SetAsync(cache, key, value, new DistributedCacheEntryOptions()
                .SetSlidingExpiration(_slidingExpiration)
                .SetAbsoluteExpiration(_absoluteExpiration),
                cancellationToken);
        }

        public static Task SetAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, SerializerOptions));
            return cache.SetAsync(key, bytes, options, cancellationToken);
        }



        public static bool TryGetValue<T>(
            this IDistributedCache cache,
            string key,
            out T? value)
        {
            byte[]? val = cache.Get(key);
            value = default;
            if (val is null) return false;
            value = JsonSerializer.Deserialize<T>(val, SerializerOptions);
            return true;
        }

        public static async Task<T?> GetOrSetAsync<T>(
            this IDistributedCache cache,
            string key,
            Func<Task<T>> factory,
            DistributedCacheEntryOptions? options = null,
            CancellationToken cancellationToken = default,
            ILogger? logger = null)
        {
            ILogger safeLogger = logger ?? NullLogger.Instance;
            if (cache.TryGetValue(key, out T? value) && value is not null)
            {
                safeLogger.LogInformation("Cache hit for {cacheKey}", key);
                return value;
            }

            safeLogger.LogInformation("Cache miss for key: {CacheKey}. Fetching from factory.", key);
            value = await factory();

            if (value is not null)
            {
                options ??= new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(_slidingExpiration)
                    .SetAbsoluteExpiration(_absoluteExpiration);

                await cache.SetAsync(key, value, options, cancellationToken);
            }
            
            return value;
        }

    }

}