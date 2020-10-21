using AspectCore.Extensions.DependencyInjection;
using Leaf.Blog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using QStack.Blog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaf.BlogTests
{
    public  class TestApplicationFactory : WebApplicationFactory<TestApplicationFactory>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // use whatever config you want here
                    webBuilder.UseTestServer();
                    webBuilder.UseStartup<Startup>();
                })
                .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
        }
    }
   
}
