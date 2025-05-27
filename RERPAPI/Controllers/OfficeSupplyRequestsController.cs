using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeSupplyRequestsController : ControllerBase
    {
        RTCContext db = new RTCContext();
        OfficeSupplyRequestsRepo officesupplyrequests = new OfficeSupplyRequestsRepo();

        [HttpGet("getdatadepartment")]
        public IActionResult GetdataDepartment()
        {
            try
            {
                List<Department> departmentList = SQLHelper<Department>.FindAll().OrderBy(x => x.STT).ToList();
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
        [HttpGet("getofficesupplyrequestsdetail")]
        public IActionResult GetOfficeSupplyRequestsDetail(int OfficeSupplyRequestsID)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists(
                        "spGetOfficeSupplyRequestsDetail",
                        new string[] { "@OfficeSupplyRequestsID" },
                       new object[] { OfficeSupplyRequestsID }

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

        [HttpPost("adminapproved")]
        public async Task<IActionResult> AdminApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest("Danh sách ID không hợp lệ.");

                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);

                    if (item != null && item.IsApproved == false && item.IsAdminApproved == false)
                    {
                        item.IsAdminApproved = true;
                        item.DateAdminApproved = DateTime.Now;                      
                    }
                    officesupplyrequests.UpdateFieldsByID(id, item);

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

        [HttpPost("unadminapproved")]
        public async Task<IActionResult> UnAdminApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest("Danh sách ID không hợp lệ.");

                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved==true && item.IsApproved==false)
                    {
                        item.IsAdminApproved = false;
                        item.DateAdminApproved = DateTime.Now;
                    }
                    officesupplyrequests.UpdateFieldsByID(id, item);
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

        [HttpPost("isapproved")]
        public async Task<IActionResult> IsApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest("Danh sách ID không hợp lệ.");
                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == false)
                    {
                        item.IsApproved = true;
                        item.DateApproved = DateTime.Now;                     
                    }
                   officesupplyrequests.UpdateFieldsByID(id, item);
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
    [HttpPost("unisapproved")]
        public IActionResult UnIsApproved([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest("Danh sách ID không hợp lệ.");
                foreach (var id in ids)
                {
                    var item = officesupplyrequests.GetByID(id);
                    if (item != null && item.IsAdminApproved == true && item.IsApproved == true)
                    {
                        item.IsApproved = false;
                        item.DateApproved = DateTime.Now;
                    }
                    officesupplyrequests.UpdateFieldsByID(id, item);
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


        [HttpGet("getofficesupplyrequests")]
        public IActionResult GetOfficeSupplyRequests(string? keyword, int? employeeID, int? departmentID, DateTime? monthInput)
        {
            try
            {
                List<List<dynamic>> result = SQLHelper<dynamic>.ProcedureToDynamicLists(
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

    }
}
