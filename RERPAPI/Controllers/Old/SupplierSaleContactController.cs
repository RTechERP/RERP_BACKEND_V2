using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierSaleContactController : ControllerBase
    {
        private SupplierSaleContactRepo _supplierSaleContactRepo;

        public SupplierSaleContactController(
            SupplierSaleContactRepo supplierSaleContactRepo
        )
        {
            _supplierSaleContactRepo = supplierSaleContactRepo;
        }

        [HttpGet("supplier-sale-contact")]
        public async Task<IActionResult> getSupplierSaleContact(int supplierID)
        {
            try
            {
                var data = _supplierSaleContactRepo.GetAll()
                    .Where(c => c.SupplierID == supplierID)
                    .OrderByDescending(c => c.ID);

                return Ok(ApiResponseFactory.Success(data, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("supplier-sale-contact")]
        [RequiresPermission("N27,N33,N35,N1")]
        public async Task<IActionResult> savesuppliersalecontact([FromBody] SupplierSaleContact supplierSaleContact)
        {
            try
            {
                if (supplierSaleContact.ID <= 0)
                {
                    await _supplierSaleContactRepo.CreateAsync(supplierSaleContact);
                }
                else
                {
                    _supplierSaleContactRepo.Update(supplierSaleContact);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}