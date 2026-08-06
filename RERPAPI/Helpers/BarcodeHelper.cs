using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

namespace RERPAPI.Helpers
{
    // Tái sử dụng kỹ thuật generate QR/Barcode bằng ZXing đang dùng trong
    // BillImportController.ImportExcel / BillExportController.ExportExcel (nhúng ảnh vào Excel),
    // tách thành helper dùng chung để trả về ảnh PNG độc lập.
    public static class BarcodeHelper
    {
        public static byte[] GeneratePng(string text, BarcodeFormat format, int width, int height, int margin = 1)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin,
                    PureBarcode = false
                }
            };

            var pixelData = writer.Write(text);
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppRgb);

                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }
}
