using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressStockController : ControllerBase
    {
        private readonly AddressStockRepo _addressStockRepo;

        public AddressStockController(AddressStockRepo addressStockRepo)
        {
            _addressStockRepo = addressStockRepo;
        }
        [HttpGet("get-by-customerID")]
        public IActionResult getDataCbbAddressStock(int customerID) {
            try
            {
                List<AddressStock> result = _addressStockRepo.GetAll(x=>x.CustomerID==customerID);
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
