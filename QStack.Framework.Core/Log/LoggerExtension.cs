using Microsoft.Extensions.Logging;
using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Core.Log
{
    /// <summary>
    /// 适配serilog的内置logging扩展，简单为主，不过度封装
    /// </summary>
    public static class LoggerExtension
    {
        public static void LogInfo(this ILogger logger,object log)
        {
            if (log.GetType().IsPrimitive || log.GetType() == typeof(string))
                logger.LogInformation(log.SafeString());
            else
                logger.LogInformation($"{{@{nameof(log) }}}", log);

        }


        public static void LogException(this ILogger logger,Exception exception,object log=null)
        {
            if (log==null||log.GetType().IsPrimitive || log.GetType() == typeof(string))
                logger.LogError(exception,log.SafeString());
            else
                logger.LogError(exception, $"{{@{nameof(log) }}}", log);
        }
    }
}
