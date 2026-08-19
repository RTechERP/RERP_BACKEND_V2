using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.HotelBookingManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM.HotelBooking;

namespace RERPAPI.Controllers.HRM.HotelBooking
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelBookingManagementController : ControllerBase
    {
        private readonly HotelBookingManagementRepo _hotelBookingManagementRepo;
        private readonly HotelBookingProposalRepo _hotelBookingProposalRepo;
        private readonly HotelBookingEmployeeRepo _hotelBookingEmployeeRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly CurrentUser _currentUser;

        public HotelBookingManagementController(
            HotelBookingManagementRepo hotelBookingManagementRepo,
            HotelBookingProposalRepo hotelBookingProposalRepo,
            HotelBookingEmployeeRepo hotelBookingEmployeeRepo,
            EmployeeRepo employeeRepo,
            ProjectRepo projectRepo,
            CurrentUser currentUser)
        {
            _hotelBookingManagementRepo = hotelBookingManagementRepo;
            _hotelBookingProposalRepo = hotelBookingProposalRepo;
            _hotelBookingEmployeeRepo = hotelBookingEmployeeRepo;
            _employeeRepo = employeeRepo;
            _projectRepo = projectRepo;
            _currentUser = currentUser;
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("get-list")]
        public IActionResult GetList([FromBody] HotelBookingRequestParam request)
        {
            try
            {
                string procedureName = "spGetHotelBookingManagement";
                string[] paramNames = new string[] { "@StartDate", "@EndDate", "@Keyword", "@EmployeeID", "@ProjectID", "@EmployeeBookerID" };
                object[] paramValues = new object[] {
                    request.StartDate ?? (object)DBNull.Value,
                    request.EndDate ?? (object)DBNull.Value,
                    request.Keyword ?? "",
                    request.EmployeeID ?? 0,
                    request.ProjectID ?? 0,
                    request.EmployeeBookerID ?? 0
                };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var list = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-by-id")]
        public IActionResult GetByID(int id)
        {
            try
            {
                string procedureName = "spGetHotelBookingManagementByID";
                string[] paramNames = new string[] { "@ID" };
                object[] paramValues = new object[] { id };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var master = SQLHelper<object>.GetListData(data, 0).FirstOrDefault();
                var proposals = SQLHelper<object>.GetListData(data, 1);
                var employees = _hotelBookingEmployeeRepo.GetAll(x => x.HotelBookingManagementID == id && x.IsDeleted == false);

                return Ok(ApiResponseFactory.Success(new { master, proposals, employees }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] HotelBookingSaveDTO dto)
        {
            try
            {
                if (dto.Employees == null || !dto.Employees.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập ít nhất một người sử dụng phòng!"));
                }
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);

                if (dto.ID > 0)
                {
                    // Cập nhật bản ghi hiện có
                    var master = _hotelBookingManagementRepo.GetByID(dto.ID);
                    if (master == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu cần cập nhật!"));

                    master.EmployeeRequestID = dto.EmployeeRequestID;
                    master.Reason = dto.Reason;
                    master.ProjectID = dto.ProjectID;
                    master.Location = dto.Location;
                    master.CheckinDate = dto.CheckinDate;
                    master.CheckOutDate = dto.CheckOutDate;
                    master.EmployeeApproverID = dto.EmployeeApproverID;
                    master.Note = dto.Note;
                    master.UpdatedDate = DateTime.Now;
                    master.UpdatedBy = _currentUser.LoginName;

                    await _hotelBookingManagementRepo.UpdateAsync(master);

                    // Cập nhật các phương án đề xuất (Detail)
                    var oldProposals = _hotelBookingProposalRepo.GetAll(x => x.HotelBookingManagementID == master.ID);
                    foreach (var p in oldProposals)
                    {
                        p.IsDeleted = true;
                        p.UpdatedDate = DateTime.Now;
                        p.UpdatedBy = _currentUser.LoginName;
                        await _hotelBookingProposalRepo.UpdateAsync(p);
                    }

                    if (dto.Proposals != null)
                    {
                        foreach (var prop in dto.Proposals)
                        {
                            prop.ID = 0; // Chèn mới
                            prop.HotelBookingManagementID = master.ID;
                            prop.IsDeleted = false;
                            await _hotelBookingProposalRepo.CreateAsync(prop);
                        }
                    }

                    // Cập nhật danh sách nhân viên sử dụng phòng
                    var oldEmployees = _hotelBookingEmployeeRepo.GetAll(x => x.HotelBookingManagementID == master.ID);
                    foreach (var e in oldEmployees)
                    {
                        await _hotelBookingEmployeeRepo.DeleteAsync(e.ID);
                    }

                    if (dto.Employees != null)
                    {
                        foreach (var emp in dto.Employees)
                        {
                            if (emp.Type == 1 && emp.EmployeeID.HasValue && string.IsNullOrEmpty(emp.FullName))
                            {
                                var e = _employeeRepo.GetByID(emp.EmployeeID.Value);
                                emp.FullName = e?.FullName;
                            }
                            emp.ID = 0;
                            emp.HotelBookingManagementID = master.ID;
                            emp.IsDeleted = false;
                            await _hotelBookingEmployeeRepo.CreateAsync(emp);
                        }
                    }
                }
                else
                {
                    // Trường hợp tạo mới
                    var master = new HotelBookingManagement
                    {
                        EmployeeRequestID = dto.EmployeeRequestID,
                        Reason = dto.Reason,
                        ProjectID = dto.ProjectID,
                        Location = dto.Location,
                        CheckinDate = dto.CheckinDate,
                        CheckOutDate = dto.CheckOutDate,
                        EmployeeApproverID = dto.EmployeeApproverID,
                        EmployeeBookerID = _currentUser.EmployeeID,
                        DateRequest = DateTime.Now,
                        Note = dto.Note,
                        IsDeleted = false
                    };

                    await _hotelBookingManagementRepo.CreateAsync(master);

                    // Thêm đề xuất cho bản ghi mới này
                    if (dto.Proposals != null)
                    {
                        foreach (var prop in dto.Proposals)
                        {
                            var newProp = new HotelBookingProposal
                            {
                                HotelBookingManagementID = master.ID,
                                HotelName = prop.HotelName,
                                TypeRoom = prop.TypeRoom,
                                Quantity = prop.Quantity,
                                UnitPrice = prop.UnitPrice,
                                TotalAmount = prop.TotalAmount,
                                Note = prop.Note,
                                IsHCNSProposal = prop.IsHCNSProposal,
                                ReasonHCNSProposal = prop.ReasonHCNSProposal,
                                Distance = prop.Distance,
                                Area = prop.Area,
                                Convenience = prop.Convenience,
                                IsDeleted = false,
                                IsApprove = prop.IsApprove ?? 0,
                                ApproveID = prop.ApproveID,
                                ReasonDecline = prop.ReasonDecline
                            };
                            await _hotelBookingProposalRepo.CreateAsync(newProp);
                        }
                    }

                    // Thêm nhân viên sử dụng phòng cho bản ghi mới này
                    if (dto.Employees != null)
                    {
                        foreach (var emp in dto.Employees)
                        {
                            string fullName = emp.FullName;
                            if (emp.Type == 1 && emp.EmployeeID.HasValue && string.IsNullOrEmpty(fullName))
                            {
                                var e = _employeeRepo.GetByID(emp.EmployeeID.Value);
                                fullName = e?.FullName;
                            }
                            var newEmp = new HotelBookingEmployee
                            {
                                HotelBookingManagementID = master.ID,
                                Type = emp.Type,
                                EmployeeID = emp.EmployeeID,
                                FullName = fullName,
                                IsDeleted = false
                            };
                            await _hotelBookingEmployeeRepo.CreateAsync(newEmp);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu đăng ký đặt phòng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class DeleteRequest
        {
            public int ID { get; set; }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest req)
        {
            try
            {
                var master = _hotelBookingManagementRepo.GetByID(req.ID);
                if (master != null)
                {
                    master.IsDeleted = true;
                    await _hotelBookingManagementRepo.UpdateAsync(master);
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa thông tin thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("approve-proposal")]
        public async Task<IActionResult> ApproveProposal([FromBody] HotelBookingApproveDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);

                var proposal = _hotelBookingProposalRepo.GetByID(dto.ProposalID);
                if (proposal == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy phương án!"));

                proposal.IsApprove = dto.Status;
                proposal.ReasonDecline = dto.ReasonDecline;
                proposal.ApproveID = dto.Status == 0 ? null : _currentUser.EmployeeID;
                await _hotelBookingProposalRepo.UpdateAsync(proposal);

                return Ok(ApiResponseFactory.Success(null, "Cập nhật trạng thái thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("get-historical-suggestions")]
        public IActionResult GetHistoricalSuggestions()
        {
            try
            {
                var query = from m in _hotelBookingManagementRepo.GetAll(x => x.IsDeleted == false)
                            join p in _hotelBookingProposalRepo.GetAll(x => x.IsDeleted == false)
                            on m.ID equals p.HotelBookingManagementID into mp
                            from p in mp.DefaultIfEmpty()
                            select new
                            {
                                Location = m.Location,
                                TypeRoom = p != null ? p.TypeRoom : null,
                                HotelName = p != null ? p.HotelName : null
                            };

                var data = query.Distinct().ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel([FromBody] HotelBookingRequestParam request)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("RTC");

                string selectedIDsStr = request.SelectedIDs != null && request.SelectedIDs.Any()
                    ? string.Join(",", request.SelectedIDs)
                    : "";

                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHotelBookingExportExcel",
                    new string[] { "@StartDate", "@EndDate", "@Keyword", "@ProjectID", "@SelectedIDs", "@EmployeeBookerID" },
                    new object[] {
                        request.StartDate ?? (object)DBNull.Value,
                        request.EndDate ?? (object)DBNull.Value,
                        request.Keyword ?? "",
                        request.ProjectID ?? 0,
                        selectedIDsStr,
                        request.EmployeeBookerID ?? (object)DBNull.Value
                    }
                );

                var listData = SQLHelper<dynamic>.GetListData(dt, 0);
                var passengersList = SQLHelper<dynamic>.GetListData(dt, 1);

                if (listData == null || listData.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xuất Excel!"));
                }

                // Nhóm hành khách theo HotelBookingManagementID
                var passengersGrouped = passengersList.Cast<IDictionary<string, object>>()
                                                      .GroupBy(x => Convert.ToInt32(x["HotelBookingManagementID"]))
                                                      .ToDictionary(g => g.Key, g => g.ToList());

                // Nhóm theo MasterID
                var groups = listData.Cast<IDictionary<string, object>>()
                                    .GroupBy(x => x["MasterID"])
                                    .ToList();

                // Xác định số PA tối đa (tối thiểu 2)
                int maxPA = groups.Max(g => g.Count());
                if (maxPA < 2) maxPA = 2;

                int startPACol = 11;
                int hcnsProposalCol = startPACol + maxPA;
                int diffCol = hcnsProposalCol + 1;
                int hcnsReasonCol = diffCol + 1;
                int totalCol = hcnsReasonCol + 1;
                int approverCol = totalCol + 1;
                int bookerCol = approverCol + 1;
                int bookedDateCol = bookerCol + 1;
                int noteCol = bookedDateCol + 1;

                int totalCols = noteCol;

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Hotel Booking");
                    sheet.Cells.Style.Font.Name = "Times New Roman";
                    sheet.Cells.Style.Font.Size = 11;

                    // 1. Tiêu đề
                    sheet.Cells[1, 1].Value = "DANH SÁCH THEO DÕI ĐẶT PHÒNG KHÁCH SẠN";
                    sheet.Cells[1, 1, 1, totalCols].Merge = true;
                    using (var range = sheet.Cells[1, 1])
                    {
                        range.Style.Font.Size = 16;
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    sheet.Row(1).Height = 35;

                    // 2. Tiêu đề cột Dòng 2 & 3
                    string[] masterHeaders = { "STT", "Người yêu cầu", "Mục đích", "Dự án", "Người đi", "Vị trí", "Phòng ban", "Địa điểm / Khách sạn", "Check-in", "Check-out" };
                    for (int i = 0; i < masterHeaders.Length; i++)
                    {
                        sheet.Cells[2, i + 1].Value = masterHeaders[i];
                        sheet.Cells[2, i + 1, 3, i + 1].Merge = true;
                    }

                    int colIndex = startPACol;
                    for (int i = 1; i <= maxPA; i++)
                    {
                        sheet.Cells[2, colIndex].Value = "Phương án " + i;
                        sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                        colIndex++;
                    }

                    sheet.Cells[2, colIndex].Value = "Phương án HCNS đề xuất";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Chênh lệch\nchi phí";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Lý do HCNS đề xuất";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Tổng tiền";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Người duyệt";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Người đặt";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Ngày đặt";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Ghi chú";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;

                    // Định dạng tiêu đề cột (Headers)
                    using (var range = sheet.Cells[2, 1, 3, totalCols])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    // 3. Đổ dữ liệu
                    int row = 4;
                    int stt = 1;

                    var groupsList = groups.ToList();
                    for (int gIdx = 0; gIdx < groupsList.Count; gIdx++)
                    {
                        var group = groupsList[gIdx];
                        var items = group.ToList();
                        int proposalCount = items.Count;
                        var first = items[0];
                        int masterID = Convert.ToInt32(first["MasterID"]);

                        List<IDictionary<string, object>> groupPassengers = null;
                        if (passengersGrouped.TryGetValue(masterID, out var pList))
                        {
                            groupPassengers = pList;
                        }
                        int passengerCount = groupPassengers != null ? groupPassengers.Count : 0;

                        int maxRows = Math.Max(passengerCount, proposalCount);
                        if (maxRows < 1) maxRows = 1;

                        int startRow = row;

                        decimal pa1Price = 0;
                        decimal pa2Price = 0;
                        decimal totalApproved = 0;
                        bool hasApproved = false;
                        decimal minPrice = decimal.MaxValue;
                        string approverName = "";

                        var hcnsProposalsList = new List<string>();
                        string hcnsReason = "";

                        for (int i = 0; i < maxRows; i++)
                        {
                            if (i == 0)
                            {
                                sheet.Cells[startRow, 1].Value = stt++;
                                sheet.Cells[startRow, 2].Value = first["RequesterName"];
                                sheet.Cells[startRow, 3].Value = first["Reason"];
                                sheet.Cells[startRow, 4].Value = first["ProjectName"];
                                sheet.Cells[startRow, 8].Value = first["Location"];
                                sheet.Cells[startRow, 9].Value = first["CheckinDate"] != null && first["CheckinDate"] != DBNull.Value ? ((DateTime)first["CheckinDate"]).ToString("dd/MM/yyyy") : "";
                                sheet.Cells[startRow, 10].Value = first["CheckOutDate"] != null && first["CheckOutDate"] != DBNull.Value ? ((DateTime)first["CheckOutDate"]).ToString("dd/MM/yyyy") : "";
                            }

                            if (groupPassengers != null && i < passengerCount)
                            {
                                var passenger = groupPassengers[i];
                                sheet.Cells[row, 5].Value = passenger["PassengerName"];
                                sheet.Cells[row, 6].Value = passenger["PositionName"];
                                sheet.Cells[row, 7].Value = passenger["DepartmentName"];
                            }

                            if (i < proposalCount)
                            {
                                var item = items[i];

                                int paCol = startPACol + i;

                                string GetDictVal(IDictionary<string, object> dict, string key)
                                {
                                    var k = dict.Keys.FirstOrDefault(x => string.Equals(x, key, StringComparison.OrdinalIgnoreCase));
                                    if (k != null && dict[k] != null && dict[k] != DBNull.Value)
                                    {
                                        return dict[k].ToString() ?? "";
                                    }
                                    return "";
                                }

                                string typeRoom = GetDictVal(item, "TypeRoom");
                                string hotelName = GetDictVal(item, "HotelName");
                                decimal unitPrice = 0;
                                var upVal = GetDictVal(item, "UnitPrice");
                                if (!string.IsNullOrEmpty(upVal)) decimal.TryParse(upVal, out unitPrice);

                                int quantity = 0;
                                var qVal = GetDictVal(item, "Quantity");
                                if (!string.IsNullOrEmpty(qVal)) int.TryParse(qVal, out quantity);

                                decimal totalAmount = 0;
                                var taVal = GetDictVal(item, "TotalAmount");
                                if (!string.IsNullOrEmpty(taVal)) decimal.TryParse(taVal, out totalAmount);
                                else totalAmount = unitPrice * quantity;

                                string priceStr = unitPrice > 0 ? unitPrice.ToString("#,##0") : "";
                                string totalStr = totalAmount > 0 ? totalAmount.ToString("#,##0") : "";

                                var lines = new List<string>();
                                if (!string.IsNullOrEmpty(hotelName)) lines.Add($"Tên khách sạn: {hotelName}");
                                if (!string.IsNullOrEmpty(typeRoom)) lines.Add($"Loại phòng: {typeRoom}");
                                if (quantity > 0) lines.Add($"SL: {quantity}");
                                if (!string.IsNullOrEmpty(priceStr)) lines.Add($"Đơn giá: {priceStr}");
                                if (!string.IsNullOrEmpty(totalStr)) lines.Add($"Thành tiền: {totalStr}");

                                sheet.Cells[row, paCol].Value = string.Join("\n", lines);
                                sheet.Cells[row, paCol].Style.WrapText = true;

                                bool isHCNS = item["HCNSProposal"] != null && item["HCNSProposal"] != DBNull.Value && Convert.ToBoolean(item["HCNSProposal"]);
                                if (isHCNS)
                                {
                                    hcnsReason = item["ReasonHCNSProposal"] != null ? item["ReasonHCNSProposal"].ToString() : "";
                                    hcnsProposalsList.Add("Phương án " + (i + 1));
                                }

                                if (i == 0) pa1Price = totalAmount;
                                if (i == 1) pa2Price = totalAmount;

                                if (totalAmount > 0 && totalAmount < minPrice)
                                {
                                    minPrice = totalAmount;
                                }

                                int isApprove = item["IsApprove"] != null && item["IsApprove"] != DBNull.Value ? Convert.ToInt32(item["IsApprove"]) : 0;
                                if (isApprove == 1)
                                {
                                    totalApproved += totalAmount;
                                    hasApproved = true;
                                    approverName = item["ApproverName"] != null ? item["ApproverName"].ToString() : "";
                                }
                                else if (string.IsNullOrEmpty(approverName) && item["ApproverName"] != null)
                                {
                                    approverName = item["ApproverName"].ToString();
                                }
                            }

                            row++;
                        }

                        int endRow = row - 1;

                        sheet.Cells[startRow, hcnsProposalCol].Value = string.Join("\n\n", hcnsProposalsList);
                        sheet.Cells[startRow, hcnsProposalCol].Style.WrapText = true;

                        sheet.Cells[startRow, hcnsReasonCol].Value = hcnsReason;

                        sheet.Cells[startRow, diffCol].Value = Math.Abs(pa1Price - pa2Price);
                        sheet.Cells[startRow, diffCol].Style.Numberformat.Format = "#,##0";

                        decimal totalVal = hasApproved ? totalApproved : (minPrice == decimal.MaxValue ? 0 : minPrice);
                        sheet.Cells[startRow, totalCol].Value = totalVal;
                        sheet.Cells[startRow, totalCol].Style.Numberformat.Format = "#,##0";

                        sheet.Cells[startRow, approverCol].Value = approverName;
                        sheet.Cells[startRow, bookerCol].Value = first["BookerName"];
                        sheet.Cells[startRow, bookedDateCol].Value = first["BookedDate"] != null && first["BookedDate"] != DBNull.Value ? ((DateTime)first["BookedDate"]).ToString("dd/MM/yyyy HH:mm") : "";
                        sheet.Cells[startRow, noteCol].Value = first["Note"];

                        if (endRow > startRow)
                        {
                            int[] colsToMerge = { 1, 2, 3, 4, 8, 9, 10, hcnsProposalCol, diffCol, hcnsReasonCol, totalCol, approverCol, bookerCol, bookedDateCol, noteCol };
                            foreach (int col in colsToMerge)
                            {
                                sheet.Cells[startRow, col, endRow, col].Merge = true;
                            }
                        }
                    }

                    // 4. Định dạng (Styling)
                    var allRange = sheet.Cells[1, 1, row - 1, totalCols];
                    allRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    allRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    allRange.Style.WrapText = true;

                    // Kẻ khung cho tất cả các ô dữ liệu
                    using (var range = sheet.Cells[4, 1, row - 1, totalCols])
                    {
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    // Độ rộng các cột
                    sheet.Column(1).Width = 5;   // STT
                    sheet.Column(2).Width = 20;  // Người yêu cầu
                    sheet.Column(3).Width = 30;  // Lý do
                    sheet.Column(4).Width = 20;  // Dự án
                    sheet.Column(5).Width = 20;  // Người đi/Hành khách
                    sheet.Column(6).Width = 15;  // Vị trí
                    sheet.Column(7).Width = 20;  // Phòng ban
                    sheet.Column(8).Width = 30;  // Địa điểm / Khách sạn
                    sheet.Column(9).Width = 15;  // Checkin
                    sheet.Column(10).Width = 15; // Checkout

                    for (int i = startPACol; i < hcnsProposalCol; i++)
                    {
                        sheet.Column(i).Width = 25;
                    }
                    sheet.Column(hcnsProposalCol).Width = 25;
                    sheet.Column(hcnsReasonCol).Width = 35;
                    sheet.Column(diffCol).Width = 20;
                    sheet.Column(totalCol).Width = 15;
                    sheet.Column(approverCol).Width = 20;
                    sheet.Column(bookerCol).Width = 20;
                    sheet.Column(bookedDateCol).Width = 20;
                    sheet.Column(noteCol).Width = 30;

                    var stream = new System.IO.MemoryStream(package.GetAsByteArray());
                    string fileName = $"HotelBooking_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("ExportExcelDetail")]
        public IActionResult ExportExcelDetail(int id)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("RTC");

                var data = SQLHelper<dynamic>.ProcedureToList("spGetHotelBookingManagementByID", new string[] { "@ID" }, new object[] { id });
                var masterList = SQLHelper<dynamic>.GetListData(data, 0);
                var proposalsList = SQLHelper<dynamic>.GetListData(data, 1);
                var employees = _hotelBookingEmployeeRepo.GetAll(x => x.HotelBookingManagementID == id && x.IsDeleted == false);

                if (masterList == null || masterList.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu yêu cầu đặt phòng!"));
                }

                var master = masterList.FirstOrDefault() as IDictionary<string, object>;
                var proposals = proposalsList != null ? proposalsList.Cast<IDictionary<string, object>>().ToList() : new List<IDictionary<string, object>>();

                string GetDictVal(IDictionary<string, object>? dict, string key)
                {
                    if (dict == null) return "";
                    var k = dict.Keys.FirstOrDefault(x => string.Equals(x, key, StringComparison.OrdinalIgnoreCase));
                    if (k != null && dict[k] != null && dict[k] != DBNull.Value)
                    {
                        return dict[k].ToString() ?? "";
                    }
                    return "";
                }

                // Tiêu đề động: KHÁCH SẠN CHO [Tên người] ĐÊM [CheckinDate]
                string passengerNames = employees != null && employees.Any()
                    ? string.Join(", ", employees.Select(e => e.FullName))
                    : GetDictVal(master, "RequesterName");

                if (string.IsNullOrEmpty(passengerNames))
                {
                    passengerNames = GetDictVal(master, "RequesterName");
                }

                string checkinStr = "";
                var checkinObj = master != null && master.ContainsKey("CheckinDate") ? master["CheckinDate"] : null;
                if (checkinObj != null && checkinObj != DBNull.Value && DateTime.TryParse(checkinObj.ToString(), out DateTime checkinDt))
                {
                    checkinStr = $"ĐÊM {checkinDt.Day}/{checkinDt.Month}";
                }

                string title = $"KHÁCH SẠN CHO {passengerNames.ToUpper()} {checkinStr}".Trim();

                int proposalCount = proposals.Count;
                int totalCols = Math.Max(2 + proposalCount, 3); // Ít nhất 3 cột (STT, DANH MỤC, Phương án 1)

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Chi tiết đặt phòng");
                    sheet.Cells.Style.Font.Name = "Times New Roman";
                    sheet.Cells.Style.Font.Size = 11;

                    // 1. Tiêu đề
                    sheet.Cells[1, 1].Value = title;
                    sheet.Cells[1, 1, 1, totalCols].Merge = true;
                    using (var range = sheet.Cells[1, 1])
                    {
                        range.Style.Font.Size = 14;
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    sheet.Row(1).Height = 35;

                    // 2. Tiêu đề cột Dòng 2 (Headers)
                    sheet.Cells[2, 1].Value = "STT";
                    sheet.Cells[2, 2].Value = "DANH MỤC";

                    for (int i = 0; i < proposalCount; i++)
                    {
                        sheet.Cells[2, 3 + i].Value = $"Phương án {i + 1}";
                    }
                    if (proposalCount == 0)
                    {
                        sheet.Cells[2, 3].Value = "Phương án 1";
                    }

                    using (var range = sheet.Cells[2, 1, 2, totalCols])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(226, 239, 218)); // Light green #E2EFDA
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    sheet.Row(2).Height = 28;

                    // 3. Các dòng danh mục (Dòng 3 - 12)
                    string[] categories = new string[]
                    {
                        "Tên khách sạn",
                        "Địa chỉ",
                        "Khoảng cách với địa chỉ công tác",
                        "Số lượng phòng",
                        "Loại phòng",
                        "Diện tích phòng (m2)",
                        "Tiện ích",
                        "Giá tiền (gồm thuế phí)",
                        "Ghi chú",
                        "Đề xuất"
                    };

                    for (int r = 0; r < categories.Length; r++)
                    {
                        int rowNum = 3 + r;
                        sheet.Cells[rowNum, 1].Value = r + 1;
                        sheet.Cells[rowNum, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        sheet.Cells[rowNum, 2].Value = categories[r];
                        sheet.Cells[rowNum, 2].Style.Font.Bold = true;
                        sheet.Cells[rowNum, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        int maxLinesInRow = 1;

                        for (int p = 0; p < proposalCount; p++)
                        {
                            var prop = proposals[p];
                            int colNum = 3 + p;
                            string cellText = "";

                            switch (r)
                            {
                                case 0: // Tên khách sạn
                                    cellText = GetDictVal(prop, "HotelName");
                                    sheet.Cells[rowNum, colNum].Value = cellText;
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 1: // Địa chỉ
                                    cellText = GetDictVal(master, "Location");
                                    sheet.Cells[rowNum, colNum].Value = cellText;
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 2: // Khoảng cách với địa chỉ công tác
                                    cellText = GetDictVal(prop, "Distance");
                                    sheet.Cells[rowNum, colNum].Value = cellText;
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 3: // Số lượng phòng
                                    var qtyStr = GetDictVal(prop, "Quantity");
                                    cellText = qtyStr;
                                    if (int.TryParse(qtyStr, out int qtyVal))
                                    {
                                        sheet.Cells[rowNum, colNum].Value = qtyVal;
                                    }
                                    else
                                    {
                                        sheet.Cells[rowNum, colNum].Value = qtyStr;
                                    }
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 4: // Loại phòng
                                    cellText = GetDictVal(prop, "TypeRoom");
                                    sheet.Cells[rowNum, colNum].Value = cellText;
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 5: // Diện tích phòng (m2)
                                    var areaStr = GetDictVal(prop, "Area");
                                    cellText = areaStr;
                                    if (decimal.TryParse(areaStr, out decimal areaVal) && areaVal > 0)
                                    {
                                        sheet.Cells[rowNum, colNum].Value = areaVal;
                                        sheet.Cells[rowNum, colNum].Style.Numberformat.Format = "#,##0.##";
                                    }
                                    else
                                    {
                                        sheet.Cells[rowNum, colNum].Value = areaStr;
                                    }
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 6: // Tiện ích
                                    var convVal = GetDictVal(prop, "Convenience");
                                    if (string.IsNullOrEmpty(convVal))
                                    {
                                        convVal = "Bao gồm bữa sáng";
                                    }
                                    cellText = convVal;
                                    sheet.Cells[rowNum, colNum].Value = cellText;
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 7: // Giá tiền (gồm thuế phí)
                                    var priceStr = GetDictVal(prop, "UnitPrice");
                                    if (string.IsNullOrEmpty(priceStr) || priceStr == "0")
                                    {
                                        priceStr = GetDictVal(prop, "TotalAmount");
                                    }
                                    cellText = priceStr;
                                    if (decimal.TryParse(priceStr, out decimal priceVal))
                                    {
                                        sheet.Cells[rowNum, colNum].Value = priceVal;
                                        sheet.Cells[rowNum, colNum].Style.Numberformat.Format = "#,##0";
                                    }
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                    break;

                                case 8: // Ghi chú
                                    cellText = GetDictVal(prop, "Note");
                                    sheet.Cells[rowNum, colNum].Value = cellText;
                                    sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    break;

                                case 9: // Đề xuất
                                    bool isHCNS = prop.ContainsKey("IsHCNSProposal") && prop["IsHCNSProposal"] != null && prop["IsHCNSProposal"] != DBNull.Value && Convert.ToBoolean(prop["IsHCNSProposal"]);
                                    if (isHCNS)
                                    {
                                        string reason = GetDictVal(prop, "ReasonHCNSProposal");
                                        cellText = $"Phương án {p + 1}\n{reason}";
                                        sheet.Cells[rowNum, colNum].Value = cellText;
                                        sheet.Cells[rowNum, colNum].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                        sheet.Cells[rowNum, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    }
                                    break;
                            }

                            if (!string.IsNullOrEmpty(cellText))
                            {
                                int explicitLines = cellText.Split('\n').Length;
                                int estimatedWrappedLines = (int)Math.Ceiling((double)cellText.Length / 35.0);
                                int totalCellLines = Math.Max(explicitLines, estimatedWrappedLines);
                                if (totalCellLines > maxLinesInRow) maxLinesInRow = totalCellLines;
                            }
                        }

                        // Đặt chiều cao dòng: tối thiểu 28pt, tăng theo số dòng text + 12pt padding
                        sheet.Row(rowNum).Height = Math.Max(28, maxLinesInRow * 16 + 12);
                    }

                    int lastDataRow = 2 + categories.Length;

                    // Kẻ khung & WrapText cho toàn bộ bảng
                    using (var range = sheet.Cells[2, 1, lastDataRow, totalCols])
                    {
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        range.Style.WrapText = true;
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    // Độ rộng các cột
                    sheet.Column(1).Width = 8;   // STT
                    sheet.Column(2).Width = 25;  // DANH MỤC
                    for (int p = 0; p < Math.Max(proposalCount, 1); p++)
                    {
                        sheet.Column(3 + p).Width = 40; // Các phương án thoáng hơn
                    }

                    var stream = new System.IO.MemoryStream(package.GetAsByteArray());
                    string fileName = $"ChiTietDatPhong_{id}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }

    public class HotelBookingApproveDTO
    {
        public int ProposalID { get; set; }
        public int Status { get; set; } // 0: Chờ duyệt, 1: Duyệt, 2: Không duyệt
        public string? ReasonDecline { get; set; }
    }
}
