using DotNetCore.ApiIntegrationTests.Fakes;
using DotNetCore.Models;
using DotNetCore.Models.Request;
using DotNetCore.Models.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNetCore.ApiIntegrationTests.Controllers
{
    [TestClass]
    public class ArticleControllerTests : BaseApiIntegrationTest
    {
        [TestInitialize]
        public void OnTestInitialize()
        {
        }

        [TestCleanup]
        public void OnTestCleanup()
        {
        }

        #region Get

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

        [TestMethod]
        public async Task Get_Should_Return_NotFound_Status_Code()
        {
            var response = await Client.GetAsync($"api/v1/articles/{int.MaxValue * -1}");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Get_Articles_Should_Return_Valid_ArticleResponse_Items()
        {
            var response = await Client.GetAsync($"api/v1/articles?{GetQueryString(new PagingRequest() { PageNumber = 1, PageSize = 10 })}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var getResponse = JsonConvert.DeserializeObject<TestPagedList<ArticleResponse>>(responseString);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(getResponse != null);
            Assert.IsTrue(getResponse.TotalCount > 0);
        }

        [TestMethod]
        public async Task Get_Articles_With_Invalid_PagingParameterRequest_Should_Return_BadRequest()
        {
            var response = await Client.GetAsync($"api/v1/articles?{GetQueryString(new PagingRequest() { PageNumber = 0, PageSize = 0 })}");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Post

        [TestMethod]
        public async Task Post_Article_Should_Create_Article()
        {
            var articleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = false,
                PublishDate = null,
                Author = "God",
            };

            var response = await Client.PostAsJsonAsync("api/v1/articles", articleRequest);
            var responseString = await response.Content.ReadAsStringAsync();

            var articleResponse = JsonConvert.DeserializeObject<ArticleResponse>(responseString);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(articleRequest.Title, articleResponse.Title);
            Assert.AreEqual(articleRequest.Body, articleResponse.Body);
            Assert.AreEqual(articleRequest.Author, articleResponse.Author);
            Assert.IsTrue(articleResponse.Id > 1);
        }

        [TestMethod]
        public async Task Post_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_Same_Title()
        {
            var articleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = false,
                PublishDate = null,
                Author = "God",
            };

            var response = await Client.PostAsJsonAsync("api/v1/articles", articleRequest);

            var responseString = await response.Content.ReadAsStringAsync();

            var articleResponse = JsonConvert.DeserializeObject<ArticleResponse>(responseString);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(articleRequest.Title, articleResponse.Title);
            Assert.AreEqual(articleRequest.Body, articleResponse.Body);
            Assert.AreEqual(articleRequest.Author, articleResponse.Author);
            Assert.IsTrue(articleResponse.Id > 1);


            var response2 = await Client.PostAsJsonAsync("api/v1/articles", articleRequest);
            var response2String = await response2.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [TestMethod]
        public async Task Post_Article_Should_Return_BadRequest_Response_When_Creating_Article_Invalid_Data()
        {
            var articleRequest = new CreateArticleRequest()
            {
                Title = null,
                Body = null,
                IsPublished = false,
                PublishDate = null,
                Author = null,
            };

            var response = await Client.PostAsJsonAsync("api/v1/articles", articleRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task Post_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_No_Data()
        {
            var articleRequest = new CreateArticleRequest();

            var response = await Client.PostAsJsonAsync("api/v1/articles", articleRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Put

        private void CreateArticle(CreateArticleRequest createArticleRequest,
            out HttpResponseMessage createHttpResponseMessage,
            out string createResponseString,
            out ArticleResponse createArticleResponse)
        {
            createHttpResponseMessage = Client.PostAsJsonAsync("api/v1/articles", createArticleRequest).Result;
            createResponseString = createHttpResponseMessage.Content.ReadAsStringAsync().Result;
            createArticleResponse = JsonConvert.DeserializeObject<ArticleResponse>(createResponseString);
        }

        private void UpdateArticle(int articleId, UpdateArticleRequest updateArticleRequest,
            out HttpResponseMessage updateHttpResponseMessage,
            out string updateResponseString)
        {
            updateHttpResponseMessage = Client.PutAsJsonAsync($"api/v1/articles/{articleId}", updateArticleRequest).Result;
            updateResponseString = updateHttpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        private void GetArticle(int articleId,
            out HttpResponseMessage getHttpResponseMessage,
            out string getResponseString,
            out ArticleResponse getArticleResponse)
        {
            getHttpResponseMessage = Client.GetAsync($"api/v1/articles/{articleId}").Result;
            getResponseString = getHttpResponseMessage.Content.ReadAsStringAsync().Result;

            if (getHttpResponseMessage.StatusCode != HttpStatusCode.NotFound)
                getArticleResponse = JsonConvert.DeserializeObject<ArticleResponse>(getResponseString);
            else
                getArticleResponse = null;
        }

        [TestMethod]
        public void Put_Article_Should_Update_Article()
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = false,
                PublishDate = null,
                Author = "God",
            };

            CreateArticle(createArticleRequest,
            out HttpResponseMessage createHttpResponseMessage,
            out string createResponseString,
            out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            var updateArticleRequest = new UpdateArticleRequest()
            {
                Author = GetUniqueStringValue(createArticleRequest.Author),
                Body = GetUniqueStringValue(createArticleRequest.Body),
                IsPublished = createArticleRequest.IsPublished,
                PublishDate = createArticleRequest.PublishDate,
                Title = GetUniqueStringValue(createArticleRequest.Title)
            };

            UpdateArticle(createArticleResponse.Id, updateArticleRequest, out HttpResponseMessage updateHttpResponseMessage, out string updateResponseString);

            Assert.AreEqual(HttpStatusCode.NoContent, updateHttpResponseMessage.StatusCode);

            GetArticle(createArticleResponse.Id,
            out HttpResponseMessage getHttpResponseMessage,
            out string getResponseString,
            out ArticleResponse getArticleResponse);

            getHttpResponseMessage.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, getHttpResponseMessage.StatusCode);
            Assert.AreEqual(updateArticleRequest.Title, getArticleResponse.Title);
            Assert.AreEqual(updateArticleRequest.Body, getArticleResponse.Body);
            Assert.AreEqual(updateArticleRequest.Author, getArticleResponse.Author);
            Assert.AreEqual(updateArticleRequest.IsPublished, getArticleResponse.IsPublished);
            Assert.AreEqual(updateArticleRequest.PublishDate, getArticleResponse.PublishDate);
        }

        [TestMethod]
        public void Put_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_Same_Title()
        {
            ArticleResponse CreateArticleRequestAndAssert()
            {
                var createArticleRequest = new CreateArticleRequest()
                {
                    Title = GetUniqueStringValue("title_"),
                    Body = GetUniqueStringValue("body_"),
                    IsPublished = false,
                    PublishDate = null,
                    Author = "God",
                };

                CreateArticle(createArticleRequest,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

                Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
                Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
                Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
                Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
                Assert.IsTrue(createArticleResponse.Id > 1);

                return createArticleResponse;
            }

            var createArticleResponse1 = CreateArticleRequestAndAssert();
            var createArticleResponse2 = CreateArticleRequestAndAssert();

            var updateArticleRequest = new UpdateArticleRequest()
            {
                Title = createArticleResponse1.Title,
                Body = GetUniqueStringValue("body_"),
                IsPublished = true,
                PublishDate = DateTime.UtcNow,
                Author = "God",
            };

            UpdateArticle(createArticleResponse2.Id, updateArticleRequest, out HttpResponseMessage updateHttpResponseMessage, out string updateResponseString);
            Assert.AreEqual(HttpStatusCode.BadRequest, updateHttpResponseMessage.StatusCode);
        }

        [TestMethod]
        public void Put_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_Invalid_Data()
        {
            ArticleResponse CreateArticleRequestAndAssert()
            {
                var createArticleRequest = new CreateArticleRequest()
                {
                    Title = GetUniqueStringValue("title_"),
                    Body = GetUniqueStringValue("body_"),
                    IsPublished = false,
                    PublishDate = null,
                    Author = "God",
                };

                CreateArticle(createArticleRequest,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

                Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
                Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
                Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
                Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
                Assert.IsTrue(createArticleResponse.Id > 1);

                return createArticleResponse;
            }

            var createArticleResponse1 = CreateArticleRequestAndAssert();
            var createArticleResponse2 = CreateArticleRequestAndAssert();

            var updateArticleRequest = new UpdateArticleRequest()
            {
                Title = null,
                Body = GetUniqueStringValue("body_"),
                IsPublished = true,
                PublishDate = DateTime.UtcNow,
                Author = "God",
            };

            UpdateArticle(createArticleResponse2.Id, updateArticleRequest, out HttpResponseMessage updateHttpResponseMessage, out string updateResponseString);
            Assert.AreEqual(HttpStatusCode.BadRequest, updateHttpResponseMessage.StatusCode);
        }

        #endregion

        #region Delete

        [TestMethod]
        public async Task Delete_Article_Should_Delete()
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = false,
                PublishDate = null,
                Author = "God",
            };

            CreateArticle(createArticleRequest,
            out HttpResponseMessage createHttpResponseMessage,
            out string createResponseString,
            out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            var response = await Client.DeleteAsync($"api/v1/articles/{createArticleResponse.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            GetArticle(createArticleResponse.Id, out HttpResponseMessage getHttpResponseMessage, out string getResponseString, out ArticleResponse getArticleResponse);

            Assert.AreEqual(HttpStatusCode.NotFound, getHttpResponseMessage.StatusCode);
        }

        #endregion
    }
}