using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using System;
using System.Collections.Generic;

namespace RERPAPI.Controllers.HRM.HRRecruitment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HRRecruitmentSummaryController : ControllerBase
    {
        [RequiresPermission("N1,N2")]
        [HttpPost("get-summary")]
        public IActionResult GetSummary([FromBody] HRRecruitmentSummaryFilterDTO request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Request body is empty"));
                }

                DateTime? dateStart = null;
                DateTime? dateEnd = null;

                if (!string.IsNullOrEmpty(request.DateStart) && DateTime.TryParse(request.DateStart, out DateTime ds))
                {
                    dateStart = ds;
                }

                if (!string.IsNullOrEmpty(request.DateEnd) && DateTime.TryParse(request.DateEnd, out DateTime de))
                {
                    dateEnd = de;
                }

                object ds_formatted = dateStart.HasValue ? new DateTime(dateStart.Value.Year, dateStart.Value.Month, dateStart.Value.Day, 0, 0, 0) : DBNull.Value;
                object de_formatted = dateEnd.HasValue ? dateEnd.Value.Date.AddDays(1) : DBNull.Value;

                var dt = SQLHelper<object>.ProcedureToList(
                    "spGetHRRecruitmentSummaryCandidate",
                    new string[] { "@DateStart", "@DateEnd", "DepartmentID", "@IsComplete" },
                    new object[] { ds_formatted, de_formatted, request.DepartmentID, request.IsComplete }
                );

                var hiringRequests = SQLHelper<object>.GetListData(dt, 0);
                var candidates = SQLHelper<object>.GetListData(dt, 1);
                var chartData = SQLHelper<object>.GetListData(dt, 2);

                return Ok(ApiResponseFactory.Success(new {
                    HiringRequests = hiringRequests ?? new List<object>(),
                    Candidates = candidates ?? new List<object>(),
                    ChartData = chartData ?? new List<object>()
                }, "Lấy danh sách tổng hợp ứng viên thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("get-source-summary")]
        public IActionResult GetSourceSummary([FromBody] HRRecruitmentSummaryFilterDTO request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Request body is empty"));
                }

                DateTime? dateStart = null;
                DateTime? dateEnd = null;

                if (!string.IsNullOrEmpty(request.DateStart) && DateTime.TryParse(request.DateStart, out DateTime ds))
                {
                    dateStart = ds;
                }

                if (!string.IsNullOrEmpty(request.DateEnd) && DateTime.TryParse(request.DateEnd, out DateTime de))
                {
                    dateEnd = de;
                }

                object ds_formatted = dateStart.HasValue ? new DateTime(dateStart.Value.Year, dateStart.Value.Month, dateStart.Value.Day, 0, 0, 0) : DBNull.Value;
                object de_formatted = dateEnd.HasValue ? dateEnd.Value.Date.AddDays(1) : DBNull.Value;

                var dt = SQLHelper<object>.ProcedureToList(
                    "spGetHRRecruitmentSourceSummary",
                    new string[] { "@DateStart", "@DateEnd", "DepartmentID" },
                    new object[] { ds_formatted, de_formatted, request.DepartmentID }
                );

                var sourceData = SQLHelper<object>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(new
                {
                    SourceData = sourceData ?? new List<object>()
                }, "Lấy thống kê nguồn tuyển dụng thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("get-education-summary")]
        public IActionResult GetEducationSummary([FromBody] HRRecruitmentSummaryFilterDTO request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Request body is empty"));
                }

                DateTime? dateStart = null;
                DateTime? dateEnd = null;

                if (!string.IsNullOrEmpty(request.DateStart) && DateTime.TryParse(request.DateStart, out DateTime ds))
                {
                    dateStart = ds;
                }

                if (!string.IsNullOrEmpty(request.DateEnd) && DateTime.TryParse(request.DateEnd, out DateTime de))
                {
                    dateEnd = de;
                }

                object ds_formatted = dateStart.HasValue ? new DateTime(dateStart.Value.Year, dateStart.Value.Month, dateStart.Value.Day, 0, 0, 0) : DBNull.Value;
                object de_formatted = dateEnd.HasValue ? dateEnd.Value.Date.AddDays(1) : DBNull.Value;

                var dt = SQLHelper<object>.ProcedureToList(
                    "spGetHRRecruitmentEducationSummary",
                    new string[] { "@DateStart", "@DateEnd", "DepartmentID" },
                    new object[] { ds_formatted, de_formatted, request.DepartmentID }
                );

                var educationData = SQLHelper<object>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(new
                {
                    EducationData = educationData ?? new List<object>()
                }, "Lấy thống kê nguồn gốc học vấn thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel([FromBody] HRRecruitmentSummaryFilterDTO request)
        {
            try
            {
                DateTime? dateStart = null;
                DateTime? dateEnd = null;

                if (!string.IsNullOrEmpty(request.DateStart) && DateTime.TryParse(request.DateStart, out DateTime ds))
                {
                    dateStart = ds;
                }

                if (!string.IsNullOrEmpty(request.DateEnd) && DateTime.TryParse(request.DateEnd, out DateTime de))
                {
                    dateEnd = de;
                }
                ExcelPackage.License.SetNonCommercialOrganization("RTC");
                object ds_formatted = dateStart.HasValue ? new DateTime(dateStart.Value.Year, dateStart.Value.Month, dateStart.Value.Day, 0, 0, 0) : DBNull.Value;
                object de_formatted = dateEnd.HasValue ? dateEnd.Value.Date.AddDays(1) : DBNull.Value;

                var dt = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHRRecruitmentSummaryExportExcel",
                    new string[] { "@DateStart", "@DateEnd", "DepartmentID", "@IsComplete" },
                    new object[] { ds_formatted, de_formatted, request.DepartmentID, request.IsComplete }
                );

                var candidates = SQLHelper<dynamic>.GetListData(dt, 0);

                if (candidates == null || candidates.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xuất ng!"));
                }

               
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("DS Ứng Viên");
                    sheet.Cells.Style.Font.Name = "Times New Roman";
                    sheet.Cells.Style.Font.Size = 11;

                    // 1. Master Header (Row 1)
                    sheet.Cells[1, 1].Value = "THÔNG TIN ỨNG VIÊN PHỎNG VẤN";
                    sheet.Cells[1, 1, 1, 35].Merge = true;
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

                    // 2. Column Definitions & Base Headers
                    string[] baseHeaders = {
                        "STT", "TIME", "DD", "MM", "YYYY", "Vị trí",
                        "Họ tên UV", "Ngày sinh", "SĐT", "Chuyên ngành", "Trường", "Email", "Nguồn UV",
                        "Người phụ trách", "Số vòng PV"
                    };

                    // PV Vòng 1 Sub-headers
                    string[] round1SubHeaders = {
                        "Ban chuyên môn", "HR", "Đánh giá chung", "Trình độ", "Kinh nghiệm", "Ngôn ngữ giao tiếp", "Khả năng gắn bó", "Nhận xét khác", "KQV1", "Chi tiết", "Ngày nhận việc dự kiến"
                    };

                    // PV Vòng 2 Sub-headers
                    string[] round2SubHeaders = {
                        "Ban chuyên môn", "HR", "Đánh giá chung", "Trình độ", "Kinh nghiệm", "Ngôn ngữ giao tiếp", "Khả năng gắn bó", "Nhận xét"
                    };

                    // 3. Set Values & Merging (Down to Row 2 & 3)
                    // Base Columns (A-O): Merged vertically Row 2-3
                    for (int i = 0; i < baseHeaders.Length; i++)
                    {
                        var cell = sheet.Cells[2, i + 1];
                        cell.Value = baseHeaders[i];
                        sheet.Cells[2, i + 1, 3, i + 1].Merge = true;
                    }

                    // Interview Round 1 Group Header
                    sheet.Cells[2, 16].Value = "Phỏng vấn vòng 1";
                    sheet.Cells[2, 16, 2, 26].Merge = true;
                    // Sub-headers for Round 1
                    for (int i = 0; i < round1SubHeaders.Length; i++)
                    {
                        sheet.Cells[3, 16 + i].Value = round1SubHeaders[i];
                    }

                    // Interview Round 2 Group Header
                    sheet.Cells[2, 27].Value = "Phỏng vấn vòng 2";
                    sheet.Cells[2, 27, 2, 34].Merge = true;
                    // Sub-headers for Round 2
                    for (int i = 0; i < round2SubHeaders.Length; i++)
                    {
                        sheet.Cells[3, 27 + i].Value = round2SubHeaders[i];
                    }

                    // Final Column AI: Merged vertically Row 2-3
                    sheet.Cells[2, 35].Value = "KQPV";
                    sheet.Cells[2, 35, 3, 35].Merge = true;

                    // Style all Sub Headers (Row 2 & 3)
                    using (var range = sheet.Cells[2, 1, 3, 35])
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
                    // Highlight Group Headers background
                    sheet.Cells[2, 16, 2, 34].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);

                    // 4. Data Binding (Starts Row 4)
                    int row = 4;
                    foreach (var item in candidates)
                    {
                        var dataRow = item as IDictionary<string, object>;
                        if (dataRow != null)
                        {
                            sheet.Cells[row, 1].Value = dataRow.ContainsKey("STT") ? dataRow["STT"] : row - 3;
                            sheet.Cells[row, 2].Value = dataRow.ContainsKey("GioUngTuyen") ? dataRow["GioUngTuyen"] : "";
                            sheet.Cells[row, 3].Value = dataRow.ContainsKey("Ngay") ? dataRow["Ngay"] : "";
                            sheet.Cells[row, 4].Value = dataRow.ContainsKey("Thang") ? dataRow["Thang"] : "";
                            sheet.Cells[row, 5].Value = dataRow.ContainsKey("Nam") ? dataRow["Nam"] : "";
                            sheet.Cells[row, 6].Value = dataRow.ContainsKey("ViTriUngTuyen") ? dataRow["ViTriUngTuyen"] : "";
                            sheet.Cells[row, 7].Value = dataRow.ContainsKey("HoTen") ? dataRow["HoTen"] : "";
                            sheet.Cells[row, 8].Value = dataRow.ContainsKey("NgaySinh") ? dataRow["NgaySinh"] : "";
                            sheet.Cells[row, 9].Value = dataRow.ContainsKey("SDT") ? dataRow["SDT"] : "";
                            sheet.Cells[row, 10].Value = dataRow.ContainsKey("ChuyenNganh") ? dataRow["ChuyenNganh"] : "";
                            sheet.Cells[row, 11].Value = dataRow.ContainsKey("Truong") ? dataRow["Truong"] : "";
                            sheet.Cells[row, 12].Value = dataRow.ContainsKey("Email") ? dataRow["Email"] : "";
                            sheet.Cells[row, 13].Value = dataRow.ContainsKey("NguonUngVien") ? dataRow["NguonUngVien"] : "";
                            sheet.Cells[row, 14].Value = ""; 
                            sheet.Cells[row, 15].Value = dataRow.ContainsKey("SoVongPV") ? dataRow["SoVongPV"] : ""; 

                            // PV Vòng 1 (P-Z)
                            sheet.Cells[row, 16].Value = dataRow.ContainsKey("Round1_BanCM") ? dataRow["Round1_BanCM"] : "";
                            sheet.Cells[row, 17].Value = ""; 
                            sheet.Cells[row, 18].Value = dataRow.ContainsKey("Round1_Overall") ? dataRow["Round1_Overall"] : "";
                            sheet.Cells[row, 19].Value = dataRow.ContainsKey("Round1_Qualif") ? dataRow["Round1_Qualif"] : "";
                            sheet.Cells[row, 20].Value = dataRow.ContainsKey("Round1_Exp") ? dataRow["Round1_Exp"] : "";
                            sheet.Cells[row, 21].Value = dataRow.ContainsKey("Round1_Lang") ? dataRow["Round1_Lang"] : "";
                            sheet.Cells[row, 22].Value = dataRow.ContainsKey("Round1_Motiv") ? dataRow["Round1_Motiv"] : "";
                            sheet.Cells[row, 23].Value = dataRow.ContainsKey("Round1_Other") ? dataRow["Round1_Other"] : "";
                            sheet.Cells[row, 24].Value = dataRow.ContainsKey("Round1_Status") ? dataRow["Round1_Status"] : "";
                            sheet.Cells[row, 25].Value = ""; 
                            sheet.Cells[row, 26].Value = ""; 

                            // PV Vòng 2 (AA-AH)
                            sheet.Cells[row, 27].Value = dataRow.ContainsKey("Round2_BanCM") ? dataRow["Round2_BanCM"] : "";
                            sheet.Cells[row, 28].Value = ""; 
                            sheet.Cells[row, 29].Value = dataRow.ContainsKey("Round2_Overall") ? dataRow["Round2_Overall"] : "";
                            sheet.Cells[row, 30].Value = dataRow.ContainsKey("Round2_Qualif") ? dataRow["Round2_Qualif"] : "";
                            sheet.Cells[row, 31].Value = dataRow.ContainsKey("Round2_Exp") ? dataRow["Round2_Exp"] : "";
                            sheet.Cells[row, 32].Value = dataRow.ContainsKey("Round2_Lang") ? dataRow["Round2_Lang"] : "";
                            sheet.Cells[row, 33].Value = dataRow.ContainsKey("Round2_Motiv") ? dataRow["Round2_Motiv"] : "";
                            sheet.Cells[row, 34].Value = dataRow.ContainsKey("Round2_Other") ? dataRow["Round2_Other"] : "";

                            // KQPV (AI)
                            sheet.Cells[row, 35].Value = dataRow.ContainsKey("EmailPhanHoi") ? dataRow["EmailPhanHoi"] : "";

                            for (int col = 1; col <= 35; col++)
                            {
                                sheet.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }
                        }
                        row++;
                    }

                    // 5. Final Styling (Width, Alignment, WrapText)
                    var allRange = sheet.Cells[1, 1, row - 1, 35];
                    allRange.Style.Font.Name = "Times New Roman";
                    allRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    allRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    allRange.Style.WrapText = true;

                    // Manual Width Setup
                    sheet.Column(1).Width = 6;   // STT
                    sheet.Column(2).Width = 10;  // TIME
                    sheet.Column(3).Width = 5;   // DD
                    sheet.Column(4).Width = 5;   // MM
                    sheet.Column(5).Width = 8;   // YYYY
                    sheet.Column(6).Width = 25;  // Vị trí
                    sheet.Column(7).Width = 25;  // UV
                    sheet.Column(8).Width = 12;  // Ngày sinh
                    sheet.Column(9).Width = 15;  // SĐT
                    sheet.Column(10).Width = 25; // Chuyên ngành
                    sheet.Column(11).Width = 30; // Trường
                    sheet.Column(12).Width = 25; // Email
                    sheet.Column(13).Width = 20; // Nguồn
                    sheet.Column(14).Width = 20; // Người phụ trách
                    sheet.Column(15).Width = 10; // Số vòng
                    sheet.Column(23).Width = 40; // Nhận xét khác

                    for (int col = 16; col <= 35; col++)
                    {
                        if(col != 23)
                        {
                            sheet.Column(col).Width = 18; //col đánh giá pv
                        }    
                    }

                    var stream = new System.IO.MemoryStream(package.GetAsByteArray());
                    string fileName = $"Theodoiungvien_tu{dateStart?.ToString("ddMMyyyy") ?? "dau"}_den{dateEnd?.ToString("ddMMyyyy") ?? "cuoi"}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
