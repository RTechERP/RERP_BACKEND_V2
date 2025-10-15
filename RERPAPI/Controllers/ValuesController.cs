using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Context;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        RTCContext context;

        public ValuesController(RTCContext context)
        {
            this.context = context;
        }

        [HttpGet("employee")]
        public IActionResult GetAllEmployee()
        {
            try
            {
                return Ok(new { status = 1, data = context.Employees.ToList() });
            }
            catch (Exception)
            {

                throw new Exception();
            }
        }
    }
}
