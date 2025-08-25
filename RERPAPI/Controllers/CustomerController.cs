using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        CustomerRepo _customerRepo = new CustomerRepo();
        [HttpGet("get-all")]
        public IActionResult GetCustomers()
        {
            List<Customer> customers = _customerRepo.GetAll();
            return Ok(new
            {
                data = customers,
                status = 1
            });

        }
    }
}
