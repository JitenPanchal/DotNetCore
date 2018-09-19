using AutoMapper;
using DotNetCore.Database.Entities;
using DotNetCore.Models.Request;
using DotNetCore.Models.Response;

namespace DotNetCore.Infrastructure.AutoMapperProfiles
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<Article, ArticleResponse>();
            CreateMap<CreateArticleRequest, Article>();
        }
    }
}
