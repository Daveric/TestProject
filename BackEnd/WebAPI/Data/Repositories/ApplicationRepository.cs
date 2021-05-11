

namespace WebAPI.Data.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
    {
        private readonly DataContext _context;

        public ApplicationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Applications.Include(app => app.User);
        }

        public IEnumerable<SelectListItem> GetComboApplications()
        {
            var list = _context.Applications.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an application...)",
                Value = "0"
            });

            return list;
        }

    }
}
