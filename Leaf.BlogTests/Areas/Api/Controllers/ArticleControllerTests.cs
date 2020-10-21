using NUnit.Framework;
using System;
using System.Text;
using Microsoft.Extensions.Hosting;
using Leaf.BlogTests;
using QStack.Framework.Basic.ViewModel.Articles;
using System.Net.Http;
using Newtonsoft.Json;
using QStack.Framework.Basic;

namespace Leaf.Blog.Areas.Api.Controllers.Tests
{
    [TestFixture()]
    public class ArticleControllerTests
    {
        IHost host;
        [SetUp]
        public void Init()
        {
           
            host = InitTestEnv.Init();
            
        }
        [Test()]
        public void ArticleControllerTest()
        {
            //Assert.();
        }

        [Test()]
        public void PostTest()
        {
            var articleDto = new ArticleDto
            {
                Author = "admin",
                Title = "test",
                CoverUrl = "/aaa.png",
                Summary = "测试摘要，；来得及拉定积分拉科技管理科大驾光临级垃圾奥利给",
                Source = "博客园",
                SourceUrl = @"https://www.cnblogs.com/lonelyxmas/p/10909890.html",
                CatagoryName = "技术分享",
                ArticleType = QStack.Framework.Basic.Enum.ArticleType.Transmit,
                PageViews = 1,
                ArticleContentHtml = @"<p>I am testing data, I am testing data.</p><p><img src=""https://wpimg.wallstcn.com/4c69009c-0fd4-4153-b112-6cb53d1cf943"" /></p>",
                Tags = new TagDto[] { new TagDto { Name = ".net" }, new TagDto { Name = ".netcore" } }

            };
            var client = host.GetClientWithToken();
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            var json = JsonConvert.SerializeObject(articleDto);
            var response = client.PostAsync("/api/article/post", new StringContent(json, Encoding.UTF8, "application/json")).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var jsonObjet = JsonConvert.DeserializeObject<ResponseResult>(responseString);
                Assert.IsTrue(jsonObjet.Message.Contains("success"));
            }
            else
                Assert.Fail();
            client.Dispose();
        }

        [Test()]
        public void GetTest()
        {
            var client = host.GetClientWithToken();
            var response = client.GetAsync("/api/article/get/21").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
               
                var jsonObjet = JsonConvert.DeserializeObject<ResponseResult>(responseString);
                Assert.IsTrue(jsonObjet.Message.Contains("success"));
            }
            else
                Assert.Fail(responseString);
            client.Dispose();
        }

        [Test()]
        public void GetArticlesTest()
        {
            var client = host.GetClientWithToken();
            var queryOptions = new QStack.Framework.Core.Entity.DataTableOption() { PageIndex = 1, PageSize = 20 };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(queryOptions), Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/article/getarticles",content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
               
                var jsonObjet = JsonConvert.DeserializeObject<ResponseResult>(responseString);
                Assert.IsTrue(jsonObjet.Message.Contains("success"));
            }
            else
                Assert.Fail(responseString);
           
            client.Dispose();
        }
  

        [Test()]
        public void DeleteTest()
        {
            //Assert.Fail();
        }
    }
}