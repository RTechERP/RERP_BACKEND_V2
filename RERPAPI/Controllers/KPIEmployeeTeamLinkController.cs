using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEmployeeTeamLinkController : ControllerBase
    {
        #region Khai báo repository
        KPIEmployeeTeamLinkRepo teamLinkRepo = new KPIEmployeeTeamLinkRepo();
        #endregion
        #region Lấy tất cả nhân viên trong team
        [HttpGet("get-all")]
        public IActionResult GetKPIEmployeeTeamLink(int kpiEmployeeteamID, int departmentID, int yearValue, int quarterValue)
        {
            try
            {
                var teamlinks = SQLHelper<object>.ProcedureToList("spGetKPIEmployeeTeamLink_New",
                                                                new string[] { "@KPIEmployeeteamID", "@DepartmentID", "@YearValue", "@QuarterValue" },
                                                                new object[] { kpiEmployeeteamID, departmentID, yearValue, quarterValue });


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
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<KPIEmployeeTeamLink> teamLinks)
        {
            try
            {
                foreach (var item in teamLinks)
                {
                    if (item.ID <= 0)
                    {
                        await teamLinkRepo.CreateAsync(item);
                    }
                    else await teamLinkRepo.UpdateAsync(item);
                }
                return Ok(new { status = 1, message = "Cập nhật thành công." });
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
