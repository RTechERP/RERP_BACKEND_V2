using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ZXing;

namespace RERPAPI.Controllers.SaleWareHouseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitCountController : ControllerBase
    {
        UnitCountRepo _unitcountRepo = new UnitCountRepo();

        [HttpGet("")]
        public IActionResult getUnitCount()
        {
            try
            {
                var dataUnit = _unitcountRepo.GetAll()
                    .Where(x => x.IsDeleted != true)
                    .OrderBy(x => x.UnitCode)
                    .ToList();
                return Ok(ApiResponseFactory.Success(dataUnit, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> saveUnitCount([FromBody] List<UnitCountDTO> dtos)
        {
            try
            {
                foreach (var dto in dtos)
                {
                    // Validate UnitCode uniqueness
                    if (!string.IsNullOrWhiteSpace(dto.UnitCode))
                    {
                        var existingUnit = _unitcountRepo.GetAll()
                            .FirstOrDefault(x => x.UnitCode.ToUpper().Trim() == dto.UnitCode.ToUpper().Trim() 
                                                && x.ID != dto.ID && x.IsDeleted != true);
                        if (existingUnit != null)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Mã đơn vị tính '{dto.UnitCode}' đã tồn tại!"));
                        }
                    }

                    if (dto.ID <= 0)
                    {
                        dto.CreatedDate = DateTime.Now;
                        dto.IsDeleted = false;
                        await _unitcountRepo.CreateAsync(dto);
                    }
                    else
                    {
                        dto.UpdatedDate = DateTime.Now;
                        await _unitcountRepo.UpdateAsync(dto);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> deleteUnitCount([FromBody] List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var unit = _unitcountRepo.GetByID(id);
                    if (unit != null)
                    {
                        unit.IsDeleted = true;
                        unit.UpdatedDate = DateTime.Now;
                        await _unitcountRepo.UpdateAsync(unit);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("validate-unitcode")]
        public IActionResult ValidateUnitCode(string unitCode, int id = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(unitCode))
                {
                    return Ok(ApiResponseFactory.Success(new { isValid = false }, "Mã đơn vị tính không được để trống!"));
                }

                var existingUnit = _unitcountRepo.GetAll()
                    .FirstOrDefault(x => x.UnitCode.ToUpper().Trim() == unitCode.ToUpper().Trim() 
                                        && x.ID != id && x.IsDeleted != true);

                bool isValid = existingUnit == null;
                string message = isValid ? "Mã đơn vị tính hợp lệ!" : "Mã đơn vị tính đã tồn tại!";

                return Ok(ApiResponseFactory.Success(new { isValid }, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
