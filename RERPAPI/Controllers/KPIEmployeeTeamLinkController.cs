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
        KPIEmployeeTeamLinkRepo teamLinkRepo = new KPIEmployeeTeamLinkRepo();
        [HttpGet("getall")]
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
        [HttpPost("savedata")]
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
                    else teamLinkRepo.UpdateFieldsByID(item.ID,item);
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

        //[HttpPost("deleteemployeeteamlink")]
        //public IActionResult DeleteEmployee([FromBody] List<int> employeeIDs)
        //{
        //    if (employeeIDs.Count <= 0) return Ok(new { status = 0, message = "Không có nhân viên nào để xóa" });

        //    var notFoundIds = new List<int>();
        //    foreach (var id in employeeIDs)
        //    {
        //        var entity = teamLinkRepo.GetByID(id);
        //        if (entity == null)
        //        {
        //            notFoundIds.Add(id);
        //            continue;
        //        }

        //        entity.IsDeleted = true;
        //        teamLinkRepo.Update(entity);
        //    }
        //    if (notFoundIds.Any())
        //    {
        //        return Ok(new
        //        {
        //            status = 0,
        //            message = $"Không tìm thấy nhân viên có ID: {string.Join(", ", notFoundIds)}"
        //        });
        //    }
        //    return Ok(new { status = 1, message = "Đã xóa thành công!" });
        //}
    }
}
