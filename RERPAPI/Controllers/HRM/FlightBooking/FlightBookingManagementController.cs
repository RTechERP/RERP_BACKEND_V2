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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM.FlightBooking
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightBookingManagementController : ControllerBase
    {
        private readonly FlightBookingManagementRepo _flightBookingManagementRepo;
        private readonly FlightBookingProposalRepo _flightBookingProposalRepo;
        private readonly EmployeeRepo _employeeRepo;

        private readonly ProjectRepo _projectRepo;
        private readonly CurrentUser _currentUser;

        public FlightBookingManagementController(
            FlightBookingManagementRepo flightBookingManagementRepo,
            FlightBookingProposalRepo flightBookingProposalRepo,
            EmployeeRepo employeeRepo,
            ProjectRepo projectRepo,
            CurrentUser currentUser)
        {
            _flightBookingManagementRepo = flightBookingManagementRepo;
            _flightBookingProposalRepo = flightBookingProposalRepo;
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
                string[] paramNames = new string[] { "@StartDate", "@EndDate", "@Keyword", "@EmployeeID", "@ProjectID" };
                object[] paramValues = new object[] { request.StartDate, request.EndDate, request.Keyword ?? "", request.EmployeeID ?? 0, request.ProjectID ?? 0 };
                
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
                
                return Ok(ApiResponseFactory.Success(new { master, proposals }, ""));
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
                if (dto.TravelerIDs == null || !dto.TravelerIDs.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn ít nhất một người đi!"));
                }
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser _currentUser = ObjectMapper.GetCurrentUser(claims);
                if (dto.ID > 0)
                {
                    //Cập nhật bản ghi hiện có (chỉ cập nhật bản ghi master )
                    var master = _flightBookingManagementRepo.GetByID(dto.ID);
                    if (master == null) return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu cần cập nhật!"));

                    // Cập nhật các trường
                    master.EmployeeID = dto.TravelerIDs.First();
                    master.Reason = dto.Reason;
                    master.ProjectID = dto.ProjectID;
                    master.DepartureAddress = dto.DepartureAddress;
                    master.ArrivesAddress = dto.ArrivesAddress;
                    master.DepartureDate = dto.DepartureDate;
                    master.DepartureTime = dto.DepartureTime;
                    master.Note = dto.Note;
                    master.EmployeeBookerID = _currentUser.EmployeeID;
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
                }
                else
                {
                    // Trường hợp: Tạo bản ghi mới
                    foreach (var travelerID in dto.TravelerIDs)
                    {
                        var master = new FlightBookingManagement
                        {
                            EmployeeID = travelerID,
                            Reason = dto.Reason,
                            ProjectID = dto.ProjectID,
                            DepartureAddress = dto.DepartureAddress,
                            ArrivesAddress = dto.ArrivesAddress,
                            DepartureDate = dto.DepartureDate,
                            DepartureTime = dto.DepartureTime,
                            Note = dto.Note,
                            EmployeeBookerID = _currentUser.EmployeeID,
                            BookedDate = DateTime.Now,
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
                                    HCNSProposal = prop.HCNSProposal
                                };
                                await _flightBookingProposalRepo.CreateAsync(newProp);
                            }
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
                    new string[] { "@StartDate", "@EndDate", "@Keyword", "@ProjectID", "@SelectedIDs" },
                    new object[] { 
                        request.StartDate ?? (object)DBNull.Value, 
                        request.EndDate ?? (object)DBNull.Value, 
                        request.Keyword ?? "", 
                        request.ProjectID ?? 0,
                        selectedIDsStr
                    }
                );

                var listData = SQLHelper<dynamic>.GetListData(dt, 0);

                if (listData == null || listData.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xuất Excel!"));
                }

                // Nhóm theo MasterID
                var groups = listData.Cast<IDictionary<string, object>>()
                                    .GroupBy(x => x["MasterID"])
                                    .ToList();

                // Xác định số PA tối đa (tối thiểu 2)
                int maxPA = groups.Max(g => g.Count());
                if (maxPA < 2) maxPA = 2;

                int diffCol = 12 + (maxPA * 4);
                int totalCol = diffCol + 1;
                int bookerCol = totalCol + 1;
                int bookedDateCol = bookerCol + 1;
                int noteCol = bookedDateCol + 1;

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Flight Booking");
                    sheet.Cells.Style.Font.Name = "Times New Roman";
                    sheet.Cells.Style.Font.Size = 11;

                    // 1. Tiêu đề
                    sheet.Cells[1, 1].Value = "DANH SÁCH THEO DÕI ĐẶT VÉ MÁY BAY";
                    int totalCols = 11 + (maxPA * 4) + 5; // Cột A-K (11) + các khối PA + Chênh lệch + Tổng + Người đặt + Ngày đặt + Ghi chú
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
                    string[] masterHeaders = { "STT", "Người yêu cầu", "Mục đích", "Dự án", "Người đi", "Vị trí", "Phòng ban", "Điểm đi", "Điểm đến", "Giờ bay", "Ngày bay" };
                    for (int i = 0; i < masterHeaders.Length; i++)
                    {
                        sheet.Cells[2, i + 1].Value = masterHeaders[i];
                        sheet.Cells[2, i + 1, 3, i + 1].Merge = true;
                    }

                    int colIndex = 12;
                    for (int i = 1; i <= maxPA; i++)
                    {
                        sheet.Cells[2, colIndex].Value = "Phương án " + i;
                        sheet.Cells[2, colIndex, 2, colIndex + 3].Merge = true;
                        sheet.Cells[3, colIndex].Value = "Hãng bay";
                        sheet.Cells[3, colIndex + 1].Value = "Chi phí";
                        sheet.Cells[3, colIndex + 2].Value = "Hành lý";
                        sheet.Cells[3, colIndex + 3].Value = "HCNS đề xuất";
                        colIndex += 4;
                    }

                    sheet.Cells[2, colIndex].Value = "Chênh lệch chi phí PA1 và PA2";
                    sheet.Cells[2, colIndex, 3, colIndex].Merge = true;
                    colIndex++;

                    sheet.Cells[2, colIndex].Value = "Tổng tiền";
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

                    // Style Headers
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

                    // Để theo dõi các nhóm gộp ô
                    int startMergeRow = row;
                    object lastEmployeeID = null;

                    var groupsList = groups.ToList();
                    for (int gIdx = 0; gIdx < groupsList.Count; gIdx++)
                    {
                        var group = groupsList[gIdx];
                        var items = group.ToList();
                        int groupCount = items.Count;
                        var first = items[0];

                        bool isSameGroup = lastEmployeeID != null && Equals(first["EmployeeID"], lastEmployeeID);

                        if (!isSameGroup && lastEmployeeID != null)
                        {
                            // Gộp ô nhóm trước đó nếu có nhiều hơn 1 dòng
                            if (row - 1 > startMergeRow)
                            {
                                int endRow = row - 1;
                                int[] colsToMerge = { 2, 5, 6, 7, bookedDateCol };
                                foreach (int col in colsToMerge)
                                {
                                    sheet.Cells[startMergeRow, col, endRow, col].Merge = true;
                                }
                            }
                            startMergeRow = row;
                        }

                        lastEmployeeID = first["EmployeeID"];

                        // Master columns (A-K)
                        sheet.Cells[row, 1].Value = stt++;
                        sheet.Cells[row, 2].Value = first["RequesterName"];
                        sheet.Cells[row, 3].Value = first["Reason"];
                        sheet.Cells[row, 4].Value = first["ProjectName"];
                        sheet.Cells[row, 5].Value = first["PassengerName"];
                        sheet.Cells[row, 6].Value = first["PositionName"];
                        sheet.Cells[row, 7].Value = first["DepartmentName"];
                        sheet.Cells[row, 8].Value = first["DepartureAddress"];
                        sheet.Cells[row, 9].Value = first["ArrivesAddress"];
                        sheet.Cells[row, 10].Value = first["DepartureTime"] != null ? ((DateTime)first["DepartureTime"]).ToString("HH:mm") : "";
                        sheet.Cells[row, 11].Value = first["DepartureDate"] != null ? ((DateTime)first["DepartureDate"]).ToString("dd/MM/yyyy") : "";

                        // Các cột PA
                        decimal pa1Price = 0;
                        decimal pa2Price = 0;
                        decimal totalApproved = 0;

                        for (int i = 0; i < groupCount; i++)
                        {
                            var item = items[i];
                            int paCol = 12 + (i * 4);
                            sheet.Cells[row, paCol].Value = item["Airline"];
                            sheet.Cells[row, paCol + 1].Value = item["Price"];
                            sheet.Cells[row, paCol + 1].Style.Numberformat.Format = "#,##0";
                            sheet.Cells[row, paCol + 2].Value = item["Baggage"];

                            bool isHCNS = item["HCNSProposal"] != null && Convert.ToBoolean(item["HCNSProposal"]);
                            if (isHCNS)
                            {
                                sheet.Cells[row, paCol + 3].Value = "✔";
                                sheet.Cells[row, paCol + 3].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                                sheet.Cells[row, paCol + 3].Style.Font.Bold = true;
                            }

                            decimal price = item["Price"] != null ? Convert.ToDecimal(item["Price"]) : 0;
                            if (i == 0) pa1Price = price;
                            if (i == 1) pa2Price = price;
                            
                            int isApprove = item["IsApprove"] != null ? Convert.ToInt32(item["IsApprove"]) : 0;
                            if (isApprove == 1) totalApproved += price;
                        }

                        // Cột chênh lệch
                        sheet.Cells[row, diffCol].Value = Math.Abs(pa1Price - pa2Price);
                        sheet.Cells[row, diffCol].Style.Numberformat.Format = "#,##0";

                        // Cột tổng tiền
                        sheet.Cells[row, totalCol].Value = totalApproved;
                        sheet.Cells[row, totalCol].Style.Numberformat.Format = "#,##0";

                        // Cột người đặt
                        sheet.Cells[row, bookerCol].Value = first["BookerName"];

                        // Cột ngày đặt
                        sheet.Cells[row, bookedDateCol].Value = first["BookedDate"] != null ? ((DateTime)first["BookedDate"]).ToString("dd/MM/yyyy HH:mm") : "";

                        // Cột ghi chú
                        sheet.Cells[row, noteCol].Value = first["Note"];

                        row++;

                        // Xử lý gộp ô cho nhóm cuối cùng
                        if (gIdx == groupsList.Count - 1)
                        {
                            if (row - 1 > startMergeRow)
                            {
                                int endRow = row - 1;
                                int[] colsToMerge = { 2, 5, 6, 7, bookedDateCol };
                                foreach (int col in colsToMerge)
                                {
                                    sheet.Cells[startMergeRow, col, endRow, col].Merge = true;
                                }
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
                    sheet.Column(2).Width = 20;  // Requester
                    sheet.Column(3).Width = 30;  // Reason
                    sheet.Column(4).Width = 20;  // Project
                    sheet.Column(5).Width = 20;  // Passenger
                    sheet.Column(6).Width = 15;  // Position
                    sheet.Column(7).Width = 20;  // Dept
                    sheet.Column(8).Width = 20;  // From
                    sheet.Column(9).Width = 20;  // To
                    sheet.Column(10).Width = 10; // Time
                    sheet.Column(11).Width = 12; // Date

                    for (int i = 12; i < diffCol; i++)
                    {
                        sheet.Column(i).Width = 15;
                    }
                    sheet.Column(diffCol).Width = 20;
                    sheet.Column(totalCol).Width = 15;
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
    }

    public class FlightBookingApproveDTO
    {
        public int ProposalID { get; set; }
        public int Status { get; set; } // 0: Chờ duyệt, 1: Duyệt, 2: Không duyệt
        public string? ReasonDecline { get; set; }
    }
}
