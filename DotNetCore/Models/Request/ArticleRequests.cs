using DotNetCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DotNetCore.Models.Request
{
    public class GetArticlesRequest : PagingRequest
    {
        public ArticlesSortBy SortBy { get; set; }
    }

    [DataContract]
    public class CreateArticleRequest
    {
        [Required]
        [DataMember]
        public string Title { get; set; }
        [Required]
        [DataMember]
        public string Body { get; set; }
        [Required]
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public DateTime? PublishDate { get; set; }
        [DataMember]
        public bool IsPublished { get; set; }
    }

    [DataContract]
    public class UpdateArticleRequest
    {
        [Required]
        [DataMember]
        public string Title { get; set; }
        [Required]
        [DataMember]
        public string Body { get; set; }
        [Required]
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public DateTime? PublishDate { get; set; }
        [DataMember]
        public bool IsPublished { get; set; }
    }
}
