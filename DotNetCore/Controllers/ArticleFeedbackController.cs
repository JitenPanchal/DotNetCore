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
    [Route("api/v1/articles/{articleId}")]
    //[ApiController]
    public class ArticleFeedbackController : BaseController
    {
        private readonly IArticleService articleService;
        private readonly IMapper mapper;

        public ArticleFeedbackController(IMapper mapper, IArticleService articleService) : base(mapper, articleService)
        {
            this.articleService = articleService;
            this.mapper = mapper;
        }

        [HttpPatch]
        [Route("like")]
        public IActionResult LikeArticle(int articleId)
        {
            articleService.SaveArticleFeedback(articleId, ArticleStatus.Like);
            return NoContent();
        }

        [HttpPatch]
        [Route("unlike")]
        public IActionResult UnLikeArticle(int articleId)
        {
            articleService.SaveArticleFeedback(articleId, ArticleStatus.UnLike);
            return NoContent();
        }
    }
}