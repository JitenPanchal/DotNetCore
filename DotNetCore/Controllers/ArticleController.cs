using AutoMapper;
using DotNetCore.Contracts;
using DotNetCore.Database.Entities;
using DotNetCore.Enums;
using DotNetCore.Infrastructure.Filters;
using DotNetCore.Models.Request;
using DotNetCore.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Controllers
{
    [Route("api/v1/articles")]
    //[ApiController]
    public class ArticleController : BaseController
    {
        private readonly IArticleService articleService;
        private readonly IMapper mapper;

        public ArticleController(IMapper mapper, IArticleService articleService) : base(mapper, articleService)
        {
            this.articleService = articleService;
            this.mapper = mapper;
        }

        [HttpGet()]
        public async Task<IActionResult> GetArticles([FromQuery] GetArticlesRequest articlesRequest)
        {
            var query = articleService.GetArticles(articlesRequest.SortBy).Where(it => it.IsPublished);
            var data = await query.ToPagedList<Article, ArticleResponse>(mapper, articlesRequest);
            return Ok(data);
        }

        [HttpGet("{id:int}", Name = nameof(GetArticle))]
        public async Task<IActionResult> GetArticle([FromRoute] int id)
        {
            var article = await articleService.GetByIdAsync<Article>(id, readOnly: true, throwExceptionOnEntityNotFound: true);
            return Ok(article);
        }

        [HttpPost]
        [Securable("CreateArticle", "Create Article")]
        public async Task<IActionResult> Post([FromBody]CreateArticleRequest createArticleRequest)
        {
            var article = mapper.Map<Article>(createArticleRequest);
            await articleService.CreateAsync(article);
            return CreatedAtRoute(nameof(GetArticle), new { id = article.Id }, article);
        }
        
        [HttpPut]
        [Route("{id:int}")]
        [Securable("UpdateArticle", "Update Article")]
        public async Task<IActionResult> Put(int id, [FromBody]UpdateArticleRequest updateArticleRequest)
        {
            return await Put<UpdateArticleRequest, Article>(id, updateArticleRequest);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Securable("DeleteArticle", "Delete Article")]
        public async Task<IActionResult> Delete(int id)
        {
            return await Delete<Article>(id);
        }

        [HttpPatch]
        [Route("{id:int}/publish")]
        [Securable("PublishArticle", "Publish Article")]
        public IActionResult PublishArticle(int id)
        {
            articleService.PublishArticle(id);
            return NoContent();
        }

        [HttpPatch]
        [Route("{id:int}/unpublish")]
        [Securable("HideArticle", "Hide Article")]
        public IActionResult UnPublishArticle(int id)
        {
            articleService.UnPublishArticle(id);
            return NoContent();
        }
     }
}