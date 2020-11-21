using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Core.Cache
{
    /// <summary>
    /// 标记缓存服务方法，注意如果存在服务接口，则需在接口方法上声明
    /// （在实现类方法上标记无法进入拦截器，因为aspectcore不支持）
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CachingAttribute:Attribute
    {
        public CacheMethod Method { get; set; }
        /// <summary>
        /// 获取或设置与当前缓存方式相关的方法名称。注：此参数仅在缓存方式为Remove时起作用。
        /// </summary>
        public string[] CorrespondingMethodNames { get; set; }
        /// <summary>
        /// 缓存时间，默认2小时
        /// </summary>
        public TimeSpan Expiration { get; set; } =TimeSpan.FromHours(2);
        public CachingAttribute(CacheMethod method)
        {
            Method = method;
           
        }

             /// <summary>
             /// 初始化一个新的<c>CachingAttribute</c>类型。
             /// </summary>
             /// <param name="method">缓存方式。</param>
             /// <param name="correspondingMethodNames">与当前缓存方式相关的方法名称。注：此参数仅在缓存方式为Remove时起作用。</param>
        public CachingAttribute(CacheMethod method, params string[] correspondingMethodNames)
            : this(method)
        {
            this.CorrespondingMethodNames = correspondingMethodNames;
           
        }
        public CachingAttribute(CacheMethod method, TimeSpan expiration)
            : this(method)
        {
            
            Expiration = expiration;
        }
    }

    public enum CacheMethod
    {
        Get,
        Remove
    }
}
