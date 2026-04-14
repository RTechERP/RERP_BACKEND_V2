using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.ProductProtectiveGear;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.IO;

namespace RERPAPI.Controllers.HRM.ProductProtectiveGear
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductProtectiveGearController : ControllerBase
    {
        private readonly ProductGroupRTCRepo _productGroupRTCRepo;
        private readonly UnitCountRepo _unitCountRepo;
        private readonly FirmRepo _firmRepo;
        private readonly ProductRTCRepo _productRTCRepo;
        private readonly ProductLocationRepo _productLocationRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        string pathServer = "\\\\192.168.1.190\\Common\\11. HCNS";
        string pathPattern = $@"DoPhongSach\Anh";
        string urlAPI = $@"http://192.168.1.2:8083/api/hcns";
        public ProductProtectiveGearController(ProductGroupRTCRepo productGroupRTCRepo, UnitCountRepo unitCountRepo, FirmRepo firmRepo, ProductLocationRepo productLocationRepo, ProductRTCRepo productRTCRepo, ConfigSystemRepo configSystemRepo)
        {
            _productGroupRTCRepo = productGroupRTCRepo;
            _unitCountRepo = unitCountRepo;
            _firmRepo = firmRepo;
            _productLocationRepo = productLocationRepo;
            _productRTCRepo = productRTCRepo;
            _configSystemRepo = configSystemRepo;
        }

        [HttpGet("get-product-group")]
        public IActionResult GetProductGroup( int warehouseID)
        {
            try
            {
                var data = _productGroupRTCRepo.GetAll(c => c.WarehouseID == warehouseID && c.ProductGroupNo.Contains("DBH"));
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-product-rtc")]
        public IActionResult GetProductRTC([FromQuery] ProductRTCParam param)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetProductRTC",
                new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID" },
                new object[] { param.ProductGroupID, param.KeyWord??"", 1, param.WarehouseID }
                );
                var data0 = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(data0, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-unit-count")]
        public IActionResult GetUnitCount()
        {
            try
            {
                var data = _unitCountRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-firm")]
        public IActionResult GetFirm()
        {
            try
            {
                var data = _firmRepo.GetAll(c=>c.FirmType ==2).OrderByDescending(c=>c.ID);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-product-location")]
        public IActionResult GetProductLocation(int wareHouseID)
        {
            try
            {
                var data = _productLocationRepo.GetAll(c=>c.WarehouseID == wareHouseID);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-image-url")]
        public IActionResult GetImageUrl(string LocationImg, string ProductCode )
        {
            try
            {
                FileInfo file = new FileInfo(LocationImg);
                string url = $"{urlAPI}/{pathPattern}/{ProductCode}_{file.Name}";
                return Ok(ApiResponseFactory.Success(url, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> PostSaveDataAsync([FromBody] ProductRTC productRTC,int warehouseType)
        {
            try
            {
                if (_productRTCRepo.checkExistProductCodeRTC(productRTC,warehouseType)==true )
                {
                    string message =$"Mã sản phẩm [{productRTC.ProductCode}] đã tồn tại!";
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (productRTC.ID <= 0)
                {
                    productRTC.CreateDate = DateTime.Now;
                    productRTC.ProductCodeRTC = _productRTCRepo.generateProductCode(TextUtils.ToInt32(productRTC.ProductGroupRTCID));
                    await _productRTCRepo.CreateAsync(productRTC);
                }
                else await _productRTCRepo.UpdateAsync(productRTC);

                return Ok(ApiResponseFactory.Success(productRTC, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("upload-file")]
        public async Task<IActionResult> PostUploadImages([FromQuery] int productRTCID)
        {
            try
            {

                var productRTC = _productRTCRepo.GetByID(productRTCID);
                if (productRTC == null)
                    return NotFound("Product not found");

                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var files = form.Files;

                // Kiểm tra input
                if (string.IsNullOrWhiteSpace(key))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));
                }

                if (files == null || files.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));
                }

                // Lấy đường dẫn từ ConfigSystem
                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));
                }

                // Đọc subPath từ form (nếu có) và ghép vào uploadPath
                var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                string targetFolder = uploadPath;
                if (!string.IsNullOrWhiteSpace(subPathRaw))
                {
                    // Chuẩn hóa dấu phân cách và loại bỏ ký tự không hợp lệ trong từng segment
                    var separator = Path.DirectorySeparatorChar;
                    var segments = subPathRaw
                        .Replace('/', separator)
                        .Replace('\\', separator)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(seg =>
                        {
                            var invalidChars = Path.GetInvalidFileNameChars();
                            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                            // Ngăn chặn đường dẫn leo lên thư mục cha
                            cleaned = cleaned.Replace("..", "").Trim();
                            return cleaned;
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                    {
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                    }
                }

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var uploadResults = new List<object>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Tạo tên file unique
                        var fileExtension = Path.GetExtension(file.FileName);
                        var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                        // var uniqueFileName = $"{originalFileName}{fileExtension}";

                        var uniqueFileName = $"{productRTC.ProductCode}_{originalFileName}{fileExtension}";


                        //var uniqueFileName = originalFileName;
                        var fullPath = Path.Combine(targetFolder, uniqueFileName);

                        // Lưu file
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        uploadResults.Add(new
                        {
                            OriginalFileName = file.FileName,
                            SavedFileName = uniqueFileName,
                            FilePath = fullPath,
                            FileSize = file.Length,
                            file.ContentType,
                            UploadTime = DateTime.Now
                        });

                        // cập nhật DB sau khi upload OK
                        productRTC.LocationImg = file.FileName;
                        _productRTCRepo.Update(productRTC);

                    }
                }
                return Ok(ApiResponseFactory.Success(uploadResults, $"Upload thành công {uploadResults.Count} file!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
     
    }
}
