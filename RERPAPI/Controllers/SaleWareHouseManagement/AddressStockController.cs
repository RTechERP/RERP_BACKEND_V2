using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.SaleWareHouseManagement
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
                List<AddressStock> result = _addressStockRepo.GetAll().Where(x=>x.CustomerID==customerID).ToList();
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu vị trí thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
