
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Data;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierSaleController : ControllerBase
    {
        SupplierSaleRepo _supplierSaleRepo = new SupplierSaleRepo();
        #region Lấy tất cả nhà cung cấp
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                List<SupplierSale> result = _supplierSaleRepo.GetAll().OrderBy(x => x.ID).ToList();
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
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        #endregion
    }
}