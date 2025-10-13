
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;

namespace RERPAPI.Controllers.Old.SaleWareHouseManagement
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
