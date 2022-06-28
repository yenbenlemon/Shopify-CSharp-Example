using System;
using System.Linq;
using ShopifyAppKyle.Data;
using ShopifyAppKyle.Models;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace ShopifyAppKyle.Cache
{
    public class UserCache : IUserCache
    {
        public UserCache(DataContext userDb, IAppCache cache)
        {
            _userDb = userDb;
            _cache = cache;
        }

        private readonly DataContext _userDb;
        private readonly IAppCache _cache;

        /// <summary>
        /// Gets the cache value for the given user.
        /// </summary>
        private CachedShopStatus LookupShopStatus(ICacheEntry entry)
        {
            var key = int.Parse((string) entry.Key);
            var user = _userDb.Users.FirstOrDefault(u => u.Id == key);

            if (user == null)
            {
                throw new NullReferenceException(nameof(user));
            }

            return new CachedShopStatus(user);
        }

        /// <summary>
        /// Deletes the shop's status from cache.
        /// </summary>
        public void ResetShopStatus(int userId)
        {
            _cache.Remove(userId.ToString());
        }

        /// <summary>
        /// Returns details about the given user's shop status.
        /// </summary>
        public CachedShopStatus GetShopStatus(int userId)
        {
            return _cache.GetOrAdd(userId.ToString(), LookupShopStatus);
        }
    }
}
