using DotNetCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Database.Entities
{
    public class ArticleFeedback : BaseEntity
    {
        public int ArticleId { get; set; }
        public string Comments { get; set; }
        public ArticleStatus Status { get; set; }
        public int UserId { get; set; }
        public DateTime? FeedbackDate { get; set; }
        public DateTime? CommentDate { get; set; }
        public int FeedbackCount { get; set; }

        public Article Article { get; set; }
    }
}
