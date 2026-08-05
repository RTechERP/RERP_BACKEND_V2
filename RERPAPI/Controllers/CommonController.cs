using Microsoft.AspNetCore.Mvc;
using RERPAPI.Helpers;
using RERPAPI.Model.Common;
using ZXing;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        [HttpGet("qrcode")]
        public IActionResult GetQrCode(string text, int size = 220)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Thiếu nội dung để tạo mã QR"));
                }

                var bytes = BarcodeHelper.GeneratePng(text.Trim(), BarcodeFormat.QR_CODE, size, size);
                return File(bytes, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("barcode")]
        public IActionResult GetBarcode(string text, int width = 320, int height = 100)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Thiếu nội dung để tạo mã vạch"));
                }

                var bytes = BarcodeHelper.GeneratePng(text.Trim(), BarcodeFormat.CODE_128, width, height);
                return File(bytes, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
