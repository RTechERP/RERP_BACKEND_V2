using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM.Visa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM.Visa
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BusinessVisaRequestController : ControllerBase
    {
        private readonly BusinessVisaRequestRepo _businessVisaRequestRepo;
        private readonly CurrentUser _currentUser;

        public BusinessVisaRequestController(
            BusinessVisaRequestRepo businessVisaRequestRepo,
            CurrentUser currentUser)
        {
            _businessVisaRequestRepo = businessVisaRequestRepo;
            _currentUser = currentUser;
        }
        //API lấy danh sách đăng ký 
        [RequiresPermission("N1,N34")]
        [HttpPost("search")]
        public IActionResult Search([FromBody] BusinessVisaRequestSearchParam param)
        {
            try
            {
                param.StartDate = param.StartDate.Value.ToLocalTime().Date;
                param.EndDate = param.EndDate.Value.ToLocalTime().Date.AddDays(+1).AddSeconds(-1);
                string procedureName = "spGetBusinessVisaRequests";
                string[] paramNames = new string[] { "@Keyword", "@StartDate", "@EndDate", "@Type", "@EmployeeID" };
                object[] paramValues = new object[] { 
                    param.Keyword ?? "", 
                    param.StartDate, 
                    param.EndDate, 
                    param.Type ?? 0, 
                    param.EmployeeID ?? 0
                };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API lưu dữ liệu
        [RequiresPermission("N1,N34")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] BusinessVisaRequest dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }

                if (dto.ID <= 0)
                {
                    await _businessVisaRequestRepo.CreateAsync(dto);
                }
                else
                {
                    var entity = _businessVisaRequestRepo.GetByID(dto.ID);
                    if (entity != null)
                    {
                        entity.Type = dto.Type;
                        entity.EmployeeID = dto.EmployeeID;
                        entity.FullName = dto.FullName;
                        entity.DateOfBirth = dto.DateOfBirth;
                        entity.Gender = dto.Gender;
                        entity.Nation = dto.Nation;
                        entity.HoChieu = dto.HoChieu;
                        entity.NgheNghiep = dto.NgheNghiep;
                        entity.CompanyName = dto.CompanyName;
                        entity.Destination = dto.Destination;
                        entity.BusinessTripFromDate = dto.BusinessTripFromDate;
                        entity.BusinessTripToDate = dto.BusinessTripToDate;
                        entity.Cost = dto.Cost;
                        entity.VisaIssueDate = dto.VisaIssueDate;
                        entity.Note = dto.Note;
                        entity.Status = dto.Status;
                        
                     
                        await _businessVisaRequestRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API lấy dữ liệu khi sửa
        [RequiresPermission("N1,N34")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var entity = await _businessVisaRequestRepo.GetByIDAsync(id);
                return Ok(ApiResponseFactory.Success(entity, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API xóa
        [RequiresPermission("N1,N34")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));
                }

                foreach (var id in ids)
                {
                    var entity = await _businessVisaRequestRepo.GetByIDAsync(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                   
                        await _businessVisaRequestRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(ids, "Xóa dữ liệu thành công"));
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
                var query = from m in _businessVisaRequestRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null)
                            select new
                            {
                                Nation = m.Nation,
                                FullName = m.FullName,
                                HoChieu = m.HoChieu,
                                NgheNghiep = m.NgheNghiep,
                                CompanyName = m.CompanyName,
                                Destination = m.Destination,
                                VisaIssueDate = m.VisaIssueDate,
                                Cost = m.Cost,
                                Status = m.Status
                            };

                var data = query.Distinct().ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("export-excel")]
        public IActionResult ExportExcel([FromBody] BusinessVisaRequestSearchParam param)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("RTC");

                param.StartDate = param.StartDate.Value.ToLocalTime().Date;
                param.EndDate = param.EndDate.Value.ToLocalTime().Date.AddDays(1).AddSeconds(-1);
                string procedureName = "spGetBusinessVisaRequestsExportExcel";
                string[] paramNames = new string[] { "@Keyword", "@StartDate", "@EndDate", "@Type", "@EmployeeID" };
                object[] paramValues = new object[] {
                    param.Keyword ?? "",
                    param.StartDate,
                    param.EndDate,
                    param.Type ?? 0,
                    param.EmployeeID ?? 0
                };

                var dt = SQLHelper<dynamic>.ProcedureToList(procedureName, paramNames, paramValues);
                var listData = SQLHelper<dynamic>.GetListData(dt, 0);

                if (listData == null || listData.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xuất Excel!"));
                }

                var allData = listData.Cast<IDictionary<string, object>>().ToList();
                var dataDoiTac = allData.Where(x => x["Type"] != null && Convert.ToInt32(x["Type"]) == 2).ToList();
                var dataCBNV = allData.Where(x => x["Type"] != null && Convert.ToInt32(x["Type"]) == 1).ToList();

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    if (dataDoiTac.Any())
                    {
                        AddVisaSheet(package, "THEO DÕI LÀM VISA CHO ĐỐI TÁC", dataDoiTac, writeDataDoiTac);
                    }

                    if (dataCBNV.Any())
                    {
                        AddVisaSheet(package, "THEO DÕI LÀM VISA CHO CBNV", dataCBNV, writeDataCBNV);
                    }

                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xuất Excel!"));
                    }

                    var stream = new System.IO.MemoryStream(package.GetAsByteArray());
                    string fileName = $"TheoDoiVisa_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Tạo sheet với tiêu đề lớn (merged full width) + dòng header cột + dữ liệu.
        /// </summary>
        private static void AddVisaSheet(
            OfficeOpenXml.ExcelPackage package,
            string sheetTitle,
            System.Collections.Generic.List<IDictionary<string, object>> data,
            System.Action<OfficeOpenXml.ExcelWorksheet, int, IDictionary<string, object>, int> writeRow)
        {
            var sheet = package.Workbook.Worksheets.Add(sheetTitle);

            // Cấu hình cột trước để có chiều rộng trước khi merge
            string[] headers = sheetTitle == "THEO DÕI LÀM VISA CHO ĐỐI TÁC"
                ? new[] { "STT", "Họ và tên", "Ngày sinh", "Giới tính", "Quốc tịch", "Số hộ chiếu", "Nghề nghiệp", "Công ty", "Điểm đến", "Thời gian xin thị thực ở Việt Nam", "Phí làm visa", "Thời gian có visa", "Tình trạng" }
                : new[] { "STT", "Họ và tên", "Ngày sinh", "Giới tính", "Số hộ chiếu", "Phòng ban", "Chức vụ", "Điểm đến", "Thời gian đi công tác", "Phí làm visa", "Thời gian có visa", "Tình trạng" };

            double[] widths = sheetTitle == "THEO DÕI LÀM VISA CHO ĐỐI TÁC"
                ? new double[] { 6, 25, 15, 10, 15, 15, 20, 35, 25, 30, 20, 15, 40 }
                : new double[] { 6, 25, 15, 10, 15, 25, 20, 25, 25, 20, 15, 40 };

            for (int i = 0; i < widths.Length; i++)
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            // Dòng 1: Tiêu đề lớn - merge toàn bộ số cột của sheet
            string firstCol = OfficeOpenXml.ExcelCellAddress.GetColumnLetter(1);
            string lastCol = OfficeOpenXml.ExcelCellAddress.GetColumnLetter(headers.Length);
            sheet.Cells[$"{firstCol}1:{lastCol}1"].Merge = true;
            sheet.Cells["A1"].Value = sheetTitle;
            sheet.Cells["A1"].Style.Font.Name = "Times New Roman";
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 16;
            sheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#1F3864"));
            sheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            sheet.Cells["A1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#D9E1F2"));
            sheet.Cells["A1"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells["A1"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells["A1"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells["A1"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Row(1).Height = 32;

            // Dòng 2: Header cột
            sheet.Row(2).Height = 45;
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[2, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Name = "Times New Roman";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 11;
                cell.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#1F3864"));
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cell.Style.WrapText = true;
                cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
            }

            // Dòng 3+: Dữ liệu
            int row = 3;
            int stt = 1;
            foreach (var item in data)
            {
                writeRow(sheet, row, item, stt++);
                row++;
            }
        }

        private static void StyleDataCell(OfficeOpenXml.ExcelRange cell, int col, bool isCostColumn)
        {
            cell.Style.Font.Size = 11;
            cell.Style.Font.Name = "Times New Roman";
            cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            cell.Style.WrapText = true;
            cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            if (isCostColumn && cell.Value != null && cell.Value.ToString() != "0")
            {
                cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                cell.Style.Font.Bold = true;
                cell.Style.Numberformat.Format = "#,##0\"đ\"";
            }
        }

        private static readonly System.Action<OfficeOpenXml.ExcelWorksheet, int, IDictionary<string, object>, int> writeDataDoiTac =
            (sheet, row, item, stt) =>
            {
                sheet.Cells[row, 1].Value = stt;
                sheet.Cells[row, 2].Value = item.ContainsKey("FullName") ? item["FullName"] : "";
                sheet.Cells[row, 3].Value = item.ContainsKey("DateOfBirth") && item["DateOfBirth"] != null ? Convert.ToDateTime(item["DateOfBirth"]).ToString("dd/MM/yyyy") : "";
                sheet.Cells[row, 4].Value = item.ContainsKey("Gender") && item["Gender"] != null ? (Convert.ToInt32(item["Gender"]) == 1 ? "Nam" : "Nữ") : "";
                sheet.Cells[row, 5].Value = item.ContainsKey("Nation") ? item["Nation"] : "";
                sheet.Cells[row, 6].Value = item.ContainsKey("HoChieu") ? item["HoChieu"] : "";
                sheet.Cells[row, 7].Value = item.ContainsKey("NgheNghiep") ? item["NgheNghiep"] : "";
                sheet.Cells[row, 8].Value = item.ContainsKey("CompanyName") ? item["CompanyName"] : "";
                sheet.Cells[row, 9].Value = item.ContainsKey("Destination") ? item["Destination"] : "";
                string timeFrom = item.ContainsKey("BusinessTripFromDate") && item["BusinessTripFromDate"] != null ? Convert.ToDateTime(item["BusinessTripFromDate"]).ToString("dd/MM/yyyy") : "";
                string timeTo = item.ContainsKey("BusinessTripToDate") && item["BusinessTripToDate"] != null ? Convert.ToDateTime(item["BusinessTripToDate"]).ToString("dd/MM/yyyy") : "";
                sheet.Cells[row, 10].Value = (!string.IsNullOrEmpty(timeFrom) || !string.IsNullOrEmpty(timeTo)) ? $"{timeFrom} - {timeTo}" : "";
                sheet.Cells[row, 11].Value = item.ContainsKey("Cost") && item["Cost"] != null ? item["Cost"] : 0;
                sheet.Cells[row, 12].Value = item.ContainsKey("VisaIssueDate") && item["VisaIssueDate"] != null ? item["VisaIssueDate"].ToString() : "";
                sheet.Cells[row, 13].Value = item.ContainsKey("Status") ? item["Status"] : "";

                for (int col = 1; col <= 13; col++)
                {
                    StyleDataCell(sheet.Cells[row, col], col, isCostColumn: col == 11);
                }
            };

        private static readonly System.Action<OfficeOpenXml.ExcelWorksheet, int, IDictionary<string, object>, int> writeDataCBNV =
            (sheet, row, item, stt) =>
            {
                sheet.Cells[row, 1].Value = stt;
                sheet.Cells[row, 2].Value = item.ContainsKey("EmployeeName") && item["EmployeeName"] != null ? item["EmployeeName"] : (item.ContainsKey("FullName") ? item["FullName"] : "");
                sheet.Cells[row, 3].Value = item.ContainsKey("DateOfBirth") && item["DateOfBirth"] != null ? Convert.ToDateTime(item["DateOfBirth"]).ToString("dd/MM/yyyy") : "";
                sheet.Cells[row, 4].Value = item.ContainsKey("Gender") && item["Gender"] != null ? (Convert.ToInt32(item["Gender"]) == 1 ? "Nam" : "Nữ") : "";
                sheet.Cells[row, 5].Value = item.ContainsKey("HoChieu") ? item["HoChieu"] : "";
                sheet.Cells[row, 6].Value = item.ContainsKey("DepartmentName") ? item["DepartmentName"] : "";
                sheet.Cells[row, 7].Value = item.ContainsKey("PositionName") ? item["PositionName"] : "";
                sheet.Cells[row, 8].Value = item.ContainsKey("Destination") ? item["Destination"] : "";
                string timeFrom = item.ContainsKey("BusinessTripFromDate") && item["BusinessTripFromDate"] != null ? Convert.ToDateTime(item["BusinessTripFromDate"]).ToString("dd/MM/yyyy") : "";
                string timeTo = item.ContainsKey("BusinessTripToDate") && item["BusinessTripToDate"] != null ? Convert.ToDateTime(item["BusinessTripToDate"]).ToString("dd/MM/yyyy") : "";
                sheet.Cells[row, 9].Value = (!string.IsNullOrEmpty(timeFrom) || !string.IsNullOrEmpty(timeTo)) ? $"{timeFrom} - {timeTo}" : "";
                sheet.Cells[row, 10].Value = item.ContainsKey("Cost") && item["Cost"] != null ? item["Cost"] : 0;
                sheet.Cells[row, 11].Value = item.ContainsKey("VisaIssueDate") && item["VisaIssueDate"] != null ? item["VisaIssueDate"].ToString() : "";
                sheet.Cells[row, 12].Value = item.ContainsKey("Status") ? item["Status"] : "";

                for (int col = 1; col <= 12; col++)
                {
                    StyleDataCell(sheet.Cells[row, col], col, isCostColumn: col == 10);
                }
            };
    }
}
