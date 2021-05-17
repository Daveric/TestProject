using System;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAll()
        {
            return Ok(_applicationRepository.GetAll().Include(app => app.User));
        }

        [HttpPost]
        [Route("SetUserToApp")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostUserToCurrentApp([FromBody] SetUserToAppRequest request)
        {
            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }

            var app = await _applicationRepository.GetAll().FirstOrDefaultAsync(a => a.AppId == Guid.Parse(request.AppId));
            app.User = user;
            if (await _applicationRepository.UpdateAsync(app) == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }
            return Ok(new Response
            {
                IsSuccess = true,
                Message = "The user now is set to the current application"
            });
        }

    }
}
