using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Net.WebSockets;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.RequestInvoice
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestInvoiceDetailController : ControllerBase
    {
        private readonly string _uploadPath;
        RequestInvoiceRepo _requestInvoiceRepo = new RequestInvoiceRepo();
        RequestInvoiceDetailRepo _requestInvoiceDetailRepo = new RequestInvoiceDetailRepo();
        RequestInvoiceFileRepo _requestInvoiceFileRepo = new RequestInvoiceFileRepo();
        EmployeeRepo _employeeRepo = new EmployeeRepo();
        ProductSaleRepo _productSaleRepo = new ProductSaleRepo();
        ProjectRepo _projectRepo = new ProjectRepo();
        public RequestInvoiceDetailController(IWebHostEnvironment environment)
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "RequestInvoice");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        [HttpGet("get-employee")]
        public IActionResult GetEmployee()
        {
            try
            {
                var data = _employeeRepo.GetAll().Where(x => x.Status == 0 && x.FullName != "").ToList();
                return Ok(new
                {
                    status = 1,
                    data,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-productsale")]
        public IActionResult GetProductSale()
        {
            try
            {
                var data = _productSaleRepo.GetAll().ToList();
                return Ok(new
                {
                    status = 1,
                    data,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("get-project")]
        public IActionResult GetProject()
        {
            try
            {
                var data = _projectRepo.GetAll().OrderByDescending(x=>x.CreatedDate).ToList();
                return Ok(new
                {
                    status = 1,
                    data,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("generate-bill-number")]
        public IActionResult GenerateBillNumber(int? requestInvoiceId = 0)
        {
            try
            {
                var now = DateTime.Now;
                string month = now.ToString("MM");
                string day = now.ToString("dd");
                string year = now.ToString("yy");
                string date = year + month + day;

                string billCode = _requestInvoiceRepo.GetLatestCodeByDate(now);

                if (requestInvoiceId == 0)
                {
                    if (string.IsNullOrEmpty(billCode))
                    {
                        return Ok(new { status = 1, data = $"YCXHD{date}001" });
                    }
                    else
                    {
                        int number = int.Parse(billCode.Substring(billCode.Length - 3));
                        string dem = (number + 1).ToString().PadLeft(3, '0');
                        return Ok(new { status = 1, data = $"YCXHD{date}{dem}" });
                    }
                }
                return Ok(new { status = 1, data = billCode });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> Save([FromBody] RequestInvoiceDetailDTO dto)
        {
            try
            {
                if (dto.RequestInvoices.IsDeleted == true)
                {
                    var updatedRequestInvoice = new Model.Entities.RequestInvoice
                    {
                        ID = dto.RequestInvoices.ID,
                        IsDeleted = dto.RequestInvoices.IsDeleted
                    };
                    _requestInvoiceRepo.UpdateFieldsByID(dto.RequestInvoices.ID, updatedRequestInvoice);
                    return Ok(new
                    {
                        status = 1,
                        message = "Đã xóa thành công",
                        data = new { id = dto.RequestInvoices.ID }
                    });
                }
                if (dto.RequestInvoices.CustomerID == null || dto.RequestInvoices.CustomerID <= 0)
                {
                    return Ok(new { status = 0, message = "Xin hãy chọn khách hàng." });
                }
                if (dto.RequestInvoices.Status == null || dto.RequestInvoices.Status < 0)
                {
                    return Ok(new { status = 0, message = "Xin hãy chọn trạng thái." });
                }
                if (dto.RequestInvoices.EmployeeRequestID == null || dto.RequestInvoices.EmployeeRequestID < 0)
                {
                    return Ok(new { status = 0, message = "Xin hãy chọn người yêu cầu." });
                }
                if (dto.RequestInvoiceDetails.Count == 0 || dto.RequestInvoiceDetails == null)
                {
                    return Ok(new { status = 0, message = "Vui lòng thêm ít nhất 1 sản phẩm" });
                }
                foreach (var item in dto.RequestInvoiceDetails)
                {
                    if (item.ProductSaleID == 0 || item.ProductSaleID == null)
                    {
                        return Ok(new { status = 0, message = "Vui lòng chọn sản phẩm cho dòng còn thiếu!" });
                    }
                    if (item.ProjectID == 0 || item.ProjectID == null)
                    {
                        return Ok(new { status = 0, message = "Vui lòng chọn dự án cho dòng còn thiếu!" });
                    }
                }

                // Kiểm tra trùng mã phiếu
                var code = dto.RequestInvoices.Code.Trim();
                int id = dto.RequestInvoices.ID;
                var exist = _requestInvoiceRepo.GetAll()
                    .Where(x => x.Code == code && x.ID != id)
                    .FirstOrDefault();
                if (exist != null)
                {
                    return Ok(new { status = 0, message = "Số phiếu này đã tồn tại!" });
                }

                if (dto.RequestInvoices.ID <= 0)
                {
                    await _requestInvoiceRepo.CreateAsync(dto.RequestInvoices);
                }
                else
                {
                    _requestInvoiceRepo.UpdateFieldsByID(dto.RequestInvoices.ID, dto.RequestInvoices);
                }
                if (dto.DeletedDetailIds != null && dto.DeletedDetailIds.Count > 0)
                {
                    foreach (var item in dto.DeletedDetailIds)
                    {
                        var detailToDelete = _requestInvoiceDetailRepo.GetByID(item);
                        if (detailToDelete != null)
                        {
                            detailToDelete.IsDeleted = true;
                            //detailToDelete.UpdatedBy = User.Identity.Name; // Mở comment nếu có phân quyền người dùng
                            detailToDelete.UpdatedDate = DateTime.Now;
                            _requestInvoiceDetailRepo.UpdateFieldsByID(item, detailToDelete);
                        }
                    }
                }
                if (dto.RequestInvoiceDetails.Count > 0)
                {
                    foreach (var item in dto.RequestInvoiceDetails)
                    {
                        if (item.ID <= 0)
                        {
                            item.RequestInvoiceID = dto.RequestInvoices.ID;
                            await _requestInvoiceDetailRepo.CreateAsync(item);
                        }
                        else
                        {
                            _requestInvoiceDetailRepo.UpdateFieldsByID(item.ID, item);
                        }
                    }
                }
                return Ok(new
                {
                    status = 1,
                    message = "Success",
                    data = new { id = dto.RequestInvoices.ID }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        #region Hàm xử lí File và lưu bảng RequestInvoiceFile
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(int RequestInvoiceID, [FromForm] List<IFormFile> files)
        {
            try
            {
                var ri = _requestInvoiceRepo.GetByID(RequestInvoiceID);
                if (ri == null)
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "RequestInvoice not found"
                    });
                }

                // Tạo thư mục local cho file
                string pathPattern = $"RI{ri.ID}";
                string pathUpload = Path.Combine(_uploadPath, pathPattern);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(pathUpload))
                {
                    Directory.CreateDirectory(pathUpload);
                }

                var processedFile = new List<RequestInvoiceFile>();

                // Lưu từng file vào thư mục local
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string filePath = Path.Combine(pathUpload, file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var fileRI = new RequestInvoiceFile
                        {
                            RequestInvoiceID = ri.ID,
                            FileName = file.FileName,
                            OriginPath = pathUpload,
                            ServerPath = pathUpload,
                            IsDeleted = false,
                            CreatedBy = User.Identity?.Name ?? "System",
                            CreatedDate = DateTime.Now,
                            UpdatedBy = User.Identity?.Name ?? "System",
                            UpdatedDate = DateTime.Now
                        };

                        await _requestInvoiceFileRepo.CreateAsync(fileRI);
                        processedFile.Add(fileRI);
                    }
                }

                return Ok(new
                {
                    status = 1,
                    message = $"{processedFile.Count} tệp đã được tải lên thành công",
                    data = processedFile
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("delete-file")]
        public IActionResult DeleteFile([FromBody] List<int> fileIds)
        {
            if (fileIds == null || !fileIds.Any())
                return BadRequest(new { success = false, message = "Danh sách file ID không được trống" });

            try
            {
                var results = new List<object>();

                foreach (var fileId in fileIds)
                {
                    var file = _requestInvoiceFileRepo.GetByID(fileId);

                    // Cập nhật database
                    file.IsDeleted = true;
                    //file.UpdatedBy = User.Identity?.Name ?? "System";
                    //file.UpdatedDate = DateTime.UtcNow;
                    _requestInvoiceFileRepo.UpdateFieldsByID(file.ID, file);

                    // Xóa file vật lý
                    var physicalPath = Path.Combine(file.ServerPath, file.FileName);
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);

                    results.Add(new { fileId, success = true, message = "Xóa thành công" });
                }

                return Ok(new { success = true, results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        #endregion

    }
}
