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
                var dataUnit = _unitcountRepo.GetAll();
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
                    if (dto.ID <= 0) await _unitcountRepo.CreateAsync(dto);
                    else await _unitcountRepo.UpdateAsync(dto);
                }
                return Ok(ApiResponseFactory.Success(null, "Xử lý dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
