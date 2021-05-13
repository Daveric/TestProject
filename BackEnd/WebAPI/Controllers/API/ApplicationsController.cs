using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Repositories;
using WebAPI.Helper;

namespace WebAPI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserHelper _userHelper;

        public ApplicationsController(IApplicationRepository applicationRepository, IUserHelper userHelper)
        {
            _applicationRepository = applicationRepository;
            _userHelper = userHelper;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAppId([FromQuery]string name)
        {
            return Ok(await _applicationRepository.GetGuidByApplicationName(name));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAccess([FromQuery] string name)
        {
            return Ok(await _applicationRepository.GetApplicationAccessAsync(name));
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_applicationRepository.GetAll());
        }

        [HttpPost("set-user-app")]
        public async Task<IActionResult> SetUserToCurrentApp([FromQuery] string username, [FromQuery]string guid)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var app = await _applicationRepository.GetAll().FirstOrDefaultAsync(a => a.AppId == Guid.Parse(guid));
            app.User = user;
            if (await _applicationRepository.UpdateAsync(app) == null)
            {
                return BadRequest("Cannot update application.");
            }
            return Ok();
        }

    }
}
