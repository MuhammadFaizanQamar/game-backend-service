using GameBackend.Core.Interfaces;
using StackExchange.Redis;

namespace GameBackend.Infrastructure.Cache;

public class RedisCacheService_Azure : RedisCacheServiceBase
{
    private readonly IDatabase _database;

    public RedisCacheService_Azure(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public override async Task<string?> GetAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public override async Task SetAsync(string key, string value, TimeSpan expiry)
    {
        await _database.StringSetAsync(key, value, expiry);
    }

    public override async Task DeleteAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}