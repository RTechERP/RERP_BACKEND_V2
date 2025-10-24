using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using ZXing;

namespace RERPAPI.Controllers.OfficeSuppliesManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSupplyRequestsController : ControllerBase
    {
        OfficeSupplyRequestsRepo officesupplyrequests = new OfficeSupplyRequestsRepo();
        DepartmentRepo _departmentRepo = new DepartmentRepo();

        #region getdatadepartment cần bỏ
        [HttpGet("get-data-department")]
        public IActionResult GetdataDepartment()
        {
            try
            {
                //List<Department> departmentList = SQLHelper<Department>.FindAll().OrderBy(x => x.STT).ToList();
                List<Department> departmentList = _departmentRepo.GetAll().OrderBy(x => x.STT).ToList();

                return Ok(ApiResponseFactory.Success(departmentList, "Lấy dữ liệu phòng ban thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
        [HttpGet("")]
        public IActionResult getOfficeSupplyRequests(string? keyword, int? employeeID, int? departmentID, DateTime? monthInput)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToList(
                    "spGetOfficeSupplyRequests",
                    new string[] { "@KeyWord", "@MonthInput", "@EmployeeID", "@DepartmentID" },
                   new object[] { keyword, monthInput, employeeID, departmentID }  // đảm bảo không null
                );     
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu danh sách đăng ký Vpp thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
       
        /// <summary>
        /// Hàm lấy chi tiết đăng ký văn phòng phẩm
        /// </summary>
        /// <param officesupplyrequestsID="id"></param>
        /// <returns></returns>
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
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu danh sách chi tiết đăng ký Vpp thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

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
                return Ok(ApiResponseFactory.Success(null, "Phê duyệt thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                    officesupplyrequests.Update(item);
                }
                return Ok(ApiResponseFactory.Success(null, "Hủy duyệt thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return Ok(ApiResponseFactory.Success(null, "Phê duyệt thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return Ok(ApiResponseFactory.Success(null, "Hủy duyệt thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu danh sách tổng hợp đăng ký Vpp thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



    }
}
