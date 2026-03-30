using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    public class Course_KPIEmployeeTeamLinkController : ControllerBase
    {
        #region Khai báo repository
        private readonly Course_KPIEmployeeTeamLinkRepo _teamLinkRepo;
        public Course_KPIEmployeeTeamLinkController(Course_KPIEmployeeTeamLinkRepo teamLinkRepo)
        {
            this._teamLinkRepo = teamLinkRepo;
        }
        #endregion  
        #region Lấy tất cả nhân viên trong team
        [HttpGet("getall")]
        public IActionResult GetKPIEmployeeTeamLink(int kpiEmployeeteamID, int departmentID)
        {
            try
            {
                var teamlinks = SQLHelper<object>.ProcedureToList("spGetCourseKPIEmployeeTeamLink_New",
                                                                new string[] { "@KPIEmployeeteamID", "@DepartmentID"},
                                                                new object[] { kpiEmployeeteamID, 0 });


                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(teamlinks, 0)
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
        #endregion
        #region Lưu dữ liệu
        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] List<Course_KPIEmployeeTeamLink> teamLinks)
        {
            try
            {
                foreach (var item in teamLinks)
                {
                    if (item.ID <= 0)
                    {
                        await _teamLinkRepo.CreateAsync(item);
                    }
                    else await _teamLinkRepo.UpdateAsync(item);
                }
                return Ok(new { data = teamLinks, status = 1, message = "Cập nhật thành công." });
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
        #endregion
    }
}
