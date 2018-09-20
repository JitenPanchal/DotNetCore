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
    public abstract class BaseController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IEntityService entityService;

        public BaseController(IMapper mapper, IEntityService entityService)
        {
            this.mapper = mapper;
            this.entityService = entityService;
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

            await entityService.DeleteAsync(entity);

            return NoContent();
        }
    }
}
