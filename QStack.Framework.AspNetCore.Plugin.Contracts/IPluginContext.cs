using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Contracts
{
    /// <summary>
    /// 插件的上下文
    /// </summary>
    public interface IPluginContext
    {
        string RouteArea { get; }
        /// <summary>
        /// 测试地址
        /// </summary>
        string TestUrl { get; }
        /// <summary>
        /// 返回插件中定义的entity所在程序集,若无，必须返回一个空字典集合<br/>
        /// key:daoFactoryName <br/>
        /// value: 程序集列表
        /// </summary>
        Dictionary<string, List<Assembly>> PluginEntityAssemblies { get; }

        /// <summary>
        /// 返回插件中定义的服务，若无，须返回空集合
        /// </summary>
        /// <returns></returns>
        IServiceCollection Services { get; }
        /// <summary>
        /// 返回插件中定义的automapper映射类，若无，须返回空集合
        /// </summary>
        /// <returns></returns>
        List<Profile> AutoMapperProfiles { get; }
    }
}
