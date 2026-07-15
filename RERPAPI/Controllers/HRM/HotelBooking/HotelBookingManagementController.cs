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
                                TypeRoom = prop.TypeRoom,
                                Quantity = prop.Quantity,
                                UnitPrice = prop.UnitPrice,
                                TotalAmount = prop.TotalAmount,
                                Note = prop.Note,
                                IsHCNSProposal = prop.IsHCNSProposal,
                                ReasonHCNSProposal = prop.ReasonHCNSProposal,
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
                                TypeRoom = p != null ? p.TypeRoom : null
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
                                string typeRoom = item["TypeRoom"] != null ? item["TypeRoom"].ToString() : "";
                                decimal unitPrice = item["UnitPrice"] != null && item["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(item["UnitPrice"]) : 0;
                                int quantity = item["Quantity"] != null && item["Quantity"] != DBNull.Value ? Convert.ToInt32(item["Quantity"]) : 0;
                                decimal totalAmount = item["TotalAmount"] != null && item["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(item["TotalAmount"]) : (unitPrice * quantity);

                                string priceStr = unitPrice > 0 ? unitPrice.ToString("#,##0") : "";
                                string totalStr = totalAmount > 0 ? totalAmount.ToString("#,##0") : "";

                                var lines = new List<string>();
                                if (!string.IsNullOrEmpty(typeRoom)) lines.Add($"Loại phòng:{typeRoom}");
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
    }

    public class HotelBookingApproveDTO
    {
        public int ProposalID { get; set; }
        public int Status { get; set; } // 0: Chờ duyệt, 1: Duyệt, 2: Không duyệt
        public string? ReasonDecline { get; set; }
    }
}
