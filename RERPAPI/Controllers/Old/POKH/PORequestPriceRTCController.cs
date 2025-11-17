using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    public class PORequestPriceRTCController : ControllerBase
    {
        private readonly ProjectPartlistPriceRequestRepo _projectPartlistPriceRequests;
        public PORequestPriceRTCController(

            ProjectPartlistPriceRequestRepo projectPartlistPriceRequests
            )
        {
            _projectPartlistPriceRequests = projectPartlistPriceRequests;
        }

        [HttpGet("get-details")]
        public IActionResult loadDetailUser(int id)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetPOKHDetail", new string[] { "@ID", "@IDDetail" }, new object[] { id, 0 });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> Save(List<ProjectPartlistPriceRequest> models)
        {
            try
            {
                if (models == null || models.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách yêu cầu báo giá trống"));

                if (!models.Any(m => m.EmployeeID.HasValue && m.EmployeeID.Value > 0))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn Người yêu cầu!"));

                foreach (var m in models)
                {
                    string code = m.ProductCode ?? string.Empty;
                    int quantity = m.Quantity.HasValue ? Convert.ToInt32(m.Quantity.Value) : 0;
                    DateTime? deadline = m.Deadline;

                    ProjectPartlistPriceRequest existingReq = new ProjectPartlistPriceRequest();
                    if (m.ID > 0)
                    {
                        existingReq = await _projectPartlistPriceRequests.GetByIDAsync(m.ID);
                    }

                    if (existingReq != null && existingReq.IsDeleted == false)
                    {
                        if (existingReq.StatusRequest == 2)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Sản phẩm mã [{code}] đã báo giá.\nBạn không thể yêu cầu báo giá!"));
                        }

                        if (existingReq.StatusRequest == 3)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Sản phẩm mã [{code}] đã Hoàn thành báo giá.\nBạn không thể yêu cầu báo giá!"));
                        }

                        if (existingReq.IsCheckPrice == true)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Sản phẩm mã [{code}] đang check giá.\nBạn không thể yêu cầu báo giá!"));
                        }
                    }

                    if (!deadline.HasValue)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập Deadline sản phẩm mã [{code}]!"));
                    }

                    if (!CheckDeadLine(deadline.Value, out string deadlineMessage))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, deadlineMessage));
                    }

                    if (quantity <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập SL yêu cầu của mã [{code}]!"));
                    }
                }

                foreach (var i in models)
                {
                    var oldRequests = _projectPartlistPriceRequests.GetAll(x => x.POKHDetailID == i.POKHDetailID && x.IsDeleted == false).ToList();
                    if (oldRequests.Any())
                    {
                        foreach (var req in oldRequests)
                        {
                            req.IsDeleted = true;
                            req.UpdatedDate = DateTime.Now;
                            await _projectPartlistPriceRequests.UpdateAsync(req);
                        }
                    }
                    var newRequest = new ProjectPartlistPriceRequest
                    {
                        DateRequest = i.DateRequest,
                        EmployeeID = i.EmployeeID,
                        Deadline = i.Deadline,
                        ProductCode = i.ProductCode,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        StatusRequest = 1,
                        IsCommercialProduct = true,
                        POKHDetailID = i.POKHDetailID
                    };
                    await _projectPartlistPriceRequests.CreateAsync(newRequest);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> SoftDelete([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng cung cấp danh sách bản ghi cần xóa"));

                var deleted = new List<int>();
                var skipped = new List<object>();

                foreach (var id in ids.Distinct())
                {
                    var req = await _projectPartlistPriceRequests.GetByIDAsync(id);
                    if (req == null)
                    {
                        skipped.Add(new { Id = id, Reason = "NotFound" });
                        continue;
                    }

                    // Nếu đã báo giá (2), đã hoàn thành (3) hoặc đang check giá -> bỏ qua
                    if ((req.StatusRequest.HasValue && (req.StatusRequest == 2 || req.StatusRequest == 3)) || (req.IsCheckPrice == true))
                    {
                        skipped.Add(new { Id = id, Reason = "StatusRequest is 2 or 3, or IsCheckPrice = true" });
                        continue;
                    }

                    req.IsDeleted = true;
                    req.UpdatedDate = DateTime.Now;
                    req.UpdatedBy = User?.Identity?.Name ?? string.Empty;

                    await _projectPartlistPriceRequests.UpdateAsync(req);
                    deleted.Add(id);
                }

                var result = new
                {
                    Deleted = deleted,
                    Skipped = skipped
                };

                return Ok(ApiResponseFactory.Success(result, "Xử lý xóa hoàn tất"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private bool CheckDeadLine(DateTime deadline, out string message)
        {
            message = null;
            // Nếu ngày yêu cầu từ sau 15h, thì bắt đầu tính từ ngày hôm sau
            // Nếu ngày yêu cầu là ngày T7 hoặc CN thì bắt đầu tính từ T2
            // Ngày deadline phải lớn hơn ngày yêu cầu ít nhất 2 ngày (không tính T7, CN)
            TimeSpan cutoff = new TimeSpan(15, 0, 0);
            DateTime dateRequest = DateTime.Now;
            TimeSpan timeRequest = dateRequest.TimeOfDay;
            if (timeRequest >= cutoff)
            {
                dateRequest = dateRequest.AddDays(1);
            }

            if (dateRequest.DayOfWeek == DayOfWeek.Saturday)
            {
                dateRequest = dateRequest.AddDays(1);
            }

            if (dateRequest.DayOfWeek == DayOfWeek.Sunday)
            {
                dateRequest = dateRequest.AddDays(1);
            }

            var listDates = new List<DateTime>();
            double totalDays = (deadline.Date - dateRequest.Date).TotalDays;
            for (int i = 0; i <= totalDays; i++)
            {
                var date = dateRequest.AddDays(i).Date;
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                if (!listDates.Contains(date))
                {
                    listDates.Add(date);
                }
            }

            if (listDates.Count < 2)
            {
                message = $"Deadline phải ít nhất là 2 ngày tính từ [{dateRequest.ToString("dd/MM/yyyy")}]";
                return false;
            }

            return true;
        }
    }
}
