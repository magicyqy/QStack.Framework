using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QStack.Blog;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace Leaf.BlogTests
{
    public static class InitTestEnv
    {
        public static IHost Init()
        {
            var hostBuilder = new HostBuilder()
               .ConfigureAppConfiguration(x =>
               {
                   var basePath = Path.Combine("app_data", "config");
                   var appsettingsPath = Path.Combine(basePath, "appsettings.json");
                   var devAppsettingsPath = Path.Combine(basePath, "appsettings.Development.json");
                   if (File.Exists(appsettingsPath))
                   {
                       x.AddJsonFile(appsettingsPath);
                   }
                   if (File.Exists(devAppsettingsPath))
                   {
                       x.AddJsonFile(devAppsettingsPath);
                   }
                   x.AddInMemoryCollection(new Dictionary<string, string>() { { "PluginOptions:Enable", "false".ToString() } }.ToImmutableList());
               })
              .ConfigureWebHost(webHost =>
              {
                   // Add TestServer
                   webHost.UseTestServer();
                  webHost
                       .UseStartup<Startup>();
                 
                  webHost.UseEnvironment(Environments.Development);

              }).UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());

            return hostBuilder.Start();
        }

        public static HttpClient GetClientWithToken(this IHost host)
        {
            var token = string.Empty;
            using (var client = host.GetTestClient())
            {
                var formDta = new Dictionary<string, string> { { "username", "admin" }, { "password", "111111" } };

                var result = client.PostAsync("/api/account/login", new FormUrlEncodedContent(formDta.ToImmutableList())).Result;
                if (result.IsSuccessStatusCode)
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    var resResult =JsonDocument.Parse (res);
                    token=resResult.RootElement.GetProperty("data").GetProperty("token").GetString();
                   
                }
            }

            var newClient = host.GetTestClient();
            newClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return newClient;
        }
    }
}
