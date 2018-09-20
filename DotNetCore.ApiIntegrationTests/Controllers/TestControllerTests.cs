using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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

        [TestMethod]
        public async Task PingControllerHappyPath()
        {
            var response = await Client.GetAsync("api/Test");

            response.EnsureSuccessStatusCode();

            var responseStrong = await response.Content.ReadAsStringAsync();

            //responseStrong.Should().Be("Ping");

            Assert.IsNotNull(responseStrong);
        }
    }
}
