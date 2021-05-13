
namespace WebAPI.Data.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Entities;

    public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
    {
        private readonly DataContext _context;

        public ApplicationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> GetApplicationAccessToUserAsync(string userName)
        {
            if (_context.Users == null) return false;
            var user = await _context.Users?.AsQueryable().FirstOrDefaultAsync(u => u.Email == userName);
            var userHasAccess = user?.HasAccess;
            return userHasAccess != null && (bool)userHasAccess;
        }


        public async Task<Guid> GetGuidByApplicationName(string name)
        {
            if (_context.Applications == null) return default;
            var appId = await _context.Applications?.AsQueryable().FirstOrDefaultAsync(a => a.Name == name);
            return appId?.AppId ?? default;
        }
        
    }
}
