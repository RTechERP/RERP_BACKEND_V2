using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Controllers.CRM;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
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
        private readonly ConfigSystemRepo _configSystemRepo;
        public RequestInvoiceSummaryController(CustomerRepo customerRepo, RequestInvoiceRepo requestInvoiceRepo, RequestInvoiceFileRepo requestInvoiceFileRepo, POKHFilesRepo pokhFilesRepo, ConfigSystemRepo configSystemRepo)
        {
            _customerRepo = customerRepo;
            _requestInvoiceRepo = requestInvoiceRepo;
            _requestInvoiceFileRepo = requestInvoiceFileRepo;
            _pokhFilesRepo = pokhFilesRepo;
            _configSystemRepo = configSystemRepo;
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
        public IActionResult DownloadBatchFiles([FromBody] List<RequestInvoiceSummaryFilesDownloadDTO> payload)
        {
            try
            {
                if (payload == null || !payload.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu để tải file"));
                int currentYear = DateTime.Now.Year;

                string key = "RequestInvoiceSummaryFiles";
                var rootPath = _configSystemRepo.GetUploadPathByKey(key);

                if (string.IsNullOrWhiteSpace(rootPath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));
                }

                // Thư mục root chứa file
                //string baseDestPath = $@"\\192.168.1.190\Software\ftp\Upload\Hóa đơn đầu ra {currentYear}";
                string baseDestPath = Path.Combine(rootPath, $"Hóa đơn đầu ra {currentYear}");

                foreach (var item in payload)
                {
                    string companyName = string.IsNullOrEmpty(item.CompanyText) ? "Unknown_Company" : item.CompanyText;
                    string invoiceNum = string.IsNullOrEmpty(item.InvoiceNumber) ? "Unknown_Invoice" : item.InvoiceNumber;

                    // Tạo folder theo Công ty -> Số Hóa Đơn
                    string invoiceFolder = Path.Combine(baseDestPath, companyName, invoiceNum);
                    if (!Directory.Exists(invoiceFolder))
                        Directory.CreateDirectory(invoiceFolder);

                    // Truy xuất DB và tải File Yêu Cầu Xuất Hóa Đơn
                    var ycxHdfiles = _requestInvoiceFileRepo.GetAll(f => f.RequestInvoiceID == item.RequestInvoiceID).ToList();
                    foreach (var f in ycxHdfiles)
                    {
                        var fullPath = Path.Combine(f.ServerPath, f.FileName);

                        if (System.IO.File.Exists(fullPath))
                        {
                            var destPath = Path.Combine(invoiceFolder, f.FileName);

                            System.IO.File.Copy(fullPath, destPath, true);
                        }
                        else
                        {
                            Console.WriteLine($"Không tìm thấy file: {fullPath}");
                        }
                    }

                    // Truy xuất DB và tải File PO (nếu có POKHId)
                    if (item.POKHId.HasValue && item.POKHId.Value > 0)
                    {
                        var poFiles = _pokhFilesRepo.GetAll(p => p.POKHID == item.POKHId.Value).ToList();
                        foreach (var pf in poFiles)
                        {
                            var fullPath = Path.Combine(pf.ServerPath, pf.FileName);

                            if (System.IO.File.Exists(fullPath))
                            {
                                var destPath = Path.Combine(invoiceFolder, pf.FileName);

                                System.IO.File.Copy(fullPath, destPath, true);
                            }
                            else
                            {
                                Console.WriteLine($"Không tìm thấy file: {fullPath}");
                            }
                        }
                    }
                }

                //return Ok(new { status = 1, message = "Đã xuất và lưu file thành công vào máy chủ." });
                return Ok(ApiResponseFactory.Success(null, $"Lưu file thành công"));
            }
            catch (Exception ex)
            {
                //return Ok(new { status = 0, message = $"Lỗi tải file hàng loạt: {ex.Message}" });
                return BadRequest(ApiResponseFactory.Fail(null, $"Lỗi tải file hàng loạt: {ex.Message}"));
            }
        }

    }
}
