using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Repo.GenericEntity;
using System.Reflection.Metadata;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController] 
    [Authorize]
    public class SummaryKPIErrorEmployeeMonthController : ControllerBase
    {

        private readonly DepartmentRepo _departmentRepo;
        private readonly KPIErrorTypeRepo _kpiErrorTypeRepo;
        public SummaryKPIErrorEmployeeMonthController(DepartmentRepo departmentRepo, KPIErrorTypeRepo kpiErrorTypeRepo)
        {
            _departmentRepo = departmentRepo;
            _kpiErrorTypeRepo = kpiErrorTypeRepo;
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
                var data = _departmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpierror")]
        public IActionResult GetKPIError(int departmentId)
        {
            try
            {
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIError",
                                                new string[] { "@Keyword", "@DepartmentID" },
                                                new object[] { "", departmentId });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-summary-kpi-error-month")]
        public IActionResult GetKPIError(int departmentId, int typeId, DateTime dateStart, DateTime dateEnd, string keyword = "")
        {
            try
            {
                DateTime dateStartLocal = dateStart.ToLocalTime();
                DateTime dateEndLocal = dateEnd.ToLocalTime();
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetSummaryKPIErrorMonth",
                                                new string[] { "@StartDate", "@EndDate", "@Keyword", "@TypeID", "@DepartmentID" },
                                                new object[] { dateStartLocal, dateEndLocal, keyword, typeId, departmentId });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
