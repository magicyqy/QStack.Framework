using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Extensions
{
    public static class IMemoryCacheExtentions
    {
        /// <summary>
        /// Clear IMemoryCache
        /// </summary>
        /// <param name="cache">Cache</param>
        /// <exception cref="InvalidOperationException">Unable to clear memory cache</exception>
        /// <exception cref="ArgumentNullException">Cache is null</exception>
        public static void Clear(this IMemoryCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("Memory cache must not be null");
            }
            else if (cache is MemoryCache memCache)
            {
                memCache.Compact(1.0);
                return;
            }
            else
            {
                MethodInfo clearMethod = cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                if (clearMethod != null)
                {
                    clearMethod.Invoke(cache, null);
                    return;
                }
                else
                {
                    PropertyInfo prop = cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                    if (prop != null)
                    {
                        object innerCache = prop.GetValue(cache);
                        if (innerCache != null)
                        {
                            clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                            if (clearMethod != null)
                            {
                                clearMethod.Invoke(innerCache, null);
                                return;
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("Unable to clear memory cache instance of type " + cache.GetType().FullName);
        }
    }
}
