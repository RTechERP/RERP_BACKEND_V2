using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.FlightBookingManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM.FlightBooking;

namespace RERPAPI.Controllers.HRM.FlightBooking
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightBookingManagementController : ControllerBase
    {
        private readonly FlightBookingManagementRepo _flightBookingManagementRepo;
        private readonly FlightBookingProposalRepo _flightBookingProposalRepo;
        private readonly FlightBookingPassengerRepo _flightBookingPassengerRepo;
        private readonly EmployeeRepo _employeeRepo;

        private readonly ProjectRepo _projectRepo;
        private readonly CurrentUser _currentUser;

        public FlightBookingManagementController(
            FlightBookingManagementRepo flightBookingManagementRepo,
            FlightBookingProposalRepo flightBookingProposalRepo,
            FlightBookingPassengerRepo flightBookingPassengerRepo,
            EmployeeRepo employeeRepo,
            ProjectRepo projectRepo,
            CurrentUser currentUser)
        {
            _flightBookingManagementRepo = flightBookingManagementRepo;
            _flightBookingProposalRepo = flightBookingProposalRepo;
            _flightBookingPassengerRepo = flightBookingPassengerRepo;
            _employeeRepo = employeeRepo;
            _projectRepo = projectRepo;
            _currentUser = currentUser;
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-employees")]
        public IActionResult GetEmployees()
        {
            try
            {
                var data = _employeeRepo.GetAll(x => x.Status != 1).OrderBy(x => x.FullName)
                    .Select(x => new { x.ID, x.FullName, x.Code }).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-projects")]
        public IActionResult GetProjects()
        {
            try
            {
                var data = _projectRepo.GetAll(x => x.IsDeleted == false).OrderByDescending(x => x.ID)
                    .Select(x => new { x.ID, Name = x.ProjectName }).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("get-list")]
        public IActionResult GetList([FromBody] FlightBookingRequestParam request)
        {
            try
            {
                string procedureName = "spGetFlightBookingManagement";
                string[] paramNames = new string[] { "@StartDate", "@EndDate", "@Keyword", "@EmployeeID", "@ProjectID", "@EmployeeBookerID" };
                object[] paramValues = new object[] { request.StartDate, request.EndDate, request.Keyword ?? "", request.EmployeeID ?? 0, request.ProjectID ?? 0, request.EmployeeBookerID ?? 0 };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, ""));
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
                string procedureName = "spGetFlightBookingManagementByID";
                string[] paramNames = new string[] { "@ID" };
                object[] paramValues = new object[] { id };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var master = SQLHelper<object>.GetListData(data, 0).FirstOrDefault();
                var proposals = SQLHelper<object>.GetListData(data, 1);
                var passengers = _flightBookingPassengerRepo.GetAll(x => x.FlightBookingManagementID == id);

                return Ok(ApiResponseFactory.Success(new { master, proposals, passengers }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N34")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] FlightBookingSaveDTO dto)
        {
            try
            {
                if (dto.Passengers == null || !dto.Passengers.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập ít nhất một người đi!"));
                }
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);
                if (dto.ID > 0)
                {
                    //Cập nhật bản ghi hiện có (chỉ cập nhật bản ghi master )
                    var master = _flightBookingManagementRepo.GetByID(dto.ID);
                    if (master == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu cần cập nhật!"));

                    // Cập nhật các trường
                    master.EmployeeID = dto.Passengers.FirstOrDefault(x => x.Type == 1)?.EmployeeID;
                    master.Reason = dto.Reason;
                    master.ProjectID = dto.ProjectID;
                    master.DepartureAddress = dto.DepartureAddress;
                    master.ArrivesAddress = dto.ArrivesAddress;
                    master.DepartureDate = dto.DepartureDate;
                    master.DepartureTime = dto.DepartureTime;
                    master.Note = dto.Note;
                    master.EmployeeBookerID = _currentUser.EmployeeID;
                    master.EmployeeRequestID = dto.EmployeeRequestID;
                    master.IsRoundTrip = dto.IsRoundTrip;
                    await _flightBookingManagementRepo.UpdateAsync(master);

                    // Cập nhật các phương án(Detail)
                    var oldProposals = _flightBookingProposalRepo.GetAll(x => x.FlightBookingManagementID == master.ID);
                    foreach (var p in oldProposals)
                    {
                        p.IsDeleted = true;
                        p.UpdatedDate = DateTime.Now;

                        await _flightBookingProposalRepo.UpdateAsync(p);
                    }

                    if (dto.Proposals != null)
                    {
                        foreach (var prop in dto.Proposals)
                        {
                            prop.ID = 0; // Chèn mới
                            prop.FlightBookingManagementID = master.ID;
                            await _flightBookingProposalRepo.CreateAsync(prop);
                        }
                    }

                    // Cập nhật danh sách hành khách
                    var oldPassengers = _flightBookingPassengerRepo.GetAll(x => x.FlightBookingManagementID == master.ID);
                    foreach (var p in oldPassengers)
                    {
                        await _flightBookingPassengerRepo.DeleteAsync(p.ID);
                    }

                    if (dto.Passengers != null)
                    {
                        foreach (var passenger in dto.Passengers)
                        {
                            if (passenger.Type == 1 && passenger.EmployeeID.HasValue && string.IsNullOrEmpty(passenger.FullName))
                            {
                                var emp = _employeeRepo.GetByID(passenger.EmployeeID.Value);
                                passenger.FullName = emp?.FullName;
                            }
                            passenger.ID = 0;
                            passenger.FlightBookingManagementID = master.ID;
                            await _flightBookingPassengerRepo.CreateAsync(passenger);
                        }
                    }
                }
                else
                {
                    // Trường hợp: Tạo bản ghi mới
                    var master = new FlightBookingManagement
                    {
                        EmployeeID = dto.Passengers.FirstOrDefault(x => x.Type == 1)?.EmployeeID,
                        Reason = dto.Reason,
                        ProjectID = dto.ProjectID,
                        DepartureAddress = dto.DepartureAddress,
                        ArrivesAddress = dto.ArrivesAddress,
                        DepartureDate = dto.DepartureDate,
                        DepartureTime = dto.DepartureTime,
                        Note = dto.Note,
                        EmployeeBookerID = _currentUser.EmployeeID,
                        EmployeeRequestID = dto.EmployeeRequestID,
                        BookedDate = DateTime.Now,
                        IsRoundTrip = dto.IsRoundTrip
                    };

                    await _flightBookingManagementRepo.CreateAsync(master);

                    // Thêm các phương án cho bản ghi cụ thể này
                    if (dto.Proposals != null)
                    {
                        foreach (var prop in dto.Proposals)
                        {
                            var newProp = new FlightBookingProposal
                            {
                                FlightBookingManagementID = master.ID,
                                Airline = prop.Airline,
                                Price = prop.Price,
                                Baggage = prop.Baggage,
                                IsApprove = prop.IsApprove,
                                ApproveID = prop.ApproveID,
                                HCNSProposal = prop.HCNSProposal,
                                ReasonHCNSProposal = prop.ReasonHCNSProposal,
                                DepartureDate = prop.DepartureDate,
                                DepartureTime = prop.DepartureTime,
                                ReturnDate = prop.ReturnDate,
                                ReturnTime = prop.ReturnTime
                            };
                            await _flightBookingProposalRepo.CreateAsync(newProp);
                        }
                    }

                    // Thêm hành khách cho bản ghi cụ thể này
                    if (dto.Passengers != null)
                    {
                        foreach (var passenger in dto.Passengers)
                        {
                            string fullName = passenger.FullName;
                            if (passenger.Type == 1 && passenger.EmployeeID.HasValue && string.IsNullOrEmpty(fullName))
                            {
                                var emp = _employeeRepo.GetByID(passenger.EmployeeID.Value);
                                fullName = emp?.FullName;
                            }
                            var newPassenger = new FlightBookingPassenger
                            {
                                FlightBookingManagementID = master.ID,
                                Type = passenger.Type,
                                EmployeeID = passenger.EmployeeID,
                                FullName = fullName
                            };
                            await _flightBookingPassengerRepo.CreateAsync(newPassenger);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu đăng ký thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            try
            {
                var master = _flightBookingManagementRepo.GetByID(id);
                if (master != null)
                {
                    master.IsDeleted = true;
                    await _flightBookingManagementRepo.UpdateAsync(master);
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa thông tin thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approve-proposal")]
        public async Task<IActionResult> ApproveProposal([FromBody] FlightBookingApproveDTO dto)
        {
            try
            {
                var proposal = _flightBookingProposalRepo.GetByID(dto.ProposalID);
                if (proposal == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy phương án!"));

                proposal.IsApprove = dto.Status;
                proposal.ReasonDecline = dto.ReasonDecline;
                proposal.ApproveID = dto.Status == 0 ? null : _currentUser.EmployeeID;
                await _flightBookingProposalRepo.UpdateAsync(proposal);

                return Ok(ApiResponseFactory.Success(null, "Cập nhật trạng thái thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-historical-suggestions")]
        public IActionResult GetHistoricalSuggestions()
        {
            try
            {
                var query = from m in _flightBookingManagementRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null)
                            join p in _flightBookingProposalRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null)
                            on m.ID equals p.FlightBookingManagementID into mp
                            from p in mp.DefaultIfEmpty()
                            select new
                            {
                                DepartureAddress = m.DepartureAddress,
                                ArrivesAddress = m.ArrivesAddress,
                                Airline = p != null ? p.Airline : null,
                                Baggage = p != null ? p.Baggage : null
                            };

                var data = query.Distinct().ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel([FromBody] FlightBookingRequestParam request)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("RTC");

                string selectedIDsStr = request.SelectedIDs != null && request.SelectedIDs.Any()
                    ? string.Join(",", request.SelectedIDs)
                    : "";

                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetFlightBookingExportExcel",
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

                // Nhóm hành khách theo FlightBookingManagementID
                var passengersGrouped = passengersList.Cast<IDictionary<string, object>>()
                                                      .GroupBy(x => Convert.ToInt32(x["FlightBookingManagementID"]))
                                                      .ToDictionary(g => g.Key, g => g.ToList());

                // Kiểm tra xem có yêu cầu đặt vé nào là khứ hồi hay không
                bool hasRoundTrip = listData.Cast<IDictionary<string, object>>()
                                            .Any(x => x["IsRoundTrip"] != null && Convert.ToBoolean(x["IsRoundTrip"]));

                // Nhóm theo MasterID
                var groups = listData.Cast<IDictionary<string, object>>()
                                    .GroupBy(x => x["MasterID"])
                                    .ToList();

                // Xác định số PA tối đa (tối thiểu 2)
                int maxPA = groups.Max(g => g.Count());
                if (maxPA < 2) maxPA = 2;

                int startPACol = hasRoundTrip ? 15 : 13;
                int diffCol = startPACol + maxPA;
                int hcnsProposalCol = diffCol + 1;
                int hcnsReasonCol = hcnsProposalCol + 1;
                int totalCol = hcnsReasonCol + 1;
                int approverCol = totalCol + 1;
                int bookerCol = approverCol + 1;
                int bookedDateCol = bookerCol + 1;
                int noteCol = bookedDateCol + 1;

                int totalCols = noteCol;

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Flight Booking");
                    sheet.Cells.Style.Font.Name = "Times New Roman";
                    sheet.Cells.Style.Font.Size = 11;

                    // 1. Tiêu đề
                    sheet.Cells[1, 1].Value = "DANH SÁCH THEO DÕI ĐẶT VÉ MÁY BAY";
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
                    string[] masterHeaders = { "STT", "Người yêu cầu", "Mục đích", "Dự án", "Người đi", "Vị trí", "Phòng ban" };
                    for (int i = 0; i < masterHeaders.Length; i++)
                    {
                        sheet.Cells[2, i + 1].Value = masterHeaders[i];
                        sheet.Cells[2, i + 1, 3, i + 1].Merge = true;
                    }

                    int colIndex = 8;
                    sheet.Cells[2, colIndex].Value = "Khứ hồi";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Điểm đi";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Điểm đến";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Giờ bay";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Ngày bay";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    if (hasRoundTrip)
                    {
                        sheet.Cells[2, colIndex].Value = "Giờ về";
                        sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                        colIndex++;

                        sheet.Cells[2, colIndex].Value = "Ngày về";
                        sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                        colIndex++;
                    }

                    for (int i = 1; i <= maxPA; i++)
                    {
                        sheet.Cells[2, colIndex].Value = "Phương án " + i;
                        sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                        colIndex++;
                    }

                    sheet.Cells[2, colIndex].Value = "Chênh lệch\nchi phí";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Phương án HCNS đề xuất";
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
                        int masterID = GetDictInt(first, "MasterID");

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
                                sheet.Cells[startRow, 2].Value = GetDictString(first, "RequesterName");
                                sheet.Cells[startRow, 3].Value = GetDictString(first, "Reason");
                                sheet.Cells[startRow, 4].Value = GetDictString(first, "ProjectName");
                                sheet.Cells[startRow, 8].Value = GetDictBool(first, "IsRoundTrip") ? "x" : "";
                                sheet.Cells[startRow, 9].Value = GetDictString(first, "DepartureAddress");
                                sheet.Cells[startRow, 10].Value = GetDictString(first, "ArrivesAddress");
                            }

                            if (groupPassengers != null && i < passengerCount)
                            {
                                var passenger = groupPassengers[i];
                                sheet.Cells[row, 5].Value = GetDictString(passenger, "PassengerName");
                                sheet.Cells[row, 6].Value = GetDictString(passenger, "PositionName");
                                sheet.Cells[row, 7].Value = GetDictString(passenger, "DepartmentName");
                            }

                            row++;
                        }

                        int endRow = row - 1;

                        // Đổ dữ liệu các Phương án (Chia đều tổng số dòng maxRows cho các Phương án)
                        for (int i = 0; i < proposalCount; i++)
                        {
                            var item = items[i];
                            int pStartRow = startRow + (i * maxRows) / proposalCount;
                            int pEndRow = startRow + ((i + 1) * maxRows) / proposalCount - 1;

                            string dayOfWeekStr = "";
                            var depDateVal = GetDictValue(item, "DepartureDate");
                            if (depDateVal != null && depDateVal != DBNull.Value)
                            {
                                var dateVal = Convert.ToDateTime(depDateVal);
                                dayOfWeekStr = dateVal.DayOfWeek switch
                                {
                                    DayOfWeek.Sunday => "Chủ Nhật",
                                    DayOfWeek.Monday => "Thứ Hai",
                                    DayOfWeek.Tuesday => "Thứ Ba",
                                    DayOfWeek.Wednesday => "Thứ Tư",
                                    DayOfWeek.Thursday => "Thứ Năm",
                                    DayOfWeek.Friday => "Thứ Sáu",
                                    DayOfWeek.Saturday => "Thứ Bảy",
                                    _ => ""
                                };
                            }

                            var depTimeVal = GetDictValue(item, "DepartureTime");
                            string depTimeStr = depTimeVal != null && depTimeVal != DBNull.Value ? Convert.ToDateTime(depTimeVal).ToString("HH:mm") : "";
                            sheet.Cells[pStartRow, 11].Value = string.IsNullOrEmpty(dayOfWeekStr) ? depTimeStr : $"{dayOfWeekStr} {depTimeStr}";
                            sheet.Cells[pStartRow, 12].Value = depDateVal != null && depDateVal != DBNull.Value ? Convert.ToDateTime(depDateVal).ToString("dd/MM/yyyy") : "";

                            if (hasRoundTrip)
                            {
                                string dayOfWeekReturnStr = "";
                                var retDateVal = GetDictValue(item, "ReturnDate");
                                if (retDateVal != null && retDateVal != DBNull.Value)
                                {
                                    var dateVal = Convert.ToDateTime(retDateVal);
                                    dayOfWeekReturnStr = dateVal.DayOfWeek switch
                                    {
                                        DayOfWeek.Sunday => "Chủ Nhật",
                                        DayOfWeek.Monday => "Thứ Hai",
                                        DayOfWeek.Tuesday => "Thứ Ba",
                                        DayOfWeek.Wednesday => "Thứ Tư",
                                        DayOfWeek.Thursday => "Thứ Năm",
                                        DayOfWeek.Friday => "Thứ Sáu",
                                        DayOfWeek.Saturday => "Thứ Bảy",
                                        _ => ""
                                    };
                                }

                                var retTimeVal = GetDictValue(item, "ReturnTime");
                                string retTimeStr = retTimeVal != null && retTimeVal != DBNull.Value ? Convert.ToDateTime(retTimeVal).ToString("HH:mm") : "";
                                sheet.Cells[pStartRow, 13].Value = string.IsNullOrEmpty(dayOfWeekReturnStr) ? retTimeStr : $"{dayOfWeekReturnStr} {retTimeStr}";
                                sheet.Cells[pStartRow, 14].Value = retDateVal != null && retDateVal != DBNull.Value ? Convert.ToDateTime(retDateVal).ToString("dd/MM/yyyy") : "";
                            }

                            int paCol = startPACol + i;
                            string airline = GetDictString(item, "Airline");
                            var priceValObj = GetDictValue(item, "Price");
                            decimal priceVal = priceValObj != null && priceValObj != DBNull.Value ? Convert.ToDecimal(priceValObj) : 0;
                            string priceStr = priceVal > 0 ? priceVal.ToString("#,##0") : "";
                            string baggage = GetDictString(item, "Baggage");

                            var lines = new List<string>();
                            if (!string.IsNullOrEmpty(airline)) lines.Add(airline);
                            if (!string.IsNullOrEmpty(priceStr)) lines.Add(priceStr);
                            if (!string.IsNullOrEmpty(baggage)) lines.Add(baggage);

                            sheet.Cells[pStartRow, paCol].Value = string.Join("\n", lines);
                            sheet.Cells[pStartRow, paCol].Style.WrapText = true;
                            sheet.Cells[pStartRow, paCol].Style.Font.Color.SetColor(System.Drawing.Color.Black);

                            if (pEndRow > pStartRow)
                            {
                                sheet.Cells[pStartRow, 11, pEndRow, 11].Merge = true;
                                sheet.Cells[pStartRow, 12, pEndRow, 12].Merge = true;
                                if (hasRoundTrip)
                                {
                                    sheet.Cells[pStartRow, 13, pEndRow, 13].Merge = true;
                                    sheet.Cells[pStartRow, 14, pEndRow, 14].Merge = true;
                                }
                                for (int c = startPACol; c < startPACol + maxPA; c++)
                                {
                                    sheet.Cells[pStartRow, c, pEndRow, c].Merge = true;
                                }
                            }

                            bool isHCNS = GetDictBool(item, "HCNSProposal");
                            if (isHCNS)
                            {
                                hcnsReason = GetDictString(item, "ReasonHCNSProposal");
                                hcnsProposalsList.Add("Phương án " + (i + 1));
                            }

                            decimal price = priceVal;
                            if (i == 0) pa1Price = price;
                            if (i == 1) pa2Price = price;

                            if (price > 0 && price < minPrice)
                            {
                                minPrice = price;
                            }

                            int isApprove = GetDictInt(item, "IsApprove");
                            string appNameStr = GetDictString(item, "ApproverName");
                            if (isApprove == 1)
                            {
                                totalApproved += price;
                                hasApproved = true;
                                if (!string.IsNullOrEmpty(appNameStr))
                                {
                                    approverName = appNameStr;
                                }
                            }
                            else if (string.IsNullOrEmpty(approverName) && !string.IsNullOrEmpty(appNameStr))
                            {
                                approverName = appNameStr;
                            }
                        }

                        sheet.Cells[startRow, diffCol].Value = Math.Abs(pa1Price - pa2Price);
                        sheet.Cells[startRow, diffCol].Style.Numberformat.Format = "#,##0";

                        sheet.Cells[startRow, hcnsProposalCol].Value = string.Join("\n\n", hcnsProposalsList);
                        sheet.Cells[startRow, hcnsProposalCol].Style.WrapText = true;

                        sheet.Cells[startRow, hcnsReasonCol].Value = hcnsReason;

                        decimal totalVal = hasApproved ? totalApproved : (minPrice == decimal.MaxValue ? 0 : minPrice);
                        sheet.Cells[startRow, totalCol].Value = totalVal;
                        sheet.Cells[startRow, totalCol].Style.Numberformat.Format = "#,##0";

                        sheet.Cells[startRow, approverCol].Value = approverName;
                        sheet.Cells[startRow, bookerCol].Value = GetDictString(first, "BookerName");
                        var bookedDateVal = GetDictValue(first, "BookedDate");
                        sheet.Cells[startRow, bookedDateCol].Value = bookedDateVal != null && bookedDateVal != DBNull.Value ? Convert.ToDateTime(bookedDateVal).ToString("dd/MM/yyyy HH:mm") : "";
                        sheet.Cells[startRow, noteCol].Value = GetDictString(first, "Note");

                        if (endRow > startRow)
                        {
                            int[] colsToMerge = { 1, 2, 3, 4, 8, 9, 10, diffCol, hcnsProposalCol, hcnsReasonCol, totalCol, approverCol, bookerCol, bookedDateCol, noteCol };
                            foreach (int col in colsToMerge)
                            {
                                sheet.Cells[startRow, col, endRow, col].Merge = true;
                            }
                        }
                    }
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
                    sheet.Column(6).Width = 15;  // Vị trí/Chức vụ
                    sheet.Column(7).Width = 20;  // Phòng ban
                    sheet.Column(8).Width = 12;  // Khứ hồi
                    sheet.Column(9).Width = 20;  // Điểm đi
                    sheet.Column(10).Width = 20; // Điểm đến
                    sheet.Column(11).Width = 10; // Giờ đi (Giờ bay)
                    sheet.Column(12).Width = 12; // Ngày đi (Ngày bay)

                    if (hasRoundTrip)
                    {
                        sheet.Column(13).Width = 10; // Giờ về
                        sheet.Column(14).Width = 12; // Ngày về
                    }

                    for (int i = startPACol; i < diffCol; i++)
                    {
                        sheet.Column(i).Width = 20;
                    }
                    sheet.Column(diffCol).Width = 20;
                    sheet.Column(hcnsProposalCol).Width = 25;
                    sheet.Column(hcnsReasonCol).Width = 35;
                    sheet.Column(diffCol).Width = 20;
                    sheet.Column(totalCol).Width = 15;
                    sheet.Column(approverCol).Width = 20;
                    sheet.Column(bookerCol).Width = 20;
                    sheet.Column(bookedDateCol).Width = 20;
                    sheet.Column(noteCol).Width = 30;

                    var stream = new System.IO.MemoryStream(package.GetAsByteArray());
                    string fileName = $"FlightBooking_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private static object? GetDictValue(IDictionary<string, object> dict, string key)
        {
            if (dict == null) return null;
            var matchKey = dict.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
            if (matchKey != null && dict.TryGetValue(matchKey, out var val))
            {
                return val;
            }
            return null;
        }

        private static string GetDictString(IDictionary<string, object> dict, string key)
        {
            var val = GetDictValue(dict, key);
            return (val != null && val != DBNull.Value) ? val.ToString() ?? "" : "";
        }

        private static bool GetDictBool(IDictionary<string, object> dict, string key)
        {
            var val = GetDictValue(dict, key);
            if (val != null && val != DBNull.Value)
            {
                try { return Convert.ToBoolean(val); } catch { }
            }
            return false;
        }

        private static int GetDictInt(IDictionary<string, object> dict, string key)
        {
            var val = GetDictValue(dict, key);
            if (val != null && val != DBNull.Value)
            {
                try { return Convert.ToInt32(val); } catch { }
            }
            return 0;
        }
    }

    public class FlightBookingApproveDTO
    {
        public int ProposalID { get; set; }
        public int Status { get; set; } // 0: Chờ duyệt, 1: Duyệt, 2: Không duyệt
        public string? ReasonDecline { get; set; }
    }
}