using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Linq.Expressions;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeWorkingProcessController : Controller
    {
        private readonly EmployeeWorkingProcessRepo _employeeWorkingProcessRepo;

        public EmployeeWorkingProcessController(EmployeeWorkingProcessRepo employeeWorkingProcessRepo)
        {
            _employeeWorkingProcessRepo = employeeWorkingProcessRepo;
        }   
        [HttpGet]
        public async Task<IActionResult> GetAll(string? filterText, DateTime dateStart, DateTime dateEnd, int pageNumber, int pageSize)
        {
            try
            {
                filterText = string.IsNullOrEmpty(filterText) ? "" : filterText;
                var workingProcesses = SQLHelper<object>.ProcedureToList("spLoadEmployeeWorkingProcess",
                                       new string[] { "@FilterText", "@DateStart", "@DateEnd", "@PageNumber", "@PageSize" },
                                                          new object[] { filterText, dateStart, dateEnd, pageNumber, pageSize});
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(workingProcesses, 0)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var workingProcess = _employeeWorkingProcessRepo.GetByID(id);
                if (workingProcess == null)
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Employee working process not found."
                    });
                }
                return Ok(new
                {
                    status = 1,
                    data = workingProcess
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

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] EmployeeWorkingProcess employeeWorkingProcess)
        {
            try
            {
                if(employeeWorkingProcess.ID <= 0)
                {
                    await _employeeWorkingProcessRepo.CreateAsync(employeeWorkingProcess);
                } else
                {
                    if(employeeWorkingProcess.IsApproved == true)
                    {
                        return BadRequest(new
                        {
                            status = 0,
                            message = "Không thể cập nhật hoặc xóa quá trình làm việc đã được phê duyệt."
                        });
                    }
                    await _employeeWorkingProcessRepo.UpdateAsync(employeeWorkingProcess);
                }
                return Ok(new
                {
                    status = 1,
                    data = employeeWorkingProcess,
                    message = "Lưu thành công"
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
