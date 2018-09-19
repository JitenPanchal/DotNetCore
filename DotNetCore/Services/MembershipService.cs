using DotNetCore.Contracts;
using DotNetCore.Database;
using DotNetCore.Database.Entities;
using DotNetCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IBlogDbContext blogDbContext;

        public MembershipService(IBlogDbContext blogDbContext) 
        {
            this.blogDbContext = blogDbContext;
        }

        public User CurrentUser => GetCurrentLoggedInUser();

        private User GetCurrentLoggedInUser(bool readOnly = true, bool throwExceptionOnEntityNotFound = false)
        {
            //// TODO 
            //var query = GetUserByNameQuery(HttpContext.Current.User.Identity.Name, readOnly);

            //var entity = query.FirstOrDefault();

            //if (throwExceptionOnEntityNotFound && entity == null)
            //    throw new EntityNotFoundException(HttpContext.Current.User.Identity.Name);

            //return entity;

            var query = blogDbContext.Set<User>();

            var entity = query.FirstOrDefault();

            return entity;
        }
    }
}
