using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QStack.Framework.Core
{
    /// <summary>
    /// 登陆会话相关的上下文环境变量，如登陆用户信息，客户端来源.<br/>
    /// TODO:......
    /// </summary>
    public interface IEnviromentContext
    {
        [DisplayName("当前登陆用户Id")]
        public int LoginUserId { get;  }
        [DisplayName("当前登陆用户名")]
        public string LoginUserName { get;  }
        [DisplayName("当前登陆用户部门Id")]
        public int LoginGroupId { get;  }
        [DisplayName("当前登陆用户部门名")]
        public string LoginGroupName { get; }
        [DisplayName("当前登陆用户角色Ids")]
        public List<int> LoginRoleIds { get; }
        [DisplayName("当前登陆用户角色名s")]
        public List<string> LoginRoles { get; }
        [DisplayName("当前登陆客户端")]
        public ClientEnum Client { get;  }

        public IEnviromentContext AddEnviroment<T>(string key, T value); 
        /// <summary>
        /// 将环境属性/值，转换为字典模式如：{LoginUserId}:1
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToEnviromentDictionary();

       
    }

    public enum ClientEnum
    {
        PC,
        Mobile,
        App
    }

   

}
