using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIErrorController : ControllerBase
    {
        private readonly KPIErrorRepo _kpiErrorRepo;
        private readonly KPIErrorTypeRepo _kpiErrorTypeRepo;
        private readonly DepartmentRepo _departmentRepo;
        public KPIErrorController(KPIErrorRepo kpiErrorRepo, DepartmentRepo departmentRepo, KPIErrorTypeRepo kpiErrorTypeRepo)
        {
            _kpiErrorRepo = kpiErrorRepo;
            _departmentRepo = departmentRepo;
            _kpiErrorTypeRepo = kpiErrorTypeRepo;
        }
        [HttpGet("get-kpierror")]
        public IActionResult GetKPIError(int departmentId, string keyword = "")
        {
            try
            {
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIError",
                                                new string[] { "@Keyword", "@DepartmentID" },
                                                new object[] { keyword, departmentId });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-error-by-id")]
        public IActionResult GetKPIErrorById(int id)
        {
            try
            {
                var data = _kpiErrorRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-kpi-error")]
        public IActionResult DeleteKPIError(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));
                }
                var data = _kpiErrorRepo.GetByID(id);
                data.IsDelete = true;
                _kpiErrorRepo.Update(data);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var data = _departmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpi-error-type")]
        public IActionResult GetKPIErrorType()
        {
            try
            {
                var data = _kpiErrorTypeRepo.GetAll(x => x.IsDelete != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveKPIError([FromBody] KPIError model)
        {
            try
            {
                if (!_kpiErrorRepo.CheckValidate(model, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }

                if (model.ID > 0)
                {
                    await _kpiErrorRepo.UpdateAsync(model);
                }
                else
                {
                    await _kpiErrorRepo.CreateAsync(model);
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
