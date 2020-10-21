using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QStack.Framework.Core
{
    /// <summary>
    /// 使用AsyncLocal封装的LogicalThreadLocal,在async/awaite等异步环境中也可以用此来传递值
    /// 注意：进入异步代码后，修改此CallContext保存的值，退出异步代码切换回原上下文时，修改值会恢复为原值
    ///
    /// </summary>
    public static class CallContext
    {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();

        public static void SetData(string name, object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        public static object GetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
    }
}
