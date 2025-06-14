using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeApproveController : ControllerBase
    {
        EmployeeApproveRepo employeeApproveRepo = new EmployeeApproveRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();

        

        [HttpGet]
        public async Task<IActionResult> GetEmployeeApprove(int type, int projectID)
        {
            try
            {
                var employeeApproves = SQLHelper<object>.ProcedureToList("spGetEmployeeApprove",
                               new string[] { "@Type", "@ProjectID" },
                                              new object[] { 1, 0 });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(employeeApproves, 0)
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeeApprove(List<int> ListEmployeeID)
        {
            try
            {
                var employeeApproves = new List<EmployeeApprove>();
                foreach (var employeeID in ListEmployeeID)
                {
                    var employeeAprrove = SQLHelper<EmployeeApprove>.FindByAttribute("EmployeeID", employeeID);
                    if (employeeAprrove.Any())
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = $"Nhân viên với ID {employeeID} đã có trong danh sách người duyệt"
                        });
                    }

                    var employee = employeeRepo.GetByID(employeeID);
                    if (employee == null)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = $"Không tìm thấy nhân viên với ID {employeeID}"
                        });
                    }

                    var employeeApprove = new EmployeeApprove
                    {
                        EmployeeID = employeeID,
                        Code = employee.Code,
                        FullName = employee.FullName,
                        Type = 1
                    };
                    employeeApproves.Add(employeeApprove);
                }
                foreach (var employeeApprove in employeeApproves)
                {
                    await employeeApproveRepo.CreateAsync(employeeApprove);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Thêm người duyệt thành công",
                    data = employeeApproves
                });
            } catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeApprove(int id)
        {
            try
            {
                var employeeApprove = employeeApproveRepo.GetByID(id);
                if (employeeApprove == null)
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Không tìm thấy người duyệt với ID " + id
                    });
                }
                await employeeApproveRepo.DeleteAsync(id);
                return Ok(new
                {
                    status = 1,
                    message = "Xóa người duyệt thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        
    }
}
