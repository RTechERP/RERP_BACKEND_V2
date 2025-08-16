using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        CustomerRepo _customerRepo = new CustomerRepo();
   

        [HttpGet("get-customers")]
        public IActionResult GetAll()
        {
            try
            {
                List<Customer> customers = _customerRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = customers
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
