using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Leaf.BlogTests;
using QStack.Blog.Controllers;

namespace Leaf.Blog.Controllers.Tests
{
    [TestFixture()]
    public class HomeControllerTests:BaseTests
    {
       
       
        [Test()]
        public void IndexTest()
        {
            var result =host.Services.GetService<HomeController>().Index().Result;
            Assert.Pass(result.ToString());
        }
    }
}