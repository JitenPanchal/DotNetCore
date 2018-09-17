using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Database.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime? PublishDate { get; set; }
        public int AddedByUserId { get; set; }
        public bool IsPublished { get; set; }
        public List<ArticleFeedback> ArticleFeedbacks { get; set; }
    }
}