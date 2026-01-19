using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.Technical
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SummaryKPIErrorEmployeeController : ControllerBase
    {

        private readonly DepartmentRepo _departmentRepo;
        private readonly KPIErrorTypeRepo _kpiErrorTypeRepo;
        public SummaryKPIErrorEmployeeController(DepartmentRepo departmentRepo, KPIErrorTypeRepo kpiErrorTypeRepo)
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
                var data = _kpiErrorTypeRepo.GetAll(x => x.IsDelete != true).OrderBy(x => x.STT);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-kpierror")]
        public IActionResult GetKPIError(int typeId)
        {
            try
            {
                var dataKpiError = SQLHelper<object>.ProcedureToList("spGetKPIError",
                                                new string[] {"@TypeID"},
                                                new object[] { typeId });
                var data = SQLHelper<object>.GetListData(dataKpiError, 0);

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

        [HttpGet("get-data-th")]
        public IActionResult LoadDataTongHop(int month, int year, int kpiErrorId, int employeeId, int departmentId, string keywords = "")
        {
            try
            {
                var dataRaw1 = SQLHelper<object>.ProcedureToList("spGetSummaryKPIErrorEmployee",
                                                new string[] { "@Month", "@Year", "@KPIErrorID", "@EmployeeID", "@Keyword", "@TypeID", "@DepartmentID" },
                                                new object[] { month, year, kpiErrorId, employeeId, keywords, 1, departmentId });
                var data1 = SQLHelper<object>.GetListData(dataRaw1, 0);

                var dataRaw2 = SQLHelper<object>.ProcedureToList("spGetSummaryKPIErrorEmployee",
                                new string[] { "@Month", "@Year", "@KPIErrorID", "@EmployeeID", "@Keyword", "@TypeID", "@DepartmentID" },
                                new object[] { month, year, kpiErrorId, employeeId, keywords, 3, departmentId });
                var data2 = SQLHelper<object>.GetListData(dataRaw2, 0);

                var dataRaw3 = SQLHelper<object>.ProcedureToList("spGetSummaryKPIErrorEmployee",
                                new string[] { "@Month", "@Year", "@KPIErrorID", "@EmployeeID", "@Keyword", "@TypeID", "@DepartmentID" },
                                new object[] { month, year, kpiErrorId, employeeId, keywords, 9, departmentId });
                var data3 = SQLHelper<object>.GetListData(dataRaw3, 0);

                return Ok(ApiResponseFactory.Success(new { data1, data2, data3 }, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data-file")]
        public IActionResult LoadDataFile(int month, int year, int kpiErrorId, int employeeId, int departmentId, int typeId , string keywords = "")
        {
            try
            {
                var dataRaw1 = SQLHelper<object>.ProcedureToList("spGetSummaryKPIErrorEmployee",
                                                new string[] { "@Month", "@Year", "@KPIErrorID", "@EmployeeID", "@Keyword", "@TypeID", "@DepartmentID" },
                                                new object[] { month, year, kpiErrorId, employeeId, keywords, typeId, departmentId });
                var datafile = SQLHelper<object>.GetListData(dataRaw1, 1);
                return Ok(ApiResponseFactory.Success(datafile, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data-tk")]
        public IActionResult LoadDataTongKet(int month, int year, int typeId, int departmentId, string keywords = "")
        {
            try
            {
                var dataRaw1 = SQLHelper<object>.ProcedureToList("spGetSummaryKPIError",
                                                new string[] { "@Month", "@Year", "@Keyword", "@TypeID", "@DepartmentID" },
                                                new object[] { month, year, keywords, typeId, departmentId });
                var data1 = SQLHelper<object>.GetListData(dataRaw1, 0);
                return Ok(ApiResponseFactory.Success(data1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-kpi-error-in-month")]
        public IActionResult LoadKPIErrorInMonth(int month, int year, int kpiErrorId, int weekIndex, int deparmentID)
        {
            try
            {
                var dataRaw1 = SQLHelper<object>.ProcedureToList("spGetSummaryEmployeeKPIError",
                                                new string[] { "@Month", "@Year", "@KPIErrorID", "@WeekIndex", "@DepartmentID" },
                                                new object[] { month, year, kpiErrorId, weekIndex, deparmentID });
                var data1 = SQLHelper<object>.GetListData(dataRaw1, 0);
                return Ok(ApiResponseFactory.Success(data1, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
