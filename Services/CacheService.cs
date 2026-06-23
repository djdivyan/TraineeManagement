using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Exceptions;
using TraineeManagementApi.Models;
using TraineeManagementApi.Utilities;

namespace TraineeManagementApi.Services
{
    class CacheService(IDistributedCache cache, ILogger<CacheService> logger) : ICacheService
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<CacheService> _logger = logger;

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cache.GetOrSetAsync(key, factory, cancellationToken: cancellationToken, logger: _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Redis Unavailable, Switching to database");

            }

            return await factory();
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.SetAsync(key, value,cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Redis Unavailable during Set");
            }
        }
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Redis Unavailable during Set");
            }
        }

    }
}