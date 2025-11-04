using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using System.Security.Cryptography;

namespace RERPAPI.Controllers.Old.OfficeSuppliesManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSupplyUnitController : ControllerBase
    {
        OfficeSupplyUnitRepo _officesupplyunitRepo = new OfficeSupplyUnitRepo();

        [HttpGet("get-office-suplly-unit")]
        public IActionResult getOfficeSupplyUnit()
        {
            try
            {
                List<OfficeSupplyUnit> data = _officesupplyunitRepo.GetAll().Where(x => x.IsDeleted !=true).OrderByDescending(x => x.CreatedDate).ToList(); 
                return Ok(new
                {
                    status = 1,
                    data
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

        [HttpGet("get-supply-unit-by-id")]
        public IActionResult getOfficeSupplyUnitByID(int id)
        {
            try
            {
                OfficeSupplyUnit dst = _officesupplyunitRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = dst
                });
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

                if (dst != null && dst.IsDeleted != true)
                {
                    if (!_officesupplyunitRepo.Validate(dst, out string message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (dst.ID <= 0)
                {
                    //dst.IsDeleted = false;
                    await _officesupplyunitRepo.CreateAsync(dst);
                }
                else
                {
                    _officesupplyunitRepo.Update(dst);
                }
                return Ok(new
                {
                    status = 1,
                    data = dst
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("delete-office-supply-unit")]
        //public async Task<IActionResult> deleteOfficeSupplyUnit([FromBody] List<int> ids)
        //{
        //    try
        //    {
        //        if (ids == null || ids.Count == 0)
        //            return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });

        //        foreach (var id in ids)
        //        {
        //            var item = _officesupplyunitRepo.GetByID(id);
        //            if (item != null)
        //            {
        //                item.IsDeleted = true; // Gán trường IsDeleted thành true
        //                /* await off.UpdateAsync(item);*/
        //                _officesupplyunitRepo.Update(item);/* // Cập nhật lại mục*/
        //            }
        //        }
        //        return Ok(new
        //        {
        //            status = 1,
        //            message = "Đã xóa thành công"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }

        //}
    }
}
