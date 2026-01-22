using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Document;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.BBNV;
using RERPAPI.Repo.GenericEntity.DocumentManager;
using System.Data;
using System.Globalization;




namespace RERPAPI.Controllers.DocumentManager
{
    [Route("api/[controller]")]
    [ApiController]

    public class DocumentController : ControllerBase
    {
        DocumentTypeRepo _documenttype;

        DocumentRepo _document;

        DocumentFileRepo _documentfile;

        DepartmentRepo _tsDepartment;
        private IConfiguration _configuration;

        public DocumentController(DocumentTypeRepo documenttype, DocumentRepo document, DocumentFileRepo documentfile, DepartmentRepo tsDepartment, IConfiguration configuration) 
        {
            _configuration = configuration;
            _documenttype = documenttype;
            _document = document;
            _documentfile = documentfile;
            _tsDepartment = tsDepartment;
        }

        [HttpGet("get-document-type")]
        public IActionResult GetDocumentType()
        {
            try
            {
                var documenttype = _documenttype.GetAll(x=>x.IsDeleted!=true);
                return Ok(new
                {
                    status = 1,
                    data = documenttype

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }

        [HttpPost("get-document")]
        public IActionResult GetListDocument([FromBody] DocumentRequestParam request)
        {   
            try
            {
                var document = SQLHelper<dynamic>.ProcedureToList("spGetDocument",
                    new string[] { "@IDDocumentType", "@DepartmentID" },
                    new object[] { request.IDDocumentType, request.DepartmentID });
                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        asset = SQLHelper<dynamic>.GetListData(document, 0),
                    }
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-document-file/{id}")]
        public IActionResult GetFileDocumentByID(int id)
        {
            try
            {
                List<DocumentFile> result = _documentfile.GetAll(x => x.DocumentID == id && x.IsDeleted !=true).ToList();
                return Ok(new
                {
                    status = 1,
                    data = result,
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-departments")]

        public IActionResult GetDepartment()
        {
            try
            {
                var datadepartment = _tsDepartment.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = datadepartment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-document-type")]
        public async Task<IActionResult> SaveData([FromBody] DocumentType documenttype)
        {
            try
            {
                // Check trùng mã kho
                var existed = _documenttype.GetAll()
                    .Any(x => x.Code == documenttype.Code
                           && x.ID != documenttype.ID); // loại trừ chính nó khi update

                if (existed)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Mã văn bản đã tồn tại."
                    });
                }

                if (documenttype.ID <= 0)
                {
                    await _documenttype.CreateAsync(documenttype);
                }
                else
                {
                    _documenttype.Update(documenttype);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-document")]
        public async Task<IActionResult> SaveData([FromBody] Document document)
        {
            try
            {
                // Check trùng mã kho
                var existed = _document.GetAll()
                    .Any(x => x.Code == document.Code
                           && x.ID != document.ID); // loại trừ chính nó khi update

                if (existed)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Mã văn bản đã tồn tại."
                    });
                }

                if (document.ID <= 0)
                {
                 //   var maxStt = _document.GetAll()
                 //.Where(x => x.DocumentTypeID == document.DocumentTypeID
                 //         && x.DepartmentID == document.DepartmentID)
                 //.Select(x => x.STT ?? 0)
                 //.DefaultIfEmpty(0)
                 //.Max();

                 //   document.STT = maxStt + 1;
                    await _document.CreateAsync(document);
                }
                else
                {
                    _document.Update(document);
                }

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-next-stt")]
        public IActionResult GetNextSTT(int documentTypeID, int departmentID)
        {
            try
            {
                var query = _document.GetAll()
                    .Where(x => x.DocumentTypeID == documentTypeID);

                // Nếu phòng ban khác 0 thì lọc theo phòng ban
                if (departmentID != 0)
                {
                    query = query.Where(x => x.DepartmentID == departmentID);
                }

                var maxStt = query
                    .Select(x => x.STT ?? 0)
                    .DefaultIfEmpty(0)
                    .Max();

                return Ok(new
                {
                    status = 1,
                    nextStt = maxStt + 1
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }






        [HttpPost("save-document-file")]
        public async Task<IActionResult> SaveData([FromBody] DocumentFile documentfile)
        {
            try
            {
                if (documentfile.ID <= 0) await _documentfile.CreateAsync(documentfile);
                else await _documentfile.UpdateAsync(documentfile);

                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công.",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("export-excel/{id}")]
        public IActionResult ExportExcelDocumentsByType(int id)
        {
            try
            {
                // Bước 1: Lấy dữ liệu từ stored procedure spGetDocument với filter theo IDDocumentType
                // Giả sử spGetDocument có parameter @IDDocumentType (nếu không, thêm hoặc dùng @FilterText)
                // Nếu idDocumentType = 0, coi như xuất tất cả
                object filterId = id == 0 ? 0 : id; // Hoặc dùng "" nếu là string filter

                List<List<dynamic>> documentList = SQLHelper<dynamic>.ProcedureToList(
                    "spGetDocument",
                    new string[] { "@IDDocumentType", "@DepartmentID", "@GroupType" }, 
                    new object[] { filterId, -1, 1 }
                );

                if (documentList == null || !documentList.Any() || !documentList[0].Any())
                {
                    return BadRequest("Không có dữ liệu để xuất.");
                }

                var documentData = SQLHelper<dynamic>.GetListData(documentList, 0);
                DataTable dtAll = ConvertToDataTable(documentData);

                if (dtAll == null || dtAll.Rows.Count == 0)
                {
                    return BadRequest("Không có dữ liệu để xuất.");
                }

                // Bước 2: Áp dụng sort theo DatePromulgate DESC (tương tự WinForm)
                var sortCol = "SortDatePromulgate";
                if (!dtAll.Columns.Contains(sortCol))
                {
                    dtAll.Columns.Add(sortCol, typeof(DateTime));
                }

                // Local function để parse date (sử dụng trước khi sort)
                bool TryGetDate(object v, out DateTime d)
                {
                    d = default;
                    if (v == null || v == DBNull.Value) return false;
                    if (v is DateTime dd) { d = dd; return true; }
                    var s = v.ToString();
                    return DateTime.TryParse(s, new CultureInfo("vi-VN"), DateTimeStyles.None, out d)
                        || DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
                }

                foreach (DataRow r in dtAll.Rows)
                {
                    if (TryGetDate(r["DatePromulgate"], out var d))
                    {
                        r[sortCol] = d;
                    }
                    else
                    {
                        r[sortCol] = DateTime.MinValue;
                    }
                }

                var dv = new DataView(dtAll);
                dv.Sort = $"{sortCol} DESC";
                DataTable dt = dv.ToTable();
                dt.Columns.Remove(sortCol);

                if (dt.Rows.Count == 0)
                {
                    return BadRequest("Không có dữ liệu sau khi sắp xếp.");
                }

                // Bước 3: Đường dẫn template
                ExcelPackage.License.SetNonCommercialOrganization("rtc");

                //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "BienBanBanGiao.xlsx");

                var templateFolder = _configuration.GetValue<string>("PathTemplate");

                if (string.IsNullOrWhiteSpace(templateFolder))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy đường dẫn thư mục {templateFolder} trên sever!!"));
                string templatePath = Path.Combine(templateFolder, "ExportExcel", "VBPhatHanh.xlsx");
                if (!System.IO.File.Exists(templatePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy File mẫu!"));

                // Bước 4: Mở template và điền dữ liệu với ClosedXML
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1); // Sheet đầu tiên (RTC)

                    // Điền header ngày hiện tại (C3: Ngày, E3: Tháng)
                    var now = DateTime.Now;
                    sheet.Cell("C3").Value = now.Day.ToString("00");
                    sheet.Cell("E3").Value = now.Month.ToString("00");

                    // Local function GetStr (định nghĩa lại ở đây để scope đúng)
                    string GetStr(DataRow r, string col) =>
                        dt.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : "";

                    // Điền dữ liệu từ row 5
                    int startRow = 5;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var r = dt.Rows[i];
                        int row = startRow + i;

                        // A: STT
                        sheet.Cell(row, 1).Value = i + 1;

                        // B: Code
                        sheet.Cell(row, 2).Value = GetStr(r, "Code");

                        // C: DocumentTypeCode
                        sheet.Cell(row, 3).Value = GetStr(r, "DocumentTypeCode");

                        // D: NameDocument (Tiêu đề)
                        sheet.Cell(row, 4).Value = GetStr(r, "NameDocument");

                        // E: DepartmentCode
                        sheet.Cell(row, 5).Value = GetStr(r, "DepartmentCode");

                        // F: DepartmentName
                        sheet.Cell(row, 6).Value = GetStr(r, "DepartmentName");

                        // G: EmployeeSignCode
                        sheet.Cell(row, 7).Value = GetStr(r, "EmployeeSignCode");

                        // H: EmployeeSignName
                        sheet.Cell(row, 8).Value = GetStr(r, "EmployeeSignName");

                        // I/J/K: DatePromulgate (Ngày/Tháng/Năm – cột 9/10/11)
                        if (TryGetDate(r["DatePromulgate"], out var dp))
                        {
                            sheet.Cell(row, 9).Value = dp.Day;
                            sheet.Cell(row, 10).Value = dp.Month;
                            sheet.Cell(row, 11).Value = dp.Year;
                        }

                        // L/M/N: DateEffective (cột 12/13/14)
                        if (TryGetDate(r["DateEffective"], out var de))
                        {
                            sheet.Cell(row, 12).Value = de.Day;
                            sheet.Cell(row, 13).Value = de.Month;
                            sheet.Cell(row, 14).Value = de.Year;
                        }

                        // O: AffectedScope (cột 15)
                        sheet.Cell(row, 15).Value = GetStr(r, "AffectedScope");
                    }

                    // Bước 5: Căn chỉnh (tương tự WinForm)
                    int lastRow = startRow + dt.Rows.Count - 1;
                    if (lastRow >= startRow)
                    {
                        // Center cho các cột cụ thể
                        sheet.Range($"B{startRow}:B{lastRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sheet.Range($"C{startRow}:C{lastRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sheet.Range($"F{startRow}:F{lastRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sheet.Range($"G{startRow}:G{lastRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sheet.Range($"H{startRow}:H{lastRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sheet.Range($"O{startRow}:O{lastRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Wrap text và vertical center cho toàn bộ range dữ liệu
                        sheet.Range(startRow, 1, lastRow, 15).Style.Alignment.WrapText = true;
                        sheet.Range(startRow, 1, lastRow, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }

                    // Bước 6: Insert rows nếu dữ liệu vượt quá template (giả sử 18 rows trống từ row 5-22)
                    int maxTemplateRows = 22 - startRow + 1; // 18
                    if (dt.Rows.Count > maxTemplateRows)
                    {
                        int extraRows = dt.Rows.Count - maxTemplateRows;
                        sheet.Row(startRow + maxTemplateRows - 1).InsertRowsBelow(extraRows);
                    }

                    // Bước 7: Save to stream và return file
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        var fileName = $"VBPhatHanh_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Helper method: Convert List<dynamic> sang DataTable
        private DataTable ConvertToDataTable(List<dynamic> data)
        {
            if (data == null || !data.Any()) return null;

            var table = new DataTable();
            var firstItem = (IDictionary<string, object>)data[0];
            foreach (var key in firstItem.Keys)
            {
                // Giả sử tất cả là string; nếu cần type khác, điều chỉnh dựa trên dữ liệu thực tế
                table.Columns.Add(key, typeof(string));
            }

            foreach (var item in data)
            {
                var row = table.NewRow();
                var dict = (IDictionary<string, object>)item;
                foreach (var key in dict.Keys)
                {
                    row[key] = dict[key]?.ToString() ?? string.Empty;
                }
                table.Rows.Add(row);
            }

            return table;
        }
        [HttpGet("get-document-common")]
        public IActionResult GetDocumrntCommon(string? keyword, int departID, int groupType)
        {
            try
            {
                var document = SQLHelper<dynamic>.ProcedureToList("spGetDocument",
                       new string[] { "@FilterText", "@DepartmentID", "@GroupType" },
                    new object[] { keyword??"", departID, groupType });
              var  documentList = SQLHelper<object>.GetListData(document, 0);
                return Ok(ApiResponseFactory.Success(documentList, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-document-admin-sale")]
        public IActionResult GetDocumrntCommon(int departID)
        {
            try
            {
                var document = SQLHelper<dynamic>.ProcedureToList("spGetDocumentSaleAdmin",
                     new string[] { "@GroupType", "@DepartmentID" }, new object[] { 2, departID });
                var documentList = SQLHelper<object>.GetListData(document, 0);
                return Ok(ApiResponseFactory.Success(documentList, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
