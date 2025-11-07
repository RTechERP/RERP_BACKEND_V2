using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]

    public class EmployeeApproveController : ControllerBase
    {

        EmployeeApprovedRepo _employeeApproveRepo = new EmployeeApprovedRepo();
        private class EmployeeApproveSelected
        {
            public int EmployeeID { get; set; }
            public string Code { get; set; }
            public string FullName { get; set; }
        }

        EmployeeApproveRepo employeeApproveRepo = new EmployeeApproveRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();



        [HttpGet]
        public async Task<IActionResult> GetEmployeeApprove()
        {
            try
            {
                List<EmployeeApprove> employeeApprovals = _employeeApproveRepo.GetAll();
                //return Ok(new
                //{
                //    status = 1,
                //    data = SQLHelper<object>.GetListData(employeeApprovals, 0)
                //});

                return Ok(ApiResponseFactory.Success(employeeApprovals, ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class AddEmployeeApproveRequest
        {
            public List<int> ListEmployeeID { get; set; } = new List<int>();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeeApprove([FromBody] AddEmployeeApproveRequest request)
        {
            try
            {
                if (request?.ListEmployeeID == null || !request.ListEmployeeID.Any())
                {
                    //return BadRequest(new
                    //{
                    //    status = 0,
                    //    message = "Danh sách nhân viên không được để trống"
                    //});

                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách nhân viên không được để trống"));
                }

                var employeeApproves = new List<EmployeeApprove>();

                foreach (var employeeID in request.ListEmployeeID)
                {
                    var existingEmployeeApprove = _employeeApproveRepo.GetAll(x => x.EmployeeID == employeeID);
                    if (existingEmployeeApprove.Any())
                    {
                        //return BadRequest(new
                        //{
                        //    status = 0,
                        //    message = $"Nhân viên với ID {employeeID} đã có trong danh sách người duyệt"
                        //});

                        return BadRequest(ApiResponseFactory.Fail(null, $"Nhân viên với ID {employeeID} đã có trong danh sách người duyệt"));
                    }

                    var employee = employeeRepo.GetByID(employeeID);
                    if (employee == null)
                    {
                        //return BadRequest(new
                        //{
                        //    status = 0,
                        //    message = $"Không tìm thấy nhân viên với ID {employeeID}"
                        //});

                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy nhân viên với ID {employeeID}"));
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

                //return Ok(new
                //{
                //    status = 1,
                //    message = "Thêm người duyệt thành công",
                //    data = employeeApproves
                //});

                return Ok(ApiResponseFactory.Success(employeeApproves, "Thêm người duyệt thành công"));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = "Có lỗi xảy ra khi thêm người duyệt",
                //    error = ex.Message
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                    //return NotFound(new
                    //{
                    //    status = 0,
                    //    message = "Không tìm thấy người duyệt với ID " + id
                    //});

                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy người duyệt với ID " + id));
                }
                await employeeApproveRepo.DeleteAsync(id);
                //return Ok(new
                //{
                //    status = 1,
                //    message = "Xóa người duyệt thành công"
                //});

                return Ok(ApiResponseFactory.Success(null, "Xóa người duyệt thành công"));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});


                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}
