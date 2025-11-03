using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEmployeeTeamController : ControllerBase
    {
        #region Khai báo repository
        KPIEmployeeTeamRepo teamRepo = new KPIEmployeeTeamRepo();
        DepartmentRepo departmentRepo = new DepartmentRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();
        #endregion
        #region lấy ra tất cả team
        /// <summary>
        /// lấy team theo từng quý, nawmg, phòng ban
        /// </summary>
        /// <param name="yearValue"></param>
        /// <param name="quarterValue"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        [HttpGet("getall")]
        public IActionResult GetAll(int yearValue, int quarterValue, int departmentID)
        {

            try
            {

                var teams = SQLHelper<object>.ProcedureToList("spGetALLKPIEmployeeTeam",
                                                                new string[] { "@YearValue", "@QuarterValue", "@DepartmentID" },
                                                                new object[] { yearValue, quarterValue, departmentID });


                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(teams, 0)
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

        [HttpGet("get-employee-in-team")]
        public IActionResult GetAllEmployee(int departmentID = 0, int kpiEmployeeTeamID = 0)
        {
            try
            {
                var employees = SQLHelper<object>.ProcedureToList("spGetKPIEmployeeByDepartmentID", new string[] { "@DepartmentID", "@KPIEmployeeTeam" }, new object[] { departmentID, kpiEmployeeTeamID });

                return Ok(new { status = 1, data = SQLHelper<object>.GetListData(employees, 0) });
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
        #region lấy theo ID
        [HttpGet("getbyid")]
        public IActionResult FindByID(int id)
        {
            try
            {
                KPIEmployeeTeam team = teamRepo.GetByID(id);
                if (team.ID <= 0)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Team không có trong cơ sở dữ liệu!"
                    });
                }
                return Ok(new
                {
                    status = 1,
                    data = team
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
        public async Task<IActionResult> SaveData([FromBody] KPIEmployeeTeam team)
        {
            try
            {

                if (team.ID <= 0) await teamRepo.CreateAsync(team);
                else  await teamRepo.UpdateAsync(team);

                return Ok(new
                {
                    status = 1,
                    data = team,
                    message = "Cập nhật thành công!"
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
    }
}
