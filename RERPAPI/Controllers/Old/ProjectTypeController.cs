using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTypeController : ControllerBase
    {
        private ProjectTypeRepo _projectTypeRepo;
        public ProjectTypeController(ProjectTypeRepo projectTypeRepo)
        {
            _projectTypeRepo = projectTypeRepo;
        }
        [HttpGet]
        public IActionResult GetAllProjectType()
        {   
            try
            {
                List<ProjectType> projectTypes = _projectTypeRepo.GetAll(); 
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
