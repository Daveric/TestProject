#nullable enable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using MvvmCross.Binding.Extensions;
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

        [HttpGet("[action]")]
        public IActionResult GetAppId([FromQuery]string name)
        {
            return Ok(_applicationRepository.GetGuidByApplicationName(name));
        }

        [HttpGet("[action]")]
        public IActionResult GetAccess([FromQuery] string name)
        {
            return Ok(_applicationRepository.GetApplicationAccessToUser(name));
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_applicationRepository.GetAll());
        }

    }
}
