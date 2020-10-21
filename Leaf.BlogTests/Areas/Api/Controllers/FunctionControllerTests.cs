using NUnit.Framework;
using System;
using System.Text;
using Leaf.BlogTests;
using System.Net.Http;
using Newtonsoft.Json;
using QStack.Framework.Basic;

namespace Leaf.Blog.Areas.Api.Controllers.Tests
{
    [TestFixture()]
    public class FunctionControllerTests:BaseTests
    {
        [Test()]
        public void PostTest()
        {
            var client = host.GetClientWithToken();
            var functionDto =JsonConvert.DeserializeObject(
                "{\"id\":0,\"name\":\"文件管理\",\"code\":\"101001009\",\"describe\":\"\",\"sequence\":0,\"parentId\":1,\"parentName\":\"系统\",\"roles\":[],\"iconUrl\":\"folder\",\"path\":\"/filebroswe\",\"IsLeaf\":false,\"functionType\":0,\"hidden\":false,\"routeName\":\"/filebrowse\"}"
            );
            HttpContent content = new StringContent(JsonConvert.SerializeObject(functionDto), Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/function/post", content).Result;
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
    }
}