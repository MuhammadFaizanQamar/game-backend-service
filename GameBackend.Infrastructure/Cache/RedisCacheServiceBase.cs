using GameBackend.Core.Interfaces;

namespace GameBackend.Infrastructure.Cache;

public abstract class RedisCacheServiceBase : ICacheService
{
    public abstract Task<string?> GetAsync(string key);
    public abstract Task SetAsync(string key, string value, TimeSpan expiry);
    public abstract Task DeleteAsync(string key);
}