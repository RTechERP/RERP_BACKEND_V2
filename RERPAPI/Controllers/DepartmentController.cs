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
        DepartmentRepo departmentRepo = new DepartmentRepo();

        [HttpGet("getall")]
        public IActionResult GetAll()
        {

            try
            {
                List<Department> departments = departmentRepo.GetAll().OrderBy(x => x.STT).ToList();

                return Ok(new
                {
                    status = 1,
                    data = departments
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }

        }

    }
}
