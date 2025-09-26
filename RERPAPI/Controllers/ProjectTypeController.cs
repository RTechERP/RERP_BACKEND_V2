using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTypeController : ControllerBase
    {
        ProjectTypeRepo projectTypeRepo = new ProjectTypeRepo();
        [HttpGet]
        public IActionResult GetAllProjectType()
        {   
            try
            {
                List<ProjectType> projectTypes = projectTypeRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = projectTypes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
