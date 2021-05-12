
using System;
using WebAPI.Helper;

namespace WebAPI.Data.Repositories
{
    using Entities;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.Linq;

    public interface IApplicationRepository : IGenericRepository<Application>
    {
        bool GetApplicationAccessToUser(string name);

        Guid GetGuidByApplicationName(string name);

    }
}
