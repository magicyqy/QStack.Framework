using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QStack.Framework.Core.Cache
{
    /// <summary>
    /// 缓存
    /// </summary>
    [AspectCore.DynamicProxy.NonAspect]
    public interface ICache
    {
        /// <summary>
        /// 是否存在指定键的缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        bool Exists(string key);
        T Get<T>(string key);
        /// <summary>
        /// 从缓存中获取数据，如果不存在，则执行获取数据操作并添加到缓存中
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">获取数据操作</param>
        /// <param name="expiration">过期时间间隔</param>
        T Get<T>(string key, Func<T> func, TimeSpan? expiration = null);
        /// <summary>
        /// 从缓存中获取数据，如果不存在，则执行获取数据操作并添加到缓存中
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">获取数据操作</param>
        /// <param name="expiration">过期时间间隔</param>
        Task<T> GetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null);
        /// <summary>
        /// 当缓存数据不存在则添加，已存在不会添加，添加成功返回true
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        /// <param name="expiration">过期时间间隔</param>
        bool TryAdd<T>(string key, T value, TimeSpan? expiration = null);
        /// <summary>
        /// 当缓存数据不存在则添加，已存在不会添加，添加成功返回true
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        /// <param name="expiration">过期时间间隔</param>
        Task<bool> TryAddAsync<T>(string key, T value, TimeSpan? expiration = null);
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        void Remove(string key);
        /// <summary>
        /// 清空缓存
        /// </summary>
        void Clear();
        /// <summary>
        /// 批量删除指定缓存键前缀的缓存项
        /// </summary>
        /// <param name="prefix">缓存键前缀</param>
        void RemoveByPrefix(string prefix);
        /// <summary>
        /// 批量删除指定缓存键前缀的缓存项
        /// </summary>
        /// <param name="prefix">缓存键前缀</param>
        Task RemoveByPrefixAsync(string prefix);
        Task<IDictionary<string, CacheValue<T>>> GetAllByPrefixAsync<T>(string prefix = "");
        IDictionary<string, CacheValue<T>> GetAllByPrefix<T>(string prefix = "");
    }
}
