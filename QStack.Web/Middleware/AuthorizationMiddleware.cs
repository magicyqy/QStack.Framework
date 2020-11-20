using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QStack.Web.Middleware
{
    public class AuthorizationMiddleware
    {
        // Property key is used by Endpoint routing to determine if Authorization has run
        private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();
        private readonly RequestDelegate _next;
        ICompositeViewEngine viewEngine;
        IWebHostEnvironment environment;
        ITempDataProvider tempDataProvider;
       
       
        public AuthorizationMiddleware(RequestDelegate next, IWebHostEnvironment environment, ICompositeViewEngine engine, ITempDataProvider tempDataProvider)
        {
            _next = next;
            viewEngine = engine;
            this.environment = environment;
            this.tempDataProvider = tempDataProvider;
           


        }
        public async Task Invoke(HttpContext context, IUserService userService) //服务应该为scope,否则singleton下并发报错
        {

           
           
            //b.GetUrlHelper()
            var endpoint = context.GetEndpoint();
   
            if (endpoint != null)
            {
                // EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
                // The Authorization middleware can only make this claim if it observes an actual endpoint.
                context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
            }
            var allowAnonymous = endpoint?.Metadata.GetOrderedMetadata<IAllowAnonymous>() ?? Array.Empty<IAllowAnonymous>();
            if (allowAnonymous.Count() > 0)
            {
                await _next.Invoke(context);
                return;
            }
            var actionDescriptor = endpoint?.Metadata.GetOrderedMetadata<ControllerActionDescriptor>();

            var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();

            //var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            var shemeProviders = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

             var schemes = await shemeProviders.GetAllSchemesAsync();
            if (context.User?.Identity.IsAuthenticated == false)
            {
                //按照starup配置的其他schme,重新刷新一次登录认证
                ClaimsPrincipal newPrincipal = null;
              
                foreach (var scheme in schemes)
                {
                    var result = await context.AuthenticateAsync(scheme.Name);
                    if (result != null && result.Succeeded)
                    {
                        newPrincipal = MergeUserPrincipal(newPrincipal, result.Principal);
                    }
                }

                if (newPrincipal != null)
                {
                    context.User = newPrincipal;

                }
               
            }
            if(authorizeData.Any())
            {
                if(context.User?.Identity.IsAuthenticated==false)
                {
                    foreach (var scheme in schemes)
                    {
                        await context.ChallengeAsync(scheme.Name);
                    }
                    return;
                }
                var userId = Convert.ToInt32(context.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value);
                var userInfo = await userService.GetUserFunctions(userId);
                await UpdateSessionContext(context, userInfo);
                var path = $"/{actionDescriptor[0].RouteValues["area"] ?? ""}/{actionDescriptor[0].RouteValues["controller"]}/{actionDescriptor[0].RouteValues["action"]}";
                path = path.Replace("//", "/").ToLower();
                if (userInfo == null ||
                    (!userInfo.Roles.Any(r => r.Name.Equals("admin")) && !userInfo.Functions.Any(i => path.Equals(i.Path.ToLower()))))
                {
                    foreach (var scheme in schemes)
                    {
                        await context.ForbidAsync(scheme.Name);
                    }
                    return;
                }


            }
            await _next.Invoke(context);
 
        }

        private async Task UpdateSessionContext(HttpContext context, UserDto userInfo)
        {
            if (userInfo == null)
                return;
            var enviromentContext = context.RequestServices.GetService<IEnviromentContext>();
            if (enviromentContext == null)
                return;
            enviromentContext.AddEnviroment(nameof(IEnviromentContext.LoginUserId), userInfo.Id)
                .AddEnviroment(nameof(IEnviromentContext.LoginUserName), userInfo.Name)
                .AddEnviroment(nameof(IEnviromentContext.LoginGroupId), userInfo.GroupId.GetValueOrDefault())
                .AddEnviroment(nameof(IEnviromentContext.LoginGroupName), userInfo.GroupName)
                .AddEnviroment(nameof(IEnviromentContext.LoginRoleIds), userInfo.Roles?.Select(r => r.Id).ToList())
                .AddEnviroment(nameof(IEnviromentContext.LoginRoles), userInfo.Roles?.Select(r => r.Name).ToList())
                .AddEnviroment(nameof(IEnviromentContext.Client), context.Request.FromBrowserIsMobile() ? ClientEnum.Mobile : ClientEnum.PC);
            await context.RequestServices.GetService<IDataAuthService>()?.InitQueryFilters();
        }

        public static ClaimsPrincipal MergeUserPrincipal(ClaimsPrincipal existingPrincipal, ClaimsPrincipal additionalPrincipal)
        {
            var newPrincipal = new ClaimsPrincipal();

            // New principal identities go first
            if (additionalPrincipal != null)
            {
                newPrincipal.AddIdentities(additionalPrincipal.Identities);
            }

            // Then add any existing non empty or authenticated identities
            if (existingPrincipal != null)
            {
                newPrincipal.AddIdentities(existingPrincipal.Identities.Where(i => i.IsAuthenticated || i.Claims.Any()));
            }
            return newPrincipal;
        }
    }
}
