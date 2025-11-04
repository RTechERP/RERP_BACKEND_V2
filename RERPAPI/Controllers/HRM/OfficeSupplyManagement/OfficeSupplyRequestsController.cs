using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.HRM.OfficeSupplyManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSupplyRequestsController : ControllerBase
    {
        OfficeSupplyRequestsRepo officesupplyrequests = new OfficeSupplyRequestsRepo();
        DepartmentRepo _departmentRepo = new DepartmentRepo();
        [RequiresPermission("N1,N2,N34")]
        #region getdatadepartment cần bỏ
        [HttpGet("get-data-department")]
        public IActionResult GetdataDepartment()
        {
            try
            {
                //List<Department> departmentList = SQLHelper<Department>.FindAll().OrderBy(x => x.STT).ToList();
                List<Department> departmentList = _departmentRepo.GetAll().OrderBy(x => x.STT).ToList();
                return Ok(new
                {
                    status = 1,
                    data = departmentList
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

        /// <summary>
        /// hàm lấy dữ liệu danh sách đăng ký VPP
        /// </summary>
        /// <param name="keyword"></param>
        /// <param id người đăng ký="employeeID"></param>
        /// <param id phòng ban đăng ký="departmentID"></param>
        /// <param tháng="monthInput"></param>
        /// <returns></returns>
        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-office-supply-request")]
        public IActionResult getOfficeSupplyRequests(string? keyword, int? employeeID, int? departmentID, DateTime? monthInput)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetOfficeSupplyRequests",
                    new string[] { "@KeyWord", "@MonthInput", "@EmployeeID", "@DepartmentID" },
                   new object[] { keyword, monthInput, employeeID, departmentID }  // đảm bảo không null
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

        /// <summary>
        /// Hàm lấy chi tiết đăng ký văn phòng phẩm
        /// </summary>
        /// <param officesupplyrequestsID="id"></param>
        /// <returns></returns>
           [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-office-supply-request-detail")]
        public IActionResult GetOfficeSupplyRequestsDetail(int officeSupplyRequestsID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                        "spGetOfficeSupplyRequestsDetail",
                        new string[] { "@OfficeSupplyRequestsID" },
                       new object[] { officeSupplyRequestsID }

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
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("admin-approved")]
        public async Task<IActionResult> adminApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });

                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);

                    if (item != null && item.IsApproved == false && item.IsAdminApproved == false)
                    {
                        item.IsAdminApproved = true;
                        item.DateAdminApproved = DateTime.Now;
                    }
                    officesupplyrequests.Update(item);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Phê duyệt thành công."
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

        [HttpPost("un-admin-approved")]
        public async Task<IActionResult> UnAdminApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });

                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == false)
                    {
                        item.IsAdminApproved = false;
                        item.DateAdminApproved = DateTime.Now;
                    }
                    officesupplyrequests.Update( item);
                }
                return Ok(new
                {
                    status = 1,
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

        [HttpPost("is-approved")]
        public async Task<IActionResult> IsApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });
                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == false)
                    {
                        item.IsApproved = true;
                        item.DateApproved = DateTime.Now;
                    }
                    officesupplyrequests.Update(item);
                }
                return Ok(new
                {
                    status = 1,
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
        [HttpPost("un-is-approved")]
        public IActionResult UnIsApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(new { status = 0, message = "Lỗi", error = ToString() });
                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == true)
                    {
                        item.IsApproved = false;
                        item.DateApproved = DateTime.Now;
                    }
                    officesupplyrequests.Update(item);
                }
                return Ok(new
                {
                    status = 1,
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

        /// <summary>
        /// hàm tìm kiếm danh sách tổng đăng ký theo phòng ban
        /// </summary>
        /// <param năm tìm kiếm="year"></param>
        /// <param tháng tìm kiếm="month"></param>
        /// <param tên tìm kiếm="keyword"></param>
        /// <param id phòng ban="departmentId"></param>
        /// <returns></returns>
        [HttpPost("get-office-supply-request-summary")]
        public IActionResult getOfficeSupplyRequestSummary([FromBody] OfficeSupplyRequestSummaryParam filter)
        {
            try
            {
                DateTime dateStart = new DateTime(filter.year, filter.month, 1, 0, 0, 0);
                DateTime dateEnd = dateStart.AddMonths(1).AddSeconds(-1);

                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetOfficeSupplyRequestSummary",
                    new string[] { "@DateStart", "@DateEnd", "@Keyword", "@DepartmentID" },
                    new object[] { dateStart, dateEnd, filter.keyword, filter.departmentId }
                );
                return Ok(new
                {
                    status = 1,
                    data = result[0]
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
