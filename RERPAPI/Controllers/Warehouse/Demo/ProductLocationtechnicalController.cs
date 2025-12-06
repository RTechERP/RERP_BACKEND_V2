using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Warehouse.Demo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductLocationtechnicalController : ControllerBase
    {
        private readonly ProductLocationRepo _productLocationRepo;

        public ProductLocationtechnicalController(ProductLocationRepo productLocationRepo)
        {
            _productLocationRepo = productLocationRepo;
        }

        [HttpGet("get-all")]
        [RequiresPermission("N26")]
        public IActionResult GetAll(int warehouseID)
        {
            try
            {
                var dt = SQLHelper<dynamic>.ProcedureToList("spGetProductLocation", new string[] { "@WarehouseID" }, new object[] { warehouseID });
                var data = SQLHelper<dynamic>.GetListData(dt, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-stt")]
        public IActionResult GetSTT(int warehouseID)
        {
            int stt = _productLocationRepo.GetSTT(warehouseID);
            return Ok(ApiResponseFactory.Success(stt, ""));
        }
        [HttpPost("save-data")]
        [RequiresPermission("N26,N1,N34,N80")]
        public async Task<IActionResult> SaveData(ProductLocation p)
        {
            try
            {
                string message = string.Empty;
                if (!_productLocationRepo.Validate(p, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (p.ID > 0) await _productLocationRepo.UpdateAsync(p);
                else
                {
                    p.STT = _productLocationRepo.GetSTT(p.WarehouseID ?? 1);
                    await _productLocationRepo.CreateAsync(p);
                }
                return Ok(ApiResponseFactory.Success(p, "cập nhật dữ liệu thành công!"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _productLocationRepo.GetByIDAsync(id);
                if (data == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-data")]
        [RequiresPermission("N26,N1,N34,N80")]
        public async Task<IActionResult> DeleteData([FromBody] List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var item = await _productLocationRepo.GetByIDAsync(id);
                    if (item == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));
                    }

                    bool isInUse = _productLocationRepo.CheckLocationInUse(id);
                    if (isInUse)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể xoá vị trí này. Vì đã được sử dụng!"));
                    }

                    item.IsDeleted = true;
                    await _productLocationRepo.UpdateAsync(item);
                }

                return Ok(ApiResponseFactory.Success(ids, "Xóa dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
