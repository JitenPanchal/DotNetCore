using System;
using System.Runtime.Serialization;

namespace DotNetCore.Models.Response
{
    [DataContract]
    public class ArticleStatResponse
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public DateTime? PublishDate { get; set; }
        [DataMember]
        public bool IsPublished { get; set; }
        [DataMember]
        public long? LikeCount { get; set; }
        [DataMember]
        public long? UnLikeCount { get; set; }
        [DataMember]
        public long? NoneCount { get; set; }
    }
}
