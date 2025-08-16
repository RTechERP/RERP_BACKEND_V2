using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressStockController : ControllerBase
    {
        AddressStockRepo _addressStockRepo = new AddressStockRepo();
        [HttpGet("get-by-customerID")]
        public IActionResult getDataCbbAddressStock(int customerID) {
            try
            {
                List<AddressStock> result = _addressStockRepo.GetAll().Where(x=>x.CustomerID==customerID).ToList();
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    ex.Message
                });
            }
        }
    }
}
