
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

        public async Task<User> GetUserByApplicationName(string name)
        {
            if (_context.Applications == null) return null;
            var app = await _context.Applications.Include(a=>a.User).FirstOrDefaultAsync(a => a.Name == name);
            return app.User;
        }

        public async Task<Guid> GetGuidByApplicationName(string name)
        {
            if (_context.Applications == null) return default;
            var appId = await _context.Applications?.AsQueryable().FirstOrDefaultAsync(a => a.Name == name);
            return appId?.AppId ?? default;
        }

    }
}
