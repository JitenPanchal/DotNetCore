using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Contracts
{
    public interface IApplicationSettingsService
    {
        int PageSize { get; }

        int MaxArticleFeedbackAttempts { get; }
    }
}
