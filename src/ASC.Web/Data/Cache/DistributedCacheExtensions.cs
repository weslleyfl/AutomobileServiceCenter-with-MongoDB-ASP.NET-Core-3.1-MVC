﻿using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASC.Web.Data.Cache
{
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// - SlidingExpiration will reset the expiration time every time an entry is used before it expires
        /// - AbsoluteExpirationRelativeToNow will expire the entry after the given time, no matter how many times it’s been used
        /// - If you use both, the entry will expire when the first of the two times expire
        /// </summary>
        public static readonly DistributedCacheEntryOptions DefaultDistributedCacheEntryOptions
            = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(240),
                SlidingExpiration = TimeSpan.FromSeconds(120)
            };

        public static async Task<TObject> GetOrSetValueAsync<TObject>(this IDistributedCache cache,
            string key,
            Func<Task<TObject>> factory,
            DistributedCacheEntryOptions options = null) where TObject : class
        {
            var result = await cache.GetValueAsync<TObject>(key);
            if (result != null)
            {
                return result;
            }

            result = await factory();

            await cache.SetValueAsync(key, result, options);

            return result;
        }

        private static async Task<TObject> GetValueAsync<TObject>(this IDistributedCache cache, string key) where TObject : class
        {
            var data = await cache.GetStringAsync(key);
            if (data == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<TObject>(data);
        }

        private static async Task SetValueAsync<TObject>(this IDistributedCache cache,
            string key,
            TObject value,
            DistributedCacheEntryOptions options = null,
            CancellationToken token = default) where TObject : class
        {
            var data = JsonConvert.SerializeObject(value);

            await cache.SetStringAsync(key, data, options ?? DefaultDistributedCacheEntryOptions, token);
        }
    }
}
