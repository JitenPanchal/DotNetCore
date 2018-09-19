using AutoMapper;
using AutoMapper.QueryableExtensions;
using DotNetCore.Contracts;
using DotNetCore.Database;
using DotNetCore.Models;
using DotNetCore.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IMapper mapper;
        private readonly IEntityService entityService;

        public BaseController(IMapper mapper, IEntityService entityService)
        {
            this.mapper = mapper;
            this.entityService = entityService;
        }

        protected async Task<IActionResult> Get<TEntity, TMapTo>(int id) where TEntity : BaseEntity where TMapTo : class
        {
            return Ok(mapper.Map<TMapTo>(await entityService.GetByIdAsync<TEntity>(id, true, true)));
        }

        protected IActionResult Post<TRequestModel, TEntity>(TRequestModel requestModel) where TEntity : BaseEntity where TRequestModel : class
        {
            var entity = mapper.Map<TEntity>(requestModel);

            entityService.Create<TEntity>(entity);

            return Ok(mapper.Map<ArticleResponse>(entity));
        }

        protected async Task<IActionResult> Put<TRequestModel, TEntity>(int id, TRequestModel requestModel) where TEntity : BaseEntity where TRequestModel : class
        {
            var entity = await entityService.GetByIdAsync<TEntity>(id, false, true);

            mapper.Map(requestModel, entity);

            entityService.Update(entity, true);

            return NoContent();
        }

        protected async Task<IActionResult> Delete<TEntity>(int id) where TEntity : BaseEntity
        {
            var entity = entityService.GetById<TEntity>(id, false, true);

            entityService.Delete(entity,true);

            return Ok(entity);
        }
    }
}
