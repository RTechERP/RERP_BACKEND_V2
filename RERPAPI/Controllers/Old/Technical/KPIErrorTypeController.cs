using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Xml.XPath;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIErrorTypeController : ControllerBase
    {
        private readonly KPIErrorTypeRepo _kpiErrorTypeRepo;
        public KPIErrorTypeController(KPIErrorTypeRepo kpiErrorTypeRepo)
        {
            _kpiErrorTypeRepo = kpiErrorTypeRepo;
        }

        [HttpGet("get-kpi-error-type")]
        public IActionResult GetKPIErrorType()
        {
            try
            {
                var data = _kpiErrorTypeRepo.GetAll(x => x.IsDelete != true).OrderBy(p => p.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-error-type-by-id")]
        public IActionResult GetKPIError(int id)
        {
            try
            {
                var data = _kpiErrorTypeRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-kpi-error-type")]
        public IActionResult DeleteKPIErrorType(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));
                }
                var data = _kpiErrorTypeRepo.GetByID(id);
                data.IsDelete = true;
                _kpiErrorTypeRepo.Update(data);
                return Ok(ApiResponseFactory.Success(null, "Xoá loại lỗi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-stt-kpi-error-type")]
        public IActionResult GetSTTKPIErrorType()
        {
            try
            {
                //var data = _kpiErrorTypeRepo.GetAll(x => x.IsDelete != true).Max(p => p.STT + 1);
                var nextSTT = _kpiErrorTypeRepo
                            .GetAll(x => x.IsDelete != true)
                            .Select(x => x.STT)
                            .DefaultIfEmpty(0)
                            .Max() + 1;

                return Ok(ApiResponseFactory.Success(nextSTT, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data-kpi-error-type")]
        public IActionResult SaveDataKPIErrorType([FromBody] KPIErrorType kpiErrorType)
        {
            try
            {
                if (!_kpiErrorTypeRepo.CheckValidate(kpiErrorType, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (kpiErrorType.ID > 0)
                {
                    var existingEntity = _kpiErrorTypeRepo.GetByID(kpiErrorType.ID);
                    if (existingEntity == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Loại lỗi không tồn tại"));
                    }
                    existingEntity.Name = kpiErrorType.Name;
                    existingEntity.Code = kpiErrorType.Code;
                    existingEntity.STT = kpiErrorType.STT;
                    _kpiErrorTypeRepo.Update(existingEntity);
                    return Ok(ApiResponseFactory.Success(null, "Cập nhật loại lỗi thành công"));
                }
                else
                {
                    kpiErrorType.IsDelete = false;
                    _kpiErrorTypeRepo.Create(kpiErrorType);
                    return Ok(ApiResponseFactory.Success(null, "Thêm mới loại lỗi thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
