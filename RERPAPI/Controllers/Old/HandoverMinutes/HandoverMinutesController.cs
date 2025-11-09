using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.HandoverMinutes
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandoverMinutesController : ControllerBase
    {
        private readonly HandoverMinutesRepo _handoverMinutesRepo;
        private readonly HandoverMinutesDetailRepo _handoverMinutesDetailRepo;

        public HandoverMinutesController(
            HandoverMinutesRepo handoverMinutesRepo,
            HandoverMinutesDetailRepo handoverMinutesDetailRepo)
        {
            _handoverMinutesRepo = handoverMinutesRepo;
            _handoverMinutesDetailRepo = handoverMinutesDetailRepo;
        }
        [HttpGet]
        public IActionResult Get(DateTime dateStart, DateTime dateEnd, string keyWords = "")
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetAllHandoverMinutes", new string[] { "@DateStart", "@DateEnd", "@KeyWords" }, new object[] { dateStart, dateEnd, keyWords });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-details")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetHanoverMinutesDetail", new string[] { "@HandoverMinutesID" }, new object[] { id });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("export-excel/{id}")]
        public IActionResult ExportExcel(int id)
        {
            try
            {

                DateTime start = new DateTime(2000, 1, 1);
                DateTime end = new DateTime(2099, 12, 31);
                List<List<dynamic>> handoverList = SQLHelper<dynamic>.ProcedureToList(
                    "spGetAllHandoverMinutes",
                    new string[] { "@DateStart", "@DateEnd", "@KeyWords" },
                    new object[] { start, end, "" }
                );

                if (handoverList == null || !handoverList.Any() || !handoverList[0].Any())
                {
                    throw new Exception("Không có dữ liệu từ spGetAllHandoverMinutes");
                }

                var handoverData = SQLHelper<dynamic>.GetListData(handoverList, 0).FirstOrDefault(h => h.ID == id);
                if (handoverData == null)
                {
                    throw new Exception($"Không tìm thấy biên bản bàn giao với ID {id}");
                }

                // Lấy chi tiết biên bản
                List<List<dynamic>> detailList = SQLHelper<dynamic>.ProcedureToList(
                    "spGetHanoverMinutesDetail",
                    new string[] { "@HandoverMinutesID" },
                    new object[] { id }
                );
                var detailData = SQLHelper<dynamic>.GetListData(detailList, 0);
                if (!detailData.Any())
                {
                    throw new Exception("Không có dữ liệu chi tiết để xuất");
                }

                // Đường dẫn mẫu Excel
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "BBBGTemplate.xlsx");
                if (!System.IO.File.Exists(templatePath))
                {
                    throw new Exception("Không tìm thấy file mẫu Excel");
                }

                // Mở file Excel mẫu
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var sheet = workbook.Worksheet(1);

                    // Điền thông tin header với kiểm tra null
                    DateTime dateMinutes = handoverData.DateMinutes != null ? Convert.ToDateTime(handoverData.DateMinutes) : DateTime.Now;
                    sheet.Cell("B14").Value = $"Hà Nội, ngày {dateMinutes:dd}, tháng {dateMinutes:MM}, năm {dateMinutes:yyyy}";
                    sheet.Cell("C16").Value = handoverData.CustomerName?.ToString() ?? "";
                    sheet.Cell("C17").Value = handoverData.CustomerAddress?.ToString() ?? "";
                    sheet.Cell("C18").Value = handoverData.CustomerContact?.ToString() ?? "";
                    sheet.Cell("C19").Value = handoverData.CustomerPhone != null ? $"'{handoverData.CustomerPhone}" : "";
                    sheet.Cell("I16").Value = handoverData.FullName?.ToString() ?? "";
                    sheet.Cell("I17").Value = handoverData.DepartmentName?.ToString() ?? "";
                    sheet.Cell("I18").Value = handoverData.EmailCaNhan?.ToString() ?? "";
                    sheet.Cell("I19").Value = handoverData.SDTCaNhan != null ? $"'{handoverData.SDTCaNhan}" : "";

                    // Điền chi tiết sản phẩm
                    int startRow = 22;

                    // Nếu có nhiều hơn 1 sản phẩm thì insert thêm dòng bên dưới dòng 22
                    if (detailData.Count > 2)
                    {
                        sheet.Row(startRow + 1).InsertRowsBelow(detailData.Count - 2);
                    }

                    for (int i = 0; i < detailData.Count; i++)
                    {
                        var rowData = (IDictionary<string, object>)detailData[i];
                        int rowIdx = startRow + i;

                        sheet.Cell(rowIdx, 2).Value = i + 1; // STT
                        sheet.Cell(rowIdx, 3).Value = rowData.ContainsKey("POCode") ? rowData["POCode"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 4).Value = rowData.ContainsKey("ProductName") ? rowData["ProductName"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 5).Value = rowData.ContainsKey("ProductCode") ? rowData["ProductCode"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 6).Value = rowData.ContainsKey("Maker") ? rowData["Maker"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 7).Value = rowData.ContainsKey("Quantity") ? Convert.ToInt32(rowData["Quantity"] ?? 0) : 0;
                        sheet.Cell(rowIdx, 8).Value = rowData.ContainsKey("Unit") ? rowData["Unit"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 9).Value = rowData.ContainsKey("ProductStatus") && int.TryParse(rowData["ProductStatus"]?.ToString(), out int productStatus) ? GetProductStatusDescription(productStatus) : "";
                        sheet.Cell(rowIdx, 10).Value = rowData.ContainsKey("Guarantee") ? rowData["Guarantee"]?.ToString() ?? "" : "";
                        sheet.Cell(rowIdx, 11).Value = rowData.ContainsKey("DeliveryStatus") && int.TryParse(rowData["DeliveryStatus"]?.ToString(), out int deliveryStatus) ? GetDeliveryStatusDescription(deliveryStatus) : "";

                        // Căn chỉnh
                        for (int j = 2; j <= 11; j++)
                        {
                            var cell = sheet.Cell(rowIdx, j);
                            var cellValue = cell.Value.ToString();
                            if (!double.TryParse(cellValue, out _))
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            }
                        }
                    }


                    // Điền thông tin xác nhận
                    int finalRow = startRow + detailData.Count;
                    sheet.Cell(finalRow + 7, 4).Value = handoverData.AdminWarehouseName?.ToString() ?? "";
                    sheet.Cell(finalRow + 7, 6).Value = handoverData.FullName?.ToString() ?? "";
                    var receiverCell = sheet.Cell(finalRow + 7, 11); 
                    receiverCell.Value = $"{handoverData.Receiver?.ToString() ?? ""}-{handoverData.ReceiverPhone?.ToString() ?? ""}";

                    if (detailData.Count == 1)
                    {
                        // Nếu chỉ có 1 dòng chi tiết, kiểm tra xem có dòng thừa nào cần xóa không
                        // Ví dụ: Chỉ xóa nếu dòng finalRow không chứa nội dung cố định
                        var rowToCheck = sheet.Row(finalRow);
                        if (rowToCheck.IsEmpty())
                        {
                            rowToCheck.Delete();
                        }
                    }

                    // Lưu file vào stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        var fileName = $"{handoverData.Code?.ToString() ?? "Unknown"}_{DateTime.Now:yy-MM-dd}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private string GetProductStatusDescription(int status)
        {
            return status switch
            {
                1 => "Mới",
                2 => "Cũ",
                _ => "Không xác định"
            };
        }

        private string GetDeliveryStatusDescription(int status)
        {
            return status switch
            {
                1 => "Nhận đủ",
                2 => "Thiếu",
                _ => "Không xác định"
            };
        }
    }
}


