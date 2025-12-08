using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Course
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        // lấy danh sách nhân viên cho bảng trong tổng hợp khóa học 
        [HttpGet("get-employees")]
        public IActionResult GetEmployee(int? userTeamID, int? departmentid, int? employeeID)
        {
            try
            {

                departmentid = departmentid ?? 0;
                userTeamID = userTeamID ?? 0;
                employeeID = employeeID ?? 0;

                var data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@DepartmentID", "@Status", "@ID" },
                                                new object[] { departmentid, 0, employeeID,
                });
                if (userTeamID > 0 && employeeID <= 0)
                {
                    data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployeeByTeamID",
                                                new string[] { "@TeamID" },
                                                new object[] { userTeamID });
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-course-summary")]
        public IActionResult GetCourseSummary( int? departmentid, int? employeeID)
        {
            try
            {

                departmentid = departmentid ?? 0;
                employeeID = employeeID ?? 0;

                var data = SQLHelper<object>.ProcedureToList("spGetCourseNew",
                                                new string[] { "@DepartmentID", "@Status", "@EmployeeID" },
                                                new object[] { departmentid, -1, employeeID,
                });
             
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data,0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
