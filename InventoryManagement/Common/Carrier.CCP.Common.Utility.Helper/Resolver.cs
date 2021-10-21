using Carrier.CCP.Common.Cache.Redis;
using Carrier.CCP.Common.Model;
using Carrier.CCP.Common.Repository.Sql;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    public class Resolver : IResolver, IDisposable
    {
        private readonly IGenericRepository<Business> businessRepository;
        private readonly IRedisCacheRepository redisCacheRepository;
        private readonly SemaphoreSlim _semaphoreLock;
        private readonly MemoryCache cache;

        public Resolver(IRedisCacheRepository redisCacheRepository, IGenericRepository<Business> businessRepository)
        {
            this.redisCacheRepository = redisCacheRepository;
            this.businessRepository = businessRepository;
            _semaphoreLock = new SemaphoreSlim(1, 1);
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<string> GetBusinessSchemaAsync(string businessId)
        {
            try
            {
                string bKey = $"ccp|business|{businessId.ToLowerInvariant()}";
                if (!cache.TryGetValue(bKey, out string businessCode))
                    businessCode = GetOrCreateRedisKey(bKey);

                if (businessCode == null)
                    businessCode = await GetOrCreateBusinessKeyAsync(businessId);

                return businessCode;
            }
            catch (Exception)
            {
                return await GetOrCreateBusinessKeyAsync(businessId);
            }
        }

        public string GetBusinessSchema(string businessId)
        {
            try
            {
                string bKey = $"ccp|business|{businessId.ToLowerInvariant()}";
                if (!cache.TryGetValue(bKey, out string businessCode))
                    businessCode = GetOrCreateRedisKey(bKey);

                if (businessCode == null)
                    businessCode = GetOrCreateBusinessKey(businessId);

                return businessCode;
            }
            catch (Exception)
            {
                return GetOrCreateBusinessKey(businessId);
            }
        }

        private void CreateCacheItem(string key, string value, bool neverRemove = false)
        {
            try
            {
                if (!_semaphoreLock.Wait(0))
                {
                    return;
                }

                MemoryCacheEntryOptions cacheEntryOptions;
                if (neverRemove)
                    cacheEntryOptions = new MemoryCacheEntryOptions()
                        //Priority on removing when reaching size limit (memory pressure)
                        .SetPriority(CacheItemPriority.NeverRemove);
                else
                    cacheEntryOptions = new MemoryCacheEntryOptions()
                        //Priority on removing when reaching size limit (memory pressure)
                        .SetPriority(CacheItemPriority.High)
                        // Remove from cache after this time, regardless of sliding expiration
                        .SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddDays(1));

                // Save data in cache.
                _ = cache.Set(key, value, cacheEntryOptions);
            }
            catch { }
            finally
            {
                _semaphoreLock.Release();
            }
        }

        private string GetOrCreateRedisKey(string key)
        {
            string resultObj = null;
            try
            {
                resultObj = redisCacheRepository.GetString(key);
                if (!string.IsNullOrEmpty(resultObj))
                    CreateCacheItem(key, resultObj, true);
            }
            catch
            {
                //supressing the error
            }

            return resultObj;
        }

        private string GetOrCreateBusinessKey(string businessId)
        {
            string resultObj = null;
            try
            {
                Guid businessGuid;
                Guid.TryParse(businessId, out businessGuid);
                resultObj = businessRepository.Get("BusinessId", businessGuid, "ccp")?.BusinessCode;

                if (!string.IsNullOrEmpty(resultObj))
                    CreateCacheItem($"ccp|business|{businessId.ToLowerInvariant()}", resultObj, true);
            }
            catch
            {
            }

            return resultObj;
        }

        private async Task<string> GetOrCreateBusinessKeyAsync(string businessId)
        {
            string resultObj = null;
            try
            {
                Guid businessGuid;
                Guid.TryParse(businessId, out businessGuid);
                var valueObj = (await businessRepository.GetAsync("BusinessId", businessGuid, "ccp").ConfigureAwait(false))?.BusinessCode;

                if (!string.IsNullOrEmpty(resultObj))
                    CreateCacheItem($"ccp|business|{businessId.ToLowerInvariant()}", resultObj, true);
            }
            catch 
            {
            }

            return resultObj;
        }

        public void Dispose()
        {
            _semaphoreLock.Dispose();
            cache.Dispose();
        }
    }
}