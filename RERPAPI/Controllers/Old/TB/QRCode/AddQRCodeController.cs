using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.TB;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.Technical;


namespace RERPAPI.Controllers.Old.TB.QRCode
{
    [Route("api/[controller]")]
    [ApiController]

    public class AddQRCodeController : ControllerBase
    {

        ProductRTCRepo _productRTCRepo;
        ProductRTCQRCodeRepo _productRTCQRCodeRepo;

        public AddQRCodeController(ProductRTCRepo productRTCRepo, ProductRTCQRCodeRepo productRTCQRCodeRepo)
        {
            _productRTCRepo = productRTCRepo;
            _productRTCQRCodeRepo = productRTCQRCodeRepo;
        }

        [HttpPost("get-productRTC")]
        public IActionResult GetListAssets([FromBody] int WarehouseID, string serialNumber)
        {
            try
            {
                var products = SQLHelper<dynamic>.ProcedureToList("spGetSearchProductTechSerial",
                new string[] { "@SerialNumber", "@WarehouseID" },
                new object[] { serialNumber, WarehouseID });

                return Ok(new
                {
                    status = 1,

                    products = SQLHelper<dynamic>.GetListData(products, 0),
                    TotalPage = SQLHelper<dynamic>.GetListData(products, 1)
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
        [HttpPost("get-location-modula")]
        public IActionResult GetLocationModula()
        {
            try
            {
                string procedureName = "spGetModulaLocation";
                string[] paramNames = new string[] { "@ModulaLocationID", "@Keyword", "@IsDeleted" };
                object[] paramValues = new object[] { 0, "", 0 };
                var modulaData = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var dataList = SQLHelper<dynamic>.GetListData(modulaData, 0);
                return Ok(ApiResponseFactory.Success(new { dataList }, "Lấy danh sách vị trí modula thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-product-and-qrcode")]
        public IActionResult GetQRCode(int? wareHouseID, string? filterText = "")
        {
            try
            {
                string procedureName = "spGetProductAndQrCode";
                string[] paramNames = new string[] { "@WarehouseID", "@FilterText" };
                object[] paramValues = new object[] { wareHouseID ?? 1, filterText ?? "" };
                var qrCode = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var dataList = SQLHelper<dynamic>.GetListData(qrCode, 0);
                return Ok(ApiResponseFactory.Success(new { dataList }, "Lấy danh sách QRCode thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-product")]
        public IActionResult GetProDuct()
        {
            try
            {
                var productRTC = _productRTCRepo.GetAll(x => x.IsDelete != true);
                return Ok(ApiResponseFactory.Success(new { productRTC }, "Lấy danh sách QRCode thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProductRTCQRCode> qrCodes)
        {
            try
            {
                if (qrCodes != null && qrCodes.Any())
                {

                    foreach (var item in qrCodes)
                    {
                        if (item.IsDeleted != true)
                        {
                            var validate = _productRTCQRCodeRepo.Validate(item);
                            if (validate.status == 0) return BadRequest(validate);
                        }

                        if (item.ID <= 0)
                        {
                            await _productRTCQRCodeRepo.CreateAsync(item);
                        }
                        else
                            await _productRTCQRCodeRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(qrCodes, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-location")]
        public async Task<IActionResult> SaveLocation([FromBody] List<ProductRTCQRCode> qrCodes)
        {
            try
            {
                if (qrCodes != null && qrCodes.Any())
                {

                    foreach (var item in qrCodes)
                    {


                        if (item.ID <= 0)
                        {
                            await _productRTCQRCodeRepo.CreateAsync(item);
                        }


                        else
                            await _productRTCQRCodeRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(qrCodes, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
