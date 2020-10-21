using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QStack.Framework.Core
{
    public  class EnviromentContext:IEnviromentContext
    {
        public IEnviromentContext AddEnviroment<T>(string enviromentKey, T value)
        {
            var propertyInfo = this.GetType().GetProperty(enviromentKey, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo != null && typeof(T) == propertyInfo.PropertyType)
            {
                propertyInfo.SetValue(this, value);
            }
            return this;
        }

        public Dictionary<string, object> ToEnviromentDictionary()
        {
            var envDic = new Dictionary<string, object>();
            var env_properties = typeof(IEnviromentContext).GetProperties();
            foreach (var p in env_properties)
            {
                var value = p.GetValue(this);
                envDic.Add($"{{{p.Name}}}", value);
            }
            return envDic;

        }
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(LoginUserId);
            hash.Add(LoginUserName);
            hash.Add(LoginGroupId);
            hash.Add(LoginGroupName);
            hash.Add(Client);
            for(var i=0;i<LoginRoleIds?.Count;i++)
                    hash.Add(LoginRoleIds[i]);
            for (var i = 0; i < LoginRoles?.Count; i++)
                hash.Add(LoginRoles[i]);
            return hash.ToHashCode();
        }

        #region enviroments 上下文环境
        public int LoginUserId { get; private set; }
        public string LoginUserName { get; private set; }
        public int LoginGroupId { get; private set; }
        public string LoginGroupName { get; private set; }
        public List<int> LoginRoleIds { get; private set; }
        public List<string> LoginRoles { get; private set; }
        public ClientEnum Client { get; private set; }
        #endregion
    }
}
