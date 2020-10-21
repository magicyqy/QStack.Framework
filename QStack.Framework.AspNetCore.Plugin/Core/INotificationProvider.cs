using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public interface INotificationProvider
    {
        Dictionary<string, List<INotificationHandler>> GetNotifications();
    }
}
