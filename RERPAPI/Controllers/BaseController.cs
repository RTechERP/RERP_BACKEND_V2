using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected readonly IConfiguration Configuration;

        public BaseController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}