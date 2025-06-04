using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessFieldController : ControllerBase
    {
        BusinessFieldRepo businessFieldRepo = new BusinessFieldRepo();
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<BusinessField> businessFields = businessFieldRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = businessFields
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
