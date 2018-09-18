using DotNetCore.Contracts;
using DotNetCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Services
{
    public class ApplicationSettingsService : BaseService, IApplicationSettingsService
    {
        private const int pageSize = 10;
        private const int maxArticleFeedbackAttempts = 3;

        public ApplicationSettingsService(IDbContext dbContext)
        {
        }

        public int PageSize { get => pageSize; }

        public int MaxArticleFeedbackAttempts { get => maxArticleFeedbackAttempts; }
    }
}