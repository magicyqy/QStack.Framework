using AspectCore.DynamicProxy;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace QStack.Framework.Core.Persistent
{
    public class IgnoreQueryFiltersAttribute : AbstractInterceptorAttribute
    {
        private IEnumerable<LambdaExpression> queryFilters;
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var sessionContext = context.ServiceProvider.GetService<SessionContext>();
            queryFilters = sessionContext.QueryFilters;
            sessionContext.QueryFilters = null;
            await next(context);
            sessionContext.QueryFilters = queryFilters;
        }
    }
}
