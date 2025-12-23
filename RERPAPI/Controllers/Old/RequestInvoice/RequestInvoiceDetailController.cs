using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.RequestInvoice
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestInvoiceDetailController : ControllerBase
    {
        //private readonly string _uploadPath;
        private readonly RequestInvoiceRepo _requestInvoiceRepo;
        private readonly RequestInvoiceDetailRepo _requestInvoiceDetailRepo;
        private readonly RequestInvoiceFileRepo _requestInvoiceFileRepo;
        private readonly RequestInvoiceStatusLinkRepo _requestInvoiceStatusLinkRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly ProductSaleRepo _productSaleRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        

        public RequestInvoiceDetailController(
            RequestInvoiceRepo requestInvoiceRepo,
            RequestInvoiceDetailRepo requestInvoiceDetailRepo,
            RequestInvoiceFileRepo requestInvoiceFileRepo,
            EmployeeRepo employeeRepo,
            ProductSaleRepo productSaleRepo,
            ProjectRepo projectRepo,
            ConfigSystemRepo configSystemRepo,
            RequestInvoiceStatusLinkRepo requestInvoiceStatusLinkRepo

            )
        {
            //_uploadPath = Path.Combine(environment.ContentRootPath, "Uploads", "RequestInvoice");
            //if (!Directory.Exists(_uploadPath))
            //{
            //    Directory.CreateDirectory(_uploadPath);
            //}
            _requestInvoiceRepo = requestInvoiceRepo;
            _requestInvoiceDetailRepo = requestInvoiceDetailRepo;
            _requestInvoiceFileRepo = requestInvoiceFileRepo;
            _employeeRepo = employeeRepo;
            _productSaleRepo = productSaleRepo;
            _projectRepo = projectRepo;
            _configSystemRepo = configSystemRepo;
            _requestInvoiceStatusLinkRepo = requestInvoiceStatusLinkRepo;
        }

        [HttpGet("get-employee")]
        public IActionResult GetEmployee()
        {
            try
            {
          
                var data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] {"@Status" },
                                                new object[] {0 });
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-productsale")]
        public IActionResult GetProductSale()
        {
            try
            {
                var data = _productSaleRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-project")]
        public IActionResult GetProject()
        {
            try
            {
                var data = _projectRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                        //return Ok(new { status = 1, data = $"YCXHD{date}001" });
                        return Ok(ApiResponseFactory.Success($"YCXHD{date}001", ""));
                    }
                    else
                    {
                        int number = int.Parse(billCode.Substring(billCode.Length - 3));
                        string dem = (number + 1).ToString().PadLeft(3, '0');
                        //return Ok(new { status = 1, data = $"YCXHD{date}{dem}" });
                        return Ok(ApiResponseFactory.Success($"YCXHD{date}{dem}", ""));
                    }
                }
                return Ok(ApiResponseFactory.Success(billCode, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                    await _requestInvoiceRepo.UpdateAsync(updatedRequestInvoice);
                    //return Ok(new
                    //{
                    //    status = 1,
                    //    message = "Đã xóa thành công",
                    //    data = new { id = dto.RequestInvoices.ID }
                    //});
                    return Ok(ApiResponseFactory.Success(new { id = dto.RequestInvoices.ID }, ""));
                }
                if (dto.RequestInvoices.CustomerID == null || dto.RequestInvoices.CustomerID <= 0)
                {
                    throw new Exception("Xin hãy chọn khách hàng.");
                }
                if (dto.RequestInvoices.Status == null || dto.RequestInvoices.Status < 0)
                {
                    throw new Exception("Xin hãy chọn trạng thái.");
                }
                if (dto.RequestInvoices.EmployeeRequestID == null || dto.RequestInvoices.EmployeeRequestID < 0)
                {
                    throw new Exception("Xin hãy chọn người yêu cầu.");
                }
                if (dto.RequestInvoiceDetails.Count == 0 || dto.RequestInvoiceDetails == null)
                {
                    throw new Exception("Vui lòng thêm ít nhất 1 sản phẩm");
                }
                foreach (var item in dto.RequestInvoiceDetails)
                {
                    if (item.ProductSaleID == 0 || item.ProductSaleID == null)
                    {
                        throw new Exception("Vui lòng chọn sản phẩm cho dòng còn thiếu!");
                    }
                    if (item.ProjectID == 0 || item.ProjectID == null)
                    {
                        throw new Exception("Vui lòng chọn dự án cho dòng còn thiếu!");
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
                    throw new Exception("Số phiếu này đã tồn tại!");
                }

                if (dto.RequestInvoices.ID <= 0)
                {
                    await _requestInvoiceRepo.CreateAsync(dto.RequestInvoices);

                    RequestInvoiceStatusLink statusLinkModelDefault = new RequestInvoiceStatusLink();
                    statusLinkModelDefault.RequestInvoiceID = dto.RequestInvoices.ID;
                    statusLinkModelDefault.StatusID = 1;
                    statusLinkModelDefault.IsApproved = 1;
                    statusLinkModelDefault.IsCurrent = true;
                    _requestInvoiceStatusLinkRepo.Create(statusLinkModelDefault);
                }
                else
                {
                    await _requestInvoiceRepo.UpdateAsync(dto.RequestInvoices);
                }
                if (dto.DeletedDetailIds != null && dto.DeletedDetailIds.Count > 0)
                {
                    foreach (var item in dto.DeletedDetailIds)
                    {
                        var detailToDelete = _requestInvoiceDetailRepo.GetByID(item);
                        if (detailToDelete != null)
                        {
                            //detailToDelete.IsDeleted = true;
                            //detailToDelete.UpdatedBy = User.Identity.Name; // Mở comment nếu có phân quyền người dùng
                            detailToDelete.UpdatedDate = DateTime.Now;
                            await _requestInvoiceDetailRepo.UpdateAsync(detailToDelete);
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
                            await _requestInvoiceDetailRepo.UpdateAsync(item);
                        }
                    }
                }
                //return Ok(new
                //{
                //    status = 1,
                //    message = "Success",
                //    data = new { id = dto.RequestInvoices.ID }
                //});
                return Ok(ApiResponseFactory.Success(new { id = dto.RequestInvoices.ID }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #region Hàm xử lí File và lưu bảng RequestInvoiceFile
        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload(int RequestInvoiceID, [FromForm] List<IFormFile> files)
        //{
        //    try
        //    {
        //        var ri = _requestInvoiceRepo.GetByID(RequestInvoiceID);
        //        if (ri == null)
        //        {
        //            throw new Exception("RequestInvoice not found");
        //        }

        //        // Tạo thư mục local cho file
        //        string pathPattern = $"RI{ri.ID}";
        //        string pathUpload = Path.Combine(_uploadPath, pathPattern);

        //        // Tạo thư mục nếu chưa tồn tại
        //        if (!Directory.Exists(pathUpload))
        //        {
        //            Directory.CreateDirectory(pathUpload);
        //        }

        //        var processedFile = new List<RequestInvoiceFile>();

        //        // Lưu từng file vào thư mục local
        //        foreach (var file in files)
        //        {
        //            if (file.Length > 0)
        //            {
        //                string filePath = Path.Combine(pathUpload, file.FileName);

        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }

        //                var fileRI = new RequestInvoiceFile
        //                {
        //                    RequestInvoiceID = ri.ID,
        //                    FileName = file.FileName,
        //                    OriginPath = pathUpload,
        //                    ServerPath = pathUpload,
        //                    //IsDeleted = false,
        //                    CreatedBy = User.Identity?.Name ?? "System",
        //                    CreatedDate = DateTime.Now,
        //                    UpdatedBy = User.Identity?.Name ?? "System",
        //                    UpdatedDate = DateTime.Now
        //                };

        //                await _requestInvoiceFileRepo.CreateAsync(fileRI);
        //                processedFile.Add(fileRI);
        //            }
        //        }

        //        //return Ok(new
        //        //{
        //        //    status = 1,
        //        //    message = $"{processedFile.Count} tệp đã được tải lên thành công",
        //        //    data = processedFile
        //        //});
        //        return Ok(ApiResponseFactory.Success(processedFile, $"{processedFile.Count} tệp đã được tải lên thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}


        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        //[RequiresPermission("N27,N36,N1,N31")]

        public async Task<IActionResult> Upload(int requestInvoiceId, int fileType)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var files = form.Files;

                // Kiểm tra input
                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));

                var ri = _requestInvoiceRepo.GetByID(requestInvoiceId);
                if (ri == null)
                    throw new Exception("RequestInvoice not found");

                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

                var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                string targetFolder = uploadPath;
                if (!string.IsNullOrWhiteSpace(subPathRaw))
                {
                    var separator = Path.DirectorySeparatorChar;
                    var segments = subPathRaw
                        .Replace('/', separator)
                        .Replace('\\', separator)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(seg =>
                        {
                            var invalidChars = Path.GetInvalidFileNameChars();
                            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                            cleaned = cleaned.Replace("..", "").Trim();
                            return cleaned;
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                }
                else
                {
                    targetFolder = Path.Combine(uploadPath, $"NB{ri.ID}");
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var processedFile = new List<RequestInvoiceFile>();

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;

                    // Tạo tên file unique để tránh trùng lặp
                    var fileExtension = Path.GetExtension(file.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var uniqueFileName = $"{originalFileName}{fileExtension}";
                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    // Lưu file trực tiếp vào targetFolder (không tạo file tạm khác)
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var filePO = new RequestInvoiceFile
                    {
                        RequestInvoiceID = ri.ID,
                        FileType = fileType, // Loại file của yêu cầu xuất hóa đơn : 1, loại file tờ khai xuất khẩu: 2
                        FileName = uniqueFileName,
                        OriginPath = targetFolder,
                        ServerPath = targetFolder,
                        IsDeleted = false,
                        CreatedBy = User.Identity?.Name ?? "System",
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity?.Name ?? "System",
                        UpdatedDate = DateTime.Now
                    };

                    await _requestInvoiceFileRepo.CreateAsync(filePO);
                    processedFile.Add(filePO);
                }

                return Ok(ApiResponseFactory.Success(processedFile, $"{processedFile.Count} tệp đã được tải lên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }



        [HttpPost("delete-file")]
        public IActionResult DeleteFile([FromBody] List<int> fileIds)
        {
            if (fileIds == null || !fileIds.Any())
                throw new Exception("Danh sách file ID không được trống");

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
                    _requestInvoiceFileRepo.Update(file);

                    // Xóa file vật lý
                    var physicalPath = Path.Combine(file.ServerPath, file.FileName);
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);

                    results.Add(new { fileId, success = true, message = "Xóa thành công" });
                }

                return Ok(ApiResponseFactory.Success(results, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        #endregion

    }
}
