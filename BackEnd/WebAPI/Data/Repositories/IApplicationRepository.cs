
namespace WebAPI.Data.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Entities;

    public interface IApplicationRepository : IGenericRepository<Application>
    {
        Task<bool>  GetApplicationAccessToUserAsync(string name);

        Task<Guid> GetGuidByApplicationName(string name);
        
    }
}
