﻿using DotNetCore.Enums;
using System;
using System.Collections.Generic;
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
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsPublished { get; set; }
    }

    public class UpdateArticleRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsPublished { get; set; }
    }

}
