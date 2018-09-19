using System;
using System.Runtime.Serialization;

namespace DotNetCore.Models.Response
{
    [DataContract]
    public class ArticleMostLiked
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
        public int AddedByUserId { get; set; }
        [DataMember]
        public string AddedByUserName { get; set; }
        [DataMember]
        public bool IsPublished { get; set; }
        [DataMember]
        public int Count { get; set; }
    }
}