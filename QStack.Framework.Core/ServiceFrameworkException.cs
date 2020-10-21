using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QStack.Framework.Core
{
    [Serializable]
    public class ServiceFrameworkException:ApplicationException
    {
        public ServiceFrameworkException() : base() { }
        public ServiceFrameworkException(string message) : base(message) { }
        protected ServiceFrameworkException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ServiceFrameworkException(string message, Exception innerException) : base(message, innerException) { }
    }
}
