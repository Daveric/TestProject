using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Repositories;

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
        public async Task<IActionResult> GetAppId([FromQuery]string name)
        {
            return Ok(await _applicationRepository.GetGuidByApplicationName(name));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAccess([FromQuery] string userName)
        {
            return Ok(await _applicationRepository.GetApplicationAccessToUserAsync(userName));
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_applicationRepository.GetAll());
        }

    }
}
