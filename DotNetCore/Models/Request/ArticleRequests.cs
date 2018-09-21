using DotNetCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Models.Request
{
    public class GetArticlesRequest : PagingRequest
    {
        public ArticlesSortBy SortBy { get; set; }
    }

    public class CreateArticleRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string Author { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsPublished { get; set; }
    }

    public class UpdateArticleRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string Author { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsPublished { get; set; }
    }
}
