using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KPIErrorEmployeeSummaryMaxController : ControllerBase
    {
        private readonly DepartmentRepo _departmentRepo;
        private readonly KPIErrorTypeRepo _kpiErrorTypeRepo;
        public KPIErrorEmployeeSummaryMaxController(DepartmentRepo departmentRepo, KPIErrorTypeRepo kpiErrorTypeRepo)
        {
            _departmentRepo = departmentRepo;
            _kpiErrorTypeRepo = kpiErrorTypeRepo;
        }

        [HttpGet("get-data")]
        public IActionResult GetData(DateTime dateStart, DateTime dateEnd, int departmentId, int employeeId, int kpiErrorTypeId)
        {
            try
            {
                DateTime dateStartLocal = dateStart.ToLocalTime();
                DateTime dateEndLocal = dateEnd.ToLocalTime();
                var list = SQLHelper<object>.ProcedureToList("spGetKPIErrorEmployeeSummaryMax",
                                                new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@KPIErrorTypeID" },
                                                new object[] { dateStartLocal, dateEndLocal, departmentId, employeeId, kpiErrorTypeId });
                var data = SQLHelper<object>.GetListData(list, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

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
                var data = _departmentRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult LoadUser()
        {
            try
            {
                var list = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                 new string[] { "@Status" },
                                 new object[] { 0 });
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
