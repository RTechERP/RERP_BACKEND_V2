using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupSaleController : ControllerBase
    {
        GroupSaleRepo groupSaleRepo = new GroupSaleRepo();
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<GroupSale> groupSales = groupSaleRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = groupSales
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
