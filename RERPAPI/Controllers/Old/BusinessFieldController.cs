using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessFieldController : ControllerBase
    {
        private BusinessFieldRepo _businessFieldRepo;
        public BusinessFieldController(BusinessFieldRepo businessFieldRepo)
        {
            _businessFieldRepo = businessFieldRepo;
        }
        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                List<BusinessField> businessFields = _businessFieldRepo.GetAll();
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
