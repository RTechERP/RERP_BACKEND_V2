using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Security.Cryptography;

namespace RERPAPI.Controllers.OfficeSuppliesManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSupplyUnitController : ControllerBase
    {
        OfficeSupplyUnitRepo _officesupplyunitRepo = new OfficeSupplyUnitRepo();

        [HttpGet("")]
        public IActionResult getOfficeSupplyUnit()
        {
            try
            {
                List<OfficeSupplyUnit> data = _officesupplyunitRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu đơn vị tính thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public IActionResult getOfficeSupplyUnitByID(int id)
        {
            try
            {
                OfficeSupplyUnit dst = _officesupplyunitRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(dst, "Phê duyệt thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //danh sách tính
        [HttpPost("save-data")]
        public async Task<IActionResult> saveDataOfficeSupplyUnit([FromBody] OfficeSupplyUnit dst)
        {
            try
            {
                if (dst.ID <= 0)
                {
                    dst.IsDeleted = false;
                    await _officesupplyunitRepo.CreateAsync(dst);
                }
                else
                {
                    _officesupplyunitRepo.Update(dst);
                }
                return Ok(ApiResponseFactory.Success(dst , "Lưu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-office-supply-unit")]
        public async Task<IActionResult> deleteOfficeSupplyUnit([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });

                foreach (var id in ids)
                {
                    var item = _officesupplyunitRepo.GetByID(id);
                    if (item != null)
                    {
                        item.IsDeleted = true; // Gán trường IsDeleted thành true
                        /* await off.UpdateAsync(item);*/
                        _officesupplyunitRepo.Update(item);/* // Cập nhật lại mục*/
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
    }
}
