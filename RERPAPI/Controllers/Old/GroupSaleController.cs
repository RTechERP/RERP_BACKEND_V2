using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupSaleController : ControllerBase
    {
        private readonly GroupSaleRepo _groupSaleRepo;

        public GroupSaleController(GroupSaleRepo groupSaleRepo)
        {
            _groupSaleRepo = groupSaleRepo;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<GroupSale> groupSales = _groupSaleRepo.GetAll();
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
