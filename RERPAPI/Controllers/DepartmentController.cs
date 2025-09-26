using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        DepartmentRepo _departmentRepo = new DepartmentRepo();

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<Department> departments = _departmentRepo.GetAll().OrderBy(x => x.STT).ToList();

                return Ok(ApiResponseFactory.Success(departments, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

    }
}
