using NUnit.Framework;
using System.Text;
using Leaf.BlogTests;
using Newtonsoft.Json;
using System.Net.Http;
using QStack.Framework.Basic;

namespace Leaf.Blog.Areas.Api.Controllers.Tests
{
    [TestFixture()]
    public class AccountControllerTests : BaseTests
    {

        [Test()]
        public void UserInfoTest()
        {
            var client = host.GetClientWithToken();
           
            var response = client.GetAsync("/api/account/userInfo").Result;
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
        public void GetUsersTest()
        {
            var client = host.GetClientWithToken();
            var queryOptions = new QStack.Framework.Core.Entity.DataTableOption() { PageIndex = 1, PageSize = 20 };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(queryOptions), Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/account/GetUsers",content).Result;
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