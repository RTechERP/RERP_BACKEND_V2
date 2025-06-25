using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        ProjectRepo projectRepo = new ProjectRepo();
        [HttpGet("get-by-id")]
        public IActionResult getByID(int id)
        {
            Project p = projectRepo.GetByID(id);
            return Ok(new
            {
                status = 1,
                data = p
            });
        }
    }
}
