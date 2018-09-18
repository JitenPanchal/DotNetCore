using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Models
{
    public class ArticleMostLiked
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime? PublishDate { get; set; }
        public int AddedByUserId { get; set; }
        public string AddedByUserName { get; set; }
        public bool IsPublished { get; set; }
        public int Count { get; set; }
    }
}
