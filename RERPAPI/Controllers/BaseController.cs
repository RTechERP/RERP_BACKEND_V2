using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;

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
