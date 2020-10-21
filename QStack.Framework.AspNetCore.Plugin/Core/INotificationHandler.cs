using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public interface INotificationHandler
    {
        void Handle(string data);
    }
}
