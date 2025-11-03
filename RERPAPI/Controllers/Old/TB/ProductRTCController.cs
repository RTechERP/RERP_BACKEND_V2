using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.TB;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.TB;
using RERPAPI.Repo.GenericEntity;
using System.Net.Mime;


namespace RERPAPI.Controllers.Old.TB
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRTCController : ControllerBase
    {
        Repo.GenericEntity.ProductGroupRTCRepo _productGroupRTCRepo = new Repo.GenericEntity.ProductGroupRTCRepo();
        ProductRTCRepo _productRTCRepo = new ProductRTCRepo();
        ProductLocationRepo _productLocationRepo = new ProductLocationRepo();
        ConfigSystemRepo config = new ConfigSystemRepo();
        [HttpPost("get-productRTC")]
        public IActionResult GetListAssets([FromBody] ProductRTCRequetParam request)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetProductRTC",
                new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID", "@ProductRTCID", "@ProductGroupNo", "@PageNumber", "@PageSize" },
                new object[] { request.ProductGroupID, request.Keyword, request.CheckAll, request.WarehouseID, request.ProductRTCID, request.ProductGroupNo, request.Page, request.Size });



                var data = new
                {
                    products = SQLHelper<dynamic>.GetListData(products, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(products, 1)
                };

                //return Ok(new
                //{
                //    status = 1,

                //    products = SQLHelper<dynamic>.GetListData(products, 0),
                //    TotalPage = SQLHelper<dynamic>.GetListData(products, 1)
                //});

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                //return Ok(new
                //{

                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-productRTC-group")]
        public IActionResult GetAll()
        {
            try
            {
                List<ProductGroupRTC> productGroup = _productGroupRTCRepo
                    .GetAll();
                //.Where(x => x.IsDeleted == false)
                //.ToList();

                //return Ok(new
                //{
                //    status = 1,
                //    data = productGroup
                //});

                return Ok(ApiResponseFactory.Success(productGroup, ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //[HttpPost("upload")]
        //public IActionResult Upload(IFormFile file, string path)
        //{
        //    try
        //    {
        //        int statusCode = 0;
        //        string fileName = "";
        //        string message = "Upload file thất bại!";

        //        if (file != null)
        //        {
        //            // Danh sách định dạng ảnh cho phép
        //           // var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp",".png" };
        //            var fileExtension = Path.GetExtension(file.FileName).ToLower();

        //            //if (!allowedExtensions.Contains(fileExtension))
        //            //{
        //            //    //return BadRequest(new
        //            //    //{
        //            //    //    status = 0,
        //            //    //    Message = "Chỉ được upload file ảnh (jpg, jpeg, png, gif, bmp)"
        //            //    //});
        //            //    return Ok(ApiResponseFactory.Fail(null, "Chỉ được upload file ảnh (jpg, jpeg, png, gif, bmp)"));
        //            //}
        //           // string path = "D:\\RTC_Sw\\RTC\\ProductRTC\\";
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }

        //            using (FileStream fileStream = System.IO.File.Create(path + file.FileName))
        //            {
        //                file.CopyTo(fileStream);
        //                fileStream.Flush();

        //                statusCode = 1;
        //                fileName = file.FileName;
        //                message = "Upload File thành công!";
        //            }
        //        }
        //        else
        //        {
        //            statusCode = 0;
        //            message = "Không có file được gửi lên.";
        //        }

        //        //return Ok(new
        //        //{
        //        //    status = statusCode,
        //        //    FileName = fileName,
        //        //    Message = message
        //        //});

        //        return Ok(ApiResponseFactory.Success(fileName, message));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return Ok(new
        //        //{
        //        //    status = 0,
        //        //    Message = $"Upload file thất bại! ({ex.Message})"
        //        //});

        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        public class UploadRequest
        {
            public IFormFile file { get; set; }
            public string path { get; set; }
        }
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public IActionResult Upload([FromForm] UploadRequest req)
        {
            try
            {
                if (req?.file == null || req.file.Length == 0)
                    return BadRequest("No file");
                if (string.IsNullOrWhiteSpace(req.path))
                    return BadRequest("path required");

                Directory.CreateDirectory(req.path);
                var dest = Path.Combine(req.path, req.file.FileName);
                using var fs = System.IO.File.Create(dest);
                req.file.CopyTo(fs);
                return Ok(ApiResponseFactory.Success(null, "Upload thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpGet("get-location")]
        public IActionResult GetLocation(int? warehouseID)
        {
            try
            {
                var location = _productLocationRepo.GetAll(x => x.WarehouseID == warehouseID);
                return Ok(ApiResponseFactory.Success(new { location }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-product-code")]
        public IActionResult GenerateProductCode()
        {

            try
            {
                string newCode = _productRTCRepo.generateProductCode();
                //return Ok(new
                //{
                //    status = 1,
                //    data = newCode
                //});

                return Ok(ApiResponseFactory.Success(newCode, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("preveiw")]
        public IActionResult GetPreview([FromQuery] string full)
        {
            if (string.IsNullOrWhiteSpace(full)) return BadRequest("full required");
            var con = config.GetAll(x => x.KeyName == "PathPreview").FirstOrDefault() ?? new ConfigSystem();
            string root = "";
            if (con.ID > 0)
            {
                root = con.KeyValue.Trim();
            }
            // Chuẩn hoá đường dẫn
            full = full.Replace('/', '\\');                       // hỗ trợ cả / và \
            var normalized = Path.GetFullPath(full);
            var rootFull = Path.GetFullPath(root);

            // Chặn truy cập ngoài ROOT
            if (!normalized.StartsWith(rootFull, System.StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (!System.IO.File.Exists(normalized)) return NotFound();

            var ext = Path.GetExtension(normalized).ToLowerInvariant();
            var mime = ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };

            // Ảnh/PDF hiển thị inline, còn lại browser tự xử lý
            Response.Headers["Content-Disposition"] =
                new ContentDisposition { FileName = Path.GetFileName(normalized), Inline = true }.ToString();

            return File(System.IO.File.OpenRead(normalized), mime);
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProductRTCFullDTO product)
        {
            try
            {
                if (product == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (product.productGroupRTC != null)
                {

                    if (product.productGroupRTC.ID <= 0)
                        await _productGroupRTCRepo.CreateAsync(product.productGroupRTC);
                    else
                        await _productGroupRTCRepo.UpdateAsync(product.productGroupRTC);
                }
                if (product.productRTCs != null && product.productRTCs.Any())
                {
                    foreach (var item in product.productRTCs)
                    {

                        if (item.ID <= 0)
                            await _productRTCRepo.CreateAsync(item);
                        else
                            await _productRTCRepo.UpdateAsync(item);
                    }
                }

                //return Ok(new { status = 1 });
                return Ok(ApiResponseFactory.Success(null, ""));
            }
            catch (Exception ex)
            {
                //return BadRequest(new
                //{
                //    status = 0,
                //    message = ex.Message,
                //    error = ex.ToString()
                //});
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
    }
}
