using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;

namespace RERPAPI.Controllers.OfficeSuppliesManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportController : ControllerBase
    {
        /// <summary>
        /// hàm lấy dữ liệu báo cáo nhân sự hr-it
        /// </summary>
        /// <param ngày bắt đầu="dateStart"></param>
        /// <param ngày kết thúc="dateEnd"></param>
        /// <param theo tên="keyword"></param>
        /// <param id nhân viên="userID"></param>
        /// <param id phòng ban="departmenID"></param>
        /// <returns></returns>
        [HttpPost("get-daily-report-technical")]
     
        public IActionResult getDailyReportTechnical([FromBody] DailyReportTechnicalRequestParam filter)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                 "spGetDailyReportTechnical",
                   new string[] { "@DateStart", "@DateEnd", "UserID", "@Keyword", "@DepartmentID" },
                     new object[] { filter.dateStart, filter.dateEnd, filter.userID, filter.keyword, filter.departmenID }
             );
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công báo cáo công việc!"));
             /*   return Ok(new
                {
                    status = 1,
                    data = result
                });*/
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        /// <summary>
        /// Hàm lấy dữ liệu báo cáo cắt phim-lái xe
        /// </summary>
        /// <param ngày bắt đầu="dateStart"></param>
        /// <param  ngày kết thúc="dateEnd"></param>
        /// <param từ khóa tìm kiếm="keyword"></param>
        /// <param id nhân sự ="employeeID"></param>
        /// <returns></returns>
        [HttpPost("get-daily-report-film-and-driver")]
        public IActionResult getDailyReportFilmAndDriver([FromBody] DailyReportHrRequestParam filter)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetDailyReportHR",
                    new string[] { "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID" },
                    new object[] { filter.dateStart, filter.dateEnd, filter.keyword, filter.employeeID }
                );

                var flatResult = result.FirstOrDefault() ?? new List<dynamic>();

                var dataFilm = flatResult.Where(x => x.ChucVuHDID == 7 || x.ChucVuHDID == 72).ToList();
                var dataDriver = flatResult.Where(x => x.ChucVuHDID == 6).ToList();
                var data = new {
                    dataFilm,
                    dataDriver,
                };


                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công báo cáo công việc"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-data-employee")]
        public IActionResult getDataEmployee(int departmentID = 0, int projectID = 0)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                "spGetUserProjectItem",
                        new string[] { "@DepartmentID", "@ProjectID" },
                       new object[] { departmentID, projectID }

                    );
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
