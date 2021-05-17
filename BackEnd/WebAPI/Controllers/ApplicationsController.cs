using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Entities;
using WebAPI.Data.Repositories;
using WebAPI.Helper;

namespace WebAPI.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserHelper _userHelper;

        public ApplicationsController(IApplicationRepository applicationRepository, IUserHelper userHelper)
        {
            _applicationRepository = applicationRepository;
            _userHelper = userHelper;
        }

        // GET: Applications
        public IActionResult Index()
        {
            return View( _applicationRepository.GetAll().OrderBy(a=>a.Name));
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ApplicationNotFound");
            }

            var application = await _applicationRepository.GetByIdAsync(id.Value);
            if (application == null)
            {
                return new NotFoundViewResult("ApplicationNotFound");
            }

            return View(application);
        }

        // GET: Applications/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AppId")] Application application)
        {
            if (ModelState.IsValid)
            {
                application.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                await _applicationRepository.CreateAsync(application);
                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }

        // GET: Applications/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ApplicationNotFound");
            }

            var application = await _applicationRepository.GetByIdAsync(id.Value);
            if (application == null)
            {
                return new NotFoundViewResult("ApplicationNotFound");
            }
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AppId")] Application application)
        {
            if (id != application.Id)
            {
                return new NotFoundViewResult("ApplicationNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    application.User = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                    await _applicationRepository.UpdateAsync(application);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _applicationRepository.ExistAsync(application.Id))
                    {
                        return new NotFoundViewResult("ApplicationNotFound");
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }

        // GET: Applications/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ApplicationNotFound");
            }

            var application = await _applicationRepository.GetByIdAsync(id.Value);
            if (application == null)
            {
                return new NotFoundViewResult("ApplicationNotFound");
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            await _applicationRepository.DeleteAsync(application);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult ApplicationNotFound()
        {
            return View();
        }
    }
}
