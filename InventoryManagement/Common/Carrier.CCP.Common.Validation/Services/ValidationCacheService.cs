using Carrier.CCP.Common.Cache.Redis;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Carrier.CCP.Common.Validation.Services
{
    public class ValidationCacheService : IValidationCacheService, IDisposable
    {
        private readonly string _commonBusinessCode;
        private readonly string _featureCode;
        private readonly IRedisCacheRepository _redisCacheRepository;
        private readonly SemaphoreSlim _semaphoreLock;
        private readonly MemoryCache cache;

        private string validationJsonFilePath;

        //public ValidationCacheService() {}

        public ValidationCacheService(IRedisCacheRepository redisCacheRepository, string featureCode,
            string commonBusinessCode)
        {
            _semaphoreLock = new SemaphoreSlim(1, 1);
            _redisCacheRepository = redisCacheRepository;
            _featureCode = featureCode.ToLowerInvariant();
            _commonBusinessCode = commonBusinessCode.ToLowerInvariant();
            validationJsonFilePath = string.Empty;
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void SetDefaultValidationFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            try
            {
                validationJsonFilePath = filePath;
                var jsonDictionary =
                    JsonConvert
                        .DeserializeObject<IDictionary<string, IDictionary<string, ValidatorsValidatorProperty>>>(
                            File.ReadAllText(validationJsonFilePath));
                foreach (var item in jsonDictionary)
                    CreateCacheItem(GetKey(_featureCode, item.Key.ToLowerInvariant(), _commonBusinessCode),
                        item.Value, true);
            }
            catch (Exception)
            {
                //TODO: Handle this
            }
        }

        public IDictionary<string, ValidatorsValidatorProperty> TryGetValidationData(string dtoName,
            string businessCode = null)
        {
            var valueObj = GetBusinessValidationData(dtoName, businessCode);
            //Get Default Value
            if (valueObj == null)
                cache.TryGetValue(GetKey(_featureCode, dtoName.ToLowerInvariant(), _commonBusinessCode), out valueObj);
            return valueObj;
        }

        private IDictionary<string, ValidatorsValidatorProperty> GetBusinessValidationData(string dtoName,
            string businessCode)
        {
            IDictionary<string, ValidatorsValidatorProperty> valueObj = null;
            //try business specific key
            if (string.IsNullOrEmpty(businessCode))
                return valueObj;

            var businessRedisKey = GetKey(_featureCode, dtoName.ToLowerInvariant(), businessCode.ToLowerInvariant());
            if (!cache.TryGetValue(businessRedisKey, out valueObj))
                valueObj = GetOrCreateRedisKey(businessRedisKey);
            return valueObj;
        }

        private IDictionary<string, ValidatorsValidatorProperty> GetOrCreateRedisKey(string key)
        {
            IDictionary<string, ValidatorsValidatorProperty> resultObj = null;
            try
            {
                var redisJson = _redisCacheRepository.GetString(key);
                if (!string.IsNullOrEmpty(redisJson))
                {
                    JObject jsonObj = JObject.Parse(redisJson);
                    var valueObj = jsonObj?.ToObject<IDictionary<string, ValidatorsValidatorProperty>>();
                    if (valueObj != null)
                        resultObj = CreateCacheItem(key, valueObj, true);
                }
            }
            catch (Exception)
            {
                resultObj = null;
            }
            return resultObj;
        }

        private string GetKey(string featureCode, string dtoName, string businessCode)
        {
            return
                $"inputvalidation|{businessCode.ToLowerInvariant()}|{featureCode.ToLowerInvariant()}|{dtoName.ToLowerInvariant()}";
        }

        private IDictionary<string, ValidatorsValidatorProperty> CreateCacheItem(string key,
            IDictionary<string, ValidatorsValidatorProperty> value, bool neverRemove = false)
        {
            IDictionary<string, ValidatorsValidatorProperty> result = null;
            try
            {
                if (!_semaphoreLock.Wait(0))
                {
                    return value;
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

                result = cache.Set(key, value, cacheEntryOptions);
            }
            catch { }
            finally
            {
                _semaphoreLock.Release();
            }
            // Save data in cache.
            return result;
        }

        public void Dispose()
        {
            _semaphoreLock.Dispose();
            cache.Dispose();
        }
    }
}