using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Controllers.CRM;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.RequestInvoice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestInvoiceSummaryController : ControllerBase
    {
        private readonly CustomerRepo _customerRepo;
        private readonly RequestInvoiceRepo _requestInvoiceRepo;
        private readonly RequestInvoiceFileRepo _requestInvoiceFileRepo;
        private readonly POKHFilesRepo _pokhFilesRepo;
        public RequestInvoiceSummaryController(CustomerRepo customerRepo, RequestInvoiceRepo requestInvoiceRepo, RequestInvoiceFileRepo requestInvoiceFileRepo, POKHFilesRepo pokhFilesRepo)
        {
            _customerRepo = customerRepo;
            _requestInvoiceRepo = requestInvoiceRepo;
            _requestInvoiceFileRepo = requestInvoiceFileRepo;
            _pokhFilesRepo = pokhFilesRepo;
        }
        [HttpGet("get-request-invoice-summary")]
        public IActionResult GetEmployee(DateTime dateStart, DateTime dateEnd, int customerId, int userId, int status, string keyWords = "")
        {
            try
            {
                var data1 = SQLHelper<dynamic>.ProcedureToList("spGetRequestInvoiceSummary",
                                                new string[] { "@DateStart", "@DateEnd", "@Keywords", "@CustomerID", "@UserID", "@Status" },
                                                new object[] { dateStart, dateEnd, keyWords, customerId, userId, status });
                var data = SQLHelper<dynamic>.GetListData(data1, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-customer")]
        public IActionResult GetCustomer()
        {
            try
            {
                var data = _customerRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpPost("download-batch-files")]
        [Authorize]
        public IActionResult DownloadBatchFiles([FromBody] List<BatchDownloadDto> payload)
        {
            try
            {
                if (payload == null || !payload.Any())
                    return Ok(new { status = 0, message = "Không có hóa đơn nào hợp lệ để tải." });

                // Thư mục root chứa file
                string baseDestPath = @"\\192.168.1.190\Software\ftp\Upload\Hóa đơn đầu ra 2026";
                if (!Directory.Exists(baseDestPath))
                {
                    Directory.CreateDirectory(baseDestPath);
                }

                foreach (var item in payload)
                {
                    string companyName = string.IsNullOrEmpty(item.CompanyText) ? "Unknown_Company" : item.CompanyText;
                    string invoiceNum = string.IsNullOrEmpty(item.InvoiceNumber) ? "Unknown_Invoice" : item.InvoiceNumber;

                    // Tạo folder theo Công ty -> Số Hóa Đơn
                    string invoiceFolder = Path.Combine(baseDestPath, companyName, invoiceNum);
                    if (!Directory.Exists(invoiceFolder)) Directory.CreateDirectory(invoiceFolder);

                    // Tạo 2 thư mục con bên trong folder Hóa Đơn
                    string ycXhdFolder = Path.Combine(invoiceFolder, "FileYCXHD");
                    string poFolder = Path.Combine(invoiceFolder, "FilePO");

                    if (!Directory.Exists(ycXhdFolder)) Directory.CreateDirectory(ycXhdFolder);
                    if (!Directory.Exists(poFolder)) Directory.CreateDirectory(poFolder);

                    // Truy xuất DB và tải File Yêu Cầu Xuất Hóa Đơn
                    var ycxHdfiles = _requestInvoiceFileRepo.GetAll(f => f.RequestInvoiceID == item.RequestInvoiceID).ToList();
                    foreach (var f in ycxHdfiles)
                    {
                        if (System.IO.File.Exists(f.ServerPath))
                        {
                            System.IO.File.Copy(f.ServerPath, Path.Combine(ycXhdFolder, f.FileName), true);
                        }
                    }

                    // Truy xuất DB và tải File PO (nếu có POKHId)
                    if (item.POKHId.HasValue && item.POKHId.Value > 0)
                    {
                        var poFiles = _pokhFilesRepo.GetAll(p => p.POKHID == item.POKHId.Value).ToList();
                        foreach (var pf in poFiles)
                        {
                            if (System.IO.File.Exists(pf.ServerPath))
                            {
                                System.IO.File.Copy(pf.ServerPath, Path.Combine(poFolder, pf.FileName), true);
                            }
                        }
                    }
                }

                return Ok(new { status = 1, message = "Đã xuất và lưu file thành công vào máy chủ mạng." });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 0, message = $"Lỗi tải file hàng loạt: {ex.Message}" });
            }
        }


        public class BatchDownloadDto
        {
            public int RequestInvoiceID { get; set; }
            public int? POKHId { get; set; }
            public string CompanyText { get; set; }
            public string InvoiceNumber { get; set; }
        }

    }
}
