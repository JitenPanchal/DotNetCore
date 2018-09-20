using DotNetCore.Models.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.ApiIntegrationTests.Controllers
{
    [TestClass]
    public class TestControllerTests : BaseApiIntegrationTest
    {
        [TestInitialize]
        public void OnTestInitialize()
        {
           
        }

        [TestCleanup]
        public void OnTestCleanup()
        {
           
        }

        //[TestMethod]
        //public async Task PingControllerHappyPath()
        //{
        //    var response = await Client.GetAsync("api/Test");

        //    response.EnsureSuccessStatusCode();

        //    var responseStrong = await response.Content.ReadAsStringAsync();

        //    //responseStrong.Should().Be("Ping");

        //    Assert.IsNotNull(responseStrong);
        //}

        [TestMethod]
        public async Task Get_Should_Return_Valid_ArticleResponse()
        {
            var response = await Client.GetAsync("api/v1/articles/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var articleResponse = JsonConvert.DeserializeObject<ArticleResponse>(responseString);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(articleResponse.Id > 0);
        }
    }
}
