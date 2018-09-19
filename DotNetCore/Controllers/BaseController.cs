using DotNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Controllers
{
    public abstract class BaseController : Controller
    {
        protected async Task<PagedList<T>> ToPagedList<T>(IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            var totalCount = await source.CountAsync();

            var totalPages = totalCount / pageSize;

            if (totalCount % pageSize > 0)
                totalPages++;

            var data = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(data, pageNumber, pageSize, totalCount);
        }
    }
}
