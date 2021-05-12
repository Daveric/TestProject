

using System;
using System.Threading.Tasks;

namespace WebAPI.Data.Repositories
{
    using System.Linq;
    using Entities;

    public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
    {
        private readonly DataContext _context;

        public ApplicationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public bool GetApplicationAccessToUser(string name)
        {
            var userIsAdmin = _context.Applications?.AsQueryable().FirstOrDefault(a => a.Name == name)?.User.IsAdmin;
            return userIsAdmin != null && (bool) userIsAdmin;
        }


        public Guid GetGuidByApplicationName(string name)
        {
            var appId = _context.Applications?.AsQueryable().FirstOrDefault(a => a.Name == name)?.AppId;
            if (appId != null)
                return (Guid) appId;

            return default;
        }

    }
}
