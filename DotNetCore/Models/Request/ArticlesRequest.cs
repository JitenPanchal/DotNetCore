using DotNetCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Models.Request
{
    public class ArticlesRequest : PagingRequest
    {
        public ArticlesSortBy SortBy { get; set; }
    }
}
