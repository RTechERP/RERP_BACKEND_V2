
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierSaleController : ControllerBase
    {
        SupplierSaleRepo _supplierSaleRepo=new SupplierSaleRepo();
        [HttpGet("")]
        public IActionResult getSupplierSale()
        {

            try
            {
                List<SupplierSale> result = _supplierSaleRepo.GetAll();
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        
    }
}
