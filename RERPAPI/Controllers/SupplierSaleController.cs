
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;


namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierSaleController : ControllerBase
    {
        SupplierSaleRepo _supplierSaleRepo = new SupplierSaleRepo();
        [HttpGet("get-ncc")]
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
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
