using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data.Repositories;
using WebAPI.Helper;

namespace WebAPI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationsController(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        [HttpGet]
        public IActionResult GetApplications(Guid id)
        {
            return Ok(_applicationRepository.GetAll().First(app => app.AppId == id));
        }
    }
}
