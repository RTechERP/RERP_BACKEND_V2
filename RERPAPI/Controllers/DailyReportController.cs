using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportController : ControllerBase
    {
        [HttpGet("getdailyreporttechnical")]
        public IActionResult GetDailyReportTechnical(DateTime dateStart, DateTime dateEnd, string keyword = "", int userID = 0,int departmenID=6)
        {
            try
            {
                List<DailyReportTechcnicalDTO> result = SQLHelper<DailyReportTechcnicalDTO>.ProcedureToList(
                 "spGetDailyReportTechnical",
                   new string[] { "@DateStart", "@DateEnd","UserID", "@Keyword", "@DepartmentID"},
                     new object[] { dateStart, dateEnd,userID, keyword,departmenID }

             );        

                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }


        [HttpGet("getdatafilmanddriver")]
        public IActionResult GetDataFilmanddriver(DateTime dateStart, DateTime dateEnd, string keyword = "", int employeeID = 0)
        {
            try
            {
                List<DailyReportHRDTO> result = SQLHelper<DailyReportHRDTO>.ProcedureToList(
                    "spGetDailyReportHR",
                    new string[] { "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID" },
                    new object[] { dateStart, dateEnd, keyword, employeeID }
                );

                var dataFilm = result.Where(x => x.ChucVuHDID == 7 || x.ChucVuHDID == 72).ToList();
                var dataDriver = result.Where(x => x.ChucVuHDID == 6).ToList();

                return Ok(new
                {
                    status = 1,
                    dataFilm = dataFilm,
                    dataDriver = dataDriver,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.",
                    error = ex.Message
                });
            }
        }



        [HttpGet("getdataemployee")]
        public IActionResult GetDataEmployee(int departmentID=0, int projectID=0)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists(
                "spGetUserProjectItem",
                        new string[] { "@DepartmentID", "@ProjectID" },
                       new object[] { departmentID,projectID }

                    );
                List<dynamic> rs = result[0];
                return Ok(new
                {
                    status = 1,
                    data = rs
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
