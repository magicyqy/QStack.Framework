
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace QStack.Framework.Core
{
    public class DaoFactoryOption
    {
        public bool EnableLog { get; set; }
        public string DbType { get; set; }
        public string FactoryName { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }

        public bool EnableAutoMigration { get; set; }

        string[] _entityAssemblyNames;
        /// <summary>
        /// 用于强制更新efcore dbcontext model cache的版本号
        /// 此处是EntityAssemblies变化时必须变更这个字段
        /// </summary>
        long _version = DateTimeOffset.Now.Ticks;
        public string[] EntityAssemblyNames
        {
            get { return _entityAssemblyNames; }
            set
            {
                _entityAssemblyNames = value;
                if (value != null)
                {
                    foreach (var name in value)
                    {
                        if (string.IsNullOrWhiteSpace(name))
                            continue;
                        var assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(assembly => assembly.GetName().Name == name);
                        if (assembly == null)
                        {
                            var path = Path.Combine(AppContext.BaseDirectory, name);
                            if (File.Exists(path))
                                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                        }
                        if (assembly == null)
                            throw new ServiceFrameworkException($"error load EntityAssembilyName \"{name}\"");

                        EntityAssemblies.Add(assembly);
                    }
                }
            }
        }

        public HashSet<Assembly> EntityAssemblies { get; } = new HashSet<Assembly>();

        /// <summary>
        /// 通过hashcode强制刷新dbcontext model cache
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(EnableLog);
            hashCode.Add(DbType);
            hashCode.Add(FactoryName);
            hashCode.Add(ConnectionString);
            hashCode.Add(ProviderName);
            hashCode.Add(EnableAutoMigration);
            foreach(var assembly in EntityAssemblies)
                hashCode.Add(assembly.FullName);
            hashCode.Add(_version);
            return hashCode.ToHashCode();
        }

        public void UpdateCacheVersion()
        {
            _version = DateTimeOffset.Now.Ticks;
        }
    }
}
