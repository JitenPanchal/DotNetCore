using AutoMapper;
using AutoMapper.QueryableExtensions;
using DotNetCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedList<TEntity>> ToPagedList<TEntity>(this IQueryable<TEntity> source, PagingRequest pagingRequest) where TEntity : class
        {
            var totalCount = await source.CountAsync();

            var totalPages = totalCount / pagingRequest.PageSize;

            if (totalCount % pagingRequest.PageSize > 0)
                totalPages++;

            var data = await source.Skip((pagingRequest.PageNumber - 1) * pagingRequest.PageSize).Take(pagingRequest.PageSize).ToListAsync();

            return new PagedList<TEntity>(data, pagingRequest.PageNumber, pagingRequest.PageSize, totalCount);
        }

        public static async Task<PagedList<TMapTo>> ToPagedList<TEntity, TMapTo>(this IQueryable<TEntity> source, IMapper mapper, PagingRequest pagingRequest) where TEntity : class where TMapTo : class
        {
            var query = source.ProjectTo<TMapTo>(mapper.ConfigurationProvider);

            return await ToPagedList<TMapTo>(query, pagingRequest);
        }
    }
}
