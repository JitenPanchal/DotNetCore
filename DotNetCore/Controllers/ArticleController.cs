using DotNetCore.Contracts;
using DotNetCore.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.Database.Entities;
using DotNetCore.Models.Response;
using DotNetCore.Infrastructure.Filters;
using DotNetCore.Database;

namespace DotNetCore.Controllers
{
    [Route("api/v1/articles")]
    public class ArticleController : Controller
    {
        private readonly IArticleService articleService;
        private readonly IMapper mapper;
        private readonly IBlogDbContext blogDbContext;

        public ArticleController(IMapper mapper, IArticleService articleService, IBlogDbContext blogDbContext)
        {
            this.articleService = articleService;
            this.mapper = mapper;
            this.blogDbContext = blogDbContext;
        }

        [HttpGet()]
        public async Task<IActionResult> GetArticles([FromQuery] ArticlesRequest articlesRequest)
        {
            var query = articleService.GetArticles(articlesRequest.SortBy).Where(it => it.IsPublished);

            var data = await query.ToPagedList<Article, ArticleResponse>(mapper, articlesRequest);

            return Ok(data);
        }

        [HttpGet("{id}",Name = nameof(GetArticle))]
        public async Task<IActionResult> GetArticle([FromRoute] int id)
        {
            var article = await blogDbContext.GetByIdAsync<Article>(id, readOnly: true, throwExceptionOnEntityNotFound: true);

            return Ok(article);
        }

        [HttpPost]
        [Securable("CreateArticle", "Create Article")]
        public IActionResult Post([FromBody]CreateArticleRequest createArticleRequest)
        {
            var article = mapper.Map<Article>(createArticleRequest);
            blogDbContext.Create(article, true);
            return CreatedAtRoute(nameof(GetArticle), new { id = article.Id });
        }
    }

    [DataContract]
    public class SomeData
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
