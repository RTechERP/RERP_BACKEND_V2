using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PONCCHistoryController : ControllerBase
    {
        private PONCCHistoryRepo _pONCCHistoryRepo;
        private SupplierSaleRepo _supplierSaleRepo;

        public PONCCHistoryController(
            PONCCHistoryRepo pONCCHistoryRepo,
            SupplierSaleRepo supplierSaleRepo
            )
        {
            _pONCCHistoryRepo = pONCCHistoryRepo;
            _supplierSaleRepo = supplierSaleRepo;
        }

        [HttpPost("save-data")]
        [RequiresPermission("N35,N33,N1")]
        public async Task<IActionResult> SaveData([FromBody] PONCCHistoryDTO model)
        {
            try
            {
                if (model.SupplierSaleID != null && model.SupplierSaleID > 0)
                {
                    var supplierSale = _supplierSaleRepo.GetByID((int)model.SupplierSaleID);
                    if (supplierSale != null)
                    {
                        model.CodeNCC = supplierSale.CodeNCC;
                    }
                    else
                    {
                        model.CodeNCC = "";
                    }
                }
                if (model.ID > 0)
                {
                    await _pONCCHistoryRepo.UpdateAsync(model);
                }
                else
                {
                    await _pONCCHistoryRepo.CreateAsync(model);
                }
                return Ok(ApiResponseFactory.Success(null, "Đã cập nhật đặt hàng thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("deleted")]
        [RequiresPermission("N35,N33,N1")]
        public async Task<IActionResult> Deleted([FromBody] PONCCHistoryDTO model)
        {
            try
            {
                if (model.lsDeleted != null && model.lsDeleted.Count > 0)
                {
                    foreach (var id in model.lsDeleted)
                    {
                        await _pONCCHistoryRepo.DeleteAsync(id);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Đã xóa thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}