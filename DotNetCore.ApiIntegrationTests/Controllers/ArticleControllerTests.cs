using DotNetCore.ApiIntegrationTests.Fakes;
using DotNetCore.Enums;
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
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Post_Article_Should_Create_Article(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = false,
                PublishDate = null,
                Author = "God",
            };

            CreateArticle(createArticleRequest, dataFormat,
            out HttpResponseMessage createHttpResponseMessage,
            out string createResponseString,
            out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);
        }

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Post_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_Same_Title(DataFormat dataFormat)
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
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            CreateArticle(createArticleRequest,
                dataFormat, 
                out HttpResponseMessage createHttpResponseMessage2,
                out string createResponseString2,
                out ArticleResponse createArticleResponse2);

            Assert.AreEqual(HttpStatusCode.BadRequest, createHttpResponseMessage2.StatusCode);
        }

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Post_Article_Should_Return_BadRequest_Response_When_Creating_Article_Invalid_Data(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = null,
                Body = null,
                IsPublished = false,
                PublishDate = null,
                Author = null,
            };

            CreateArticle(createArticleRequest,
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.BadRequest, createHttpResponseMessage.StatusCode);
        }

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Post_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_No_Data(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest();

            CreateArticle(createArticleRequest,
              dataFormat,
              out HttpResponseMessage createHttpResponseMessage,
              out string createResponseString,
              out ArticleResponse createArticleResponse);


            Assert.AreEqual(HttpStatusCode.BadRequest, createHttpResponseMessage.StatusCode);
        }

        #endregion

        #region Put

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Put_Article_Should_Update_Article(DataFormat dataFormat)
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
                 dataFormat,
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

            UpdateArticle(createArticleResponse.Id, updateArticleRequest, dataFormat, out HttpResponseMessage updateHttpResponseMessage, out string updateResponseString);

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
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Put_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_Same_Title(DataFormat dataFormat)
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
                dataFormat,
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

            UpdateArticle(createArticleResponse2.Id, updateArticleRequest, dataFormat, out HttpResponseMessage updateHttpResponseMessage, out string updateResponseString);
            Assert.AreEqual(HttpStatusCode.BadRequest, updateHttpResponseMessage.StatusCode);
        }

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Put_Article_Should_Return_BadRequest_Response_When_Creating_Article_With_Invalid_Data(DataFormat dataFormat)
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
                dataFormat,
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

            UpdateArticle(createArticleResponse2.Id, updateArticleRequest, dataFormat, out HttpResponseMessage updateHttpResponseMessage, out string updateResponseString);
            Assert.AreEqual(HttpStatusCode.BadRequest, updateHttpResponseMessage.StatusCode);
        }

        #endregion

        #region Delete

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public async Task Delete_Article_Should_Delete(DataFormat dataFormat)
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
                dataFormat,
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

        #region Publish

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Publish_Article_Should_Publish_Article(DataFormat dataFormat)
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
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            PublishArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage publishArticleHttpResponseMessage, out string publishArticleResponseString);
            Assert.AreEqual(HttpStatusCode.NoContent, publishArticleHttpResponseMessage.StatusCode);

            GetArticle(createArticleResponse.Id,
            out HttpResponseMessage getHttpResponseMessage,
            out string getResponseString,
            out ArticleResponse getArticleResponse);

            getHttpResponseMessage.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, getHttpResponseMessage.StatusCode);
            Assert.IsTrue(getArticleResponse.IsPublished);
            Assert.IsTrue(getArticleResponse.PublishDate.HasValue);
        }

        #endregion

        #region UnPublish

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void UnPublish_Article_Should_UnPublish_Article(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = true,
                PublishDate = DateTime.UtcNow,
                Author = "God",
            };

            CreateArticle(createArticleRequest,
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            UnPublishArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage publishArticleHttpResponseMessage, out string publishArticleResponseString);
            Assert.AreEqual(HttpStatusCode.NoContent, publishArticleHttpResponseMessage.StatusCode);

            GetArticle(createArticleResponse.Id,
            out HttpResponseMessage getHttpResponseMessage,
            out string getResponseString,
            out ArticleResponse getArticleResponse);

            getHttpResponseMessage.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.OK, getHttpResponseMessage.StatusCode);
            Assert.IsTrue(getArticleResponse.IsPublished == false);
            Assert.IsTrue(getArticleResponse.PublishDate.HasValue == false);
        }

        #endregion

        #region Like

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Like_Article_Should_Create_ArticleFeedback(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = true,
                PublishDate = DateTime.UtcNow,
                Author = "God",
            };

            CreateArticle(createArticleRequest,
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            LikeArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage likeArticleHttpResponseMessage, out string likeArticleResponseString);
            Assert.AreEqual(HttpStatusCode.NoContent, likeArticleHttpResponseMessage.StatusCode);
        }

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void Like_Article_When_Called_Multiple_Times_Should_Return_BadRequest(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = true,
                PublishDate = DateTime.UtcNow,
                Author = "God",
            };

            CreateArticle(createArticleRequest,
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            for (int i = 0; i < 3; i++)
            {
                LikeArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage likeArticleHttpResponseMessage, out string likeArticleResponseString);
            }

            LikeArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage likeArticleHttpResponseMessage2, out string likeArticleResponseString2);
            Assert.AreEqual(HttpStatusCode.BadRequest, likeArticleHttpResponseMessage2.StatusCode);
        }

        #endregion

        #region UnLike

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void UnLike_Article_Should_Create_ArticleFeedback(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = true,
                PublishDate = DateTime.UtcNow,
                Author = "God",
            };

            CreateArticle(createArticleRequest,
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            UnLikeArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage unLikeArticleHttpResponseMessage, out string unLikeArticleResponseString);
            Assert.AreEqual(HttpStatusCode.NoContent, unLikeArticleHttpResponseMessage.StatusCode);
        }

        [TestMethod]
        [DataRow(DataFormat.Xml)]
        [DataRow(DataFormat.Json)]
        public void UnLike_Article_When_Called_Multiple_Times_Should_Return_BadRequest(DataFormat dataFormat)
        {
            var createArticleRequest = new CreateArticleRequest()
            {
                Title = GetUniqueStringValue("title_"),
                Body = GetUniqueStringValue("body_"),
                IsPublished = true,
                PublishDate = DateTime.UtcNow,
                Author = "God",
            };

            CreateArticle(createArticleRequest,
                dataFormat,
                out HttpResponseMessage createHttpResponseMessage,
                out string createResponseString,
                out ArticleResponse createArticleResponse);

            Assert.AreEqual(HttpStatusCode.Created, createHttpResponseMessage.StatusCode);
            Assert.AreEqual(createArticleRequest.Title, createArticleResponse.Title);
            Assert.AreEqual(createArticleRequest.Body, createArticleResponse.Body);
            Assert.AreEqual(createArticleRequest.Author, createArticleResponse.Author);
            Assert.IsTrue(createArticleResponse.Id > 1);

            for (int i = 0; i < 3; i++)
            {
                LikeArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage unLikeArticleHttpResponseMessage, out string unLikeArticleResponseString);
            }

            LikeArticle(createArticleResponse.Id, dataFormat, out HttpResponseMessage unLikeArticleHttpResponseMessage2, out string unLikeArticleResponseString2);
            Assert.AreEqual(HttpStatusCode.BadRequest, unLikeArticleHttpResponseMessage2.StatusCode);
        }

        #endregion

        #region Helpers

        private void CreateArticle(CreateArticleRequest createArticleRequest, DataFormat dataFormat,
            out HttpResponseMessage createHttpResponseMessage,
            out string createResponseString,
            out ArticleResponse createArticleResponse)
        {
            CreateHttpClient().Post("api/v1/articles",
                createArticleRequest, dataFormat,
                out createHttpResponseMessage,
                out createResponseString,
                out createArticleResponse);
        }

        private void UpdateArticle(int articleId, UpdateArticleRequest updateArticleRequest, DataFormat dataFormat,
            out HttpResponseMessage updateHttpResponseMessage,
            out string updateResponseString)
        {
            CreateHttpClient().Put($"api/v1/articles/{articleId}", updateArticleRequest, dataFormat, out updateHttpResponseMessage, out updateResponseString);
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

        private void PublishArticle(int articleId, DataFormat dataFormat, out HttpResponseMessage publishArticleHttpResponseMessage,
            out string publishArticleResponseString)
        {
            CreateHttpClient().Put<object>($"api/v1/articles/{articleId}/publish", null, dataFormat, out publishArticleHttpResponseMessage, out publishArticleResponseString);
        }

        private void UnPublishArticle(int articleId, DataFormat dataFormat, out HttpResponseMessage unPublishArticleHttpResponseMessage,
            out string unPublishArticleResponseString)
        {
            CreateHttpClient().Put<object>($"api/v1/articles/{articleId}/unpublish", null, dataFormat, out unPublishArticleHttpResponseMessage, out unPublishArticleResponseString);
        }

        private void LikeArticle(int articleId, DataFormat dataFormat, out HttpResponseMessage likeArticleHttpResponseMessage,
            out string likeArticleResponseString)
        {
            CreateHttpClient().Put<object>($"api/v1/articles/{articleId}/like", null, dataFormat, out likeArticleHttpResponseMessage, out likeArticleResponseString);
        }

        private void UnLikeArticle(int articleId, DataFormat dataFormat, out HttpResponseMessage unLikeArticleHttpResponseMessage,
           out string unLikeArticleResponseString)
        {
            CreateHttpClient().Put<object>($"api/v1/articles/{articleId}/unlike", null, dataFormat, out unLikeArticleHttpResponseMessage, out unLikeArticleResponseString);
        }

        #endregion
    }
}