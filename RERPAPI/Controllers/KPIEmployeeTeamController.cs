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
        private readonly KPIEmployeeTeamRepo _kPIEmployeeTeamRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly EmployeeRepo _employeeRepo;

        public KPIEmployeeTeamController(KPIEmployeeTeamRepo kPIEmployeeTeamRepo, DepartmentRepo departmentRepo, EmployeeRepo employeeRepo)
        {
            _kPIEmployeeTeamRepo = kPIEmployeeTeamRepo;
            _departmentRepo = departmentRepo;
            _employeeRepo = employeeRepo;
        }
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

        [HttpGet("getemployeeinteam")]
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
                KPIEmployeeTeam team = _kPIEmployeeTeamRepo.GetByID(id);
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

                if (team.ID <= 0) await _kPIEmployeeTeamRepo.CreateAsync(team);
                else await _kPIEmployeeTeamRepo.UpdateAsync(team);

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