using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ProductRTCController : ControllerBase
    {

        const int WAREHOUSEID = 1;
        private readonly ProductGroupRTCRepo _productGroupRTCRepo;
        private readonly ProductRTCRepo _productRTCRepo;
        private readonly ProductLocationRepo _productLocationRepo;
        private readonly ConfigSystemRepo config;
        private readonly FirmRepo _firmRepo;
        private readonly LocationRepo _locationRepo;

        public ProductRTCController(
            ProductGroupRTCRepo productGroupRTCRepo,
            ProductRTCRepo productRTCRepo,
            ProductLocationRepo productLocationRepo,
            ConfigSystemRepo configSystemRepo, FirmRepo firmRepo, LocationRepo locationRepo)
        {
            _productGroupRTCRepo = productGroupRTCRepo;
            _productRTCRepo = productRTCRepo;
            _productLocationRepo = productLocationRepo;
            config = configSystemRepo;
            _firmRepo = firmRepo;
            _locationRepo = locationRepo;
        }


        [HttpPost("get-productRTC")]
        public IActionResult GetAll([FromBody] ProductRTCRequetParam request)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetProductRTC",
                new string[] { "@ProductGroupID", "@Keyword", "@CheckAll", "@WarehouseID", "@ProductRTCID", "@ProductGroupNo", "@PageNumber", "@PageSize", "@WarehouseType" },
                new object[] { request.ProductGroupID, request.Keyword, request.CheckAll, request.WarehouseID, request.ProductRTCID, request.ProductGroupNo, request.Page, request.Size, request.WarehouseType });

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
        [HttpGet("get-productRTC-group/{warehouseType}")]
        public IActionResult GetAll(int warehouseType = 1)
        {
            try
            {
                List<ProductGroupRTC> productGroup = _productGroupRTCRepo.GetAll(x => x.WarehouseType == warehouseType);
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
                return Ok(ApiResponseFactory.Success(req.file.FileName, "Upload thành công")); //TN.Binh update
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpGet("get-location")]
        public IActionResult GetLocation(int? warehouseID, int locationType)
        {
            try
            {
                var location = _productLocationRepo.GetAll(x => x.IsDeleted != true &&
                                                                x.WarehouseID == warehouseID);
                return Ok(ApiResponseFactory.Success(new { location }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-product-code")]
        public IActionResult GenerateProductCode(int productGroupID)
        {

            try
            {
                string newCode = _productRTCRepo.generateProductCode(productGroupID);
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
        [HttpPost("save-data-excel")]
        public async Task<IActionResult> SaveDataExcel([FromBody] ProductRTCFullDTO product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu trả về."));
                }

                int successCount = 0;
                int failCount = 0;
                List<string> skippedCodes = new();

                // --- Lưu nhóm sản phẩm ---
                if (product.productGroupRTC != null)
                {
                    if (product.productGroupRTC.ID <= 0)
                        await _productGroupRTCRepo.CreateAsync(product.productGroupRTC);
                    else
                        await _productGroupRTCRepo.UpdateAsync(product.productGroupRTC);
                }

                // --- Lưu danh sách sản phẩm ---
                if (product.productRTCs != null && product.productRTCs.Any())
                {
                    foreach (var item in product.productRTCs)
                    {
                        try
                        {
                            if (item.IsDelete != true)
                            {
                                // --- Kiểm tra trùng mã sản phẩm ---
                                if (_productRTCRepo.checkExistProductCodeRTC(item))
                                {
                                    skippedCodes.Add("Mã sản phẩm:" + item.ProductCode ?? "N/A");
                                    failCount++;
                                    continue;
                                }

                                //// --- Kiểm tra trùng Serial ---
                                //if (_productRTCRepo.checkExistSerialRTC(item))
                                //{
                                //    skippedCodes.Add("SerialNumber:" + item.SerialNumber ?? "Serial N/A");
                                //    failCount++;
                                //    continue;
                                //}

                                //// --- Kiểm tra trùng Partnumber ---
                                //if (_productRTCRepo.checkExistPartnumberRTC(item))
                                //{
                                //    skippedCodes.Add("PartNumber:" + item.PartNumber ?? "Partnumber N/A");
                                //    failCount++;
                                //    continue;
                                //}
                            }

                            //// --- Xử lý xóa ---
                            //if (item.IsDelete == true)
                            //{
                            //    if (item.ID > 0)
                            //    {
                            //        await _productRTCRepo.UpdateAsync(item);
                            //        successCount++;
                            //    }
                            //}
                            else
                            {
                                var firm = _firmRepo.GetAll(x => x.FirmType == 2 && (x.FirmName ?? "").ToUpper().Trim() == (item.Maker ?? "").ToUpper().Trim() && x.IsDelete == false).FirstOrDefault();
                                if (firm != null) item.FirmID = firm.ID;
                                var location = _locationRepo.GetAll(x => (x.LocationName ?? "").Trim().ToUpper() == (item.AddressBox ?? "").Trim().ToUpper() && x.IsDeleted == false).FirstOrDefault();
                                if (location != null) item.ProductLocationID = location.ID;

                                item.ProductCodeRTC = _productRTCRepo.generateProductCode(Convert.ToInt32(item.ProductGroupRTCID));
                                if (item.ID <= 0)
                                    await _productRTCRepo.CreateAsync(item);
                                else
                                    await _productRTCRepo.UpdateAsync(item);

                                successCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Có thể log chi tiết lỗi nếu cần
                            failCount++;
                        }
                    }
                }

                string message = $"Lưu thành công {successCount} bản ghi, thất bại {failCount} bản ghi.";
                if (skippedCodes.Any())
                    message += $" Các Mã sản phẩm" +
                        //$", SerialNumber, PartNumber" +
                        $" bị bỏ qua (trùng): {string.Join(", ", skippedCodes)}.";

                return Ok(ApiResponseFactory.Success(new
                {
                    successCount,
                    failCount,
                    skippedCodes
                }, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProductRTCFullDTO product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ."));
                }

                foreach (var item in product.productRTCs)
                {
                    if (item.IsDelete != true)
                    {
                        if (_productRTCRepo.checkExistProductCodeRTC(item))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null,
                                $"Mã thiết bị [{item.ProductCode}] đã tồn tại trong hệ thống."));
                        }

                        if (_productRTCRepo.checkExistSerialRTC(item))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null,
                                $"Số serial [{item.SerialNumber}] đã tồn tại trong hệ thống."));
                        }

                        if (_productRTCRepo.checkExistPartnumberRTC(item))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null,
                                $"Partnumber [{item.PartNumber}] đã tồn tại trong hệ thống."));
                        }
                    }
                }

                // --- Lưu nhóm sản phẩm ---
                if (product.productGroupRTC != null)
                {
                    if (product.productGroupRTC.ID <= 0)
                        await _productGroupRTCRepo.CreateAsync(product.productGroupRTC);
                    else
                        await _productGroupRTCRepo.UpdateAsync(product.productGroupRTC);
                }

                // --- Lưu danh sách sản phẩm ---
                if (product.productRTCs != null && product.productRTCs.Any())
                {
                    foreach (var item in product.productRTCs)
                    {
                        if (item.IsDelete == true)
                        {
                            if (item.ID > 0)
                                await _productRTCRepo.UpdateAsync(item);
                        }
                        else
                        {
                            if (item.ID <= 0)
                            {
                                item.ProductCodeRTC = _productRTCRepo.generateProductCode(item.ProductGroupRTCID ?? 0);
                                await _productRTCRepo.CreateAsync(item);
                            }
                            else
                                await _productRTCRepo.UpdateAsync(item);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] ProductRTCFullDTO product)
        //{
        //    try
        //    {
        //        if (product == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
        //        foreach (var item in product.productRTCs)
        //        {
        //            if (item.IsDelete != true)
        //            {
        //                if (_productRTCRepo.checkExistProductCodeRTC(item))
        //                {
        //                    return BadRequest(ApiResponseFactory.Fail(null, $"Mã thiết bị [{item.ProductCode}] đã tồn tại trong hệ thống."));
        //                }

        //            }
        //        }

        //        if (product.productGroupRTC != null)
        //        {

        //            if (product.productGroupRTC.ID <= 0)
        //                await _productGroupRTCRepo.CreateAsync(product.productGroupRTC);
        //            else
        //                await _productGroupRTCRepo.UpdateAsync(product.productGroupRTC);
        //        }
        //        if (product.productRTCs != null && product.productRTCs.Any())
        //        {
        //            //TN.Binh update logic xoa
        //            foreach (var item in product.productRTCs)
        //            {
        //                if (item.IsDelete == true)
        //                {
        //                    if (item.ID > 0)
        //                        await _productRTCRepo.UpdateAsync(item);
        //                }
        //                else
        //                {
        //                    if (item.ID <= 0)
        //                        await _productRTCRepo.CreateAsync(item);
        //                    else
        //                        await _productRTCRepo.UpdateAsync(item);
        //                }
        //            }
        //            //end
        //        }

        //        //return Ok(new { status = 1 });
        //        return Ok(ApiResponseFactory.Success(null, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest(new
        //        //{
        //        //    status = 0,
        //        //    message = ex.Message,
        //        //    error = ex.ToString()
        //        //});
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }

        //}


        [HttpGet("get-by-qrcode")]
        public IActionResult GetProductByQrCode(string qrCode)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetProductRTCByQrCode",
                                                                new string[] { "@ProductRTCQRCode", "@WarehouseID" },
                                                                new object[] { qrCode, WAREHOUSEID });

                var historys = SQLHelper<object>.GetListData(datas, 0);

                if (historys.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Thiết bị có mã QR [{qrCode}] đang được mượn!", historys));
                }
                else
                {
                    var products = SQLHelper<object>.GetListData(datas, 1);
                    return Ok(ApiResponseFactory.Success(products, $""));
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
