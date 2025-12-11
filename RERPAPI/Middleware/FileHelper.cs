using Microsoft.AspNetCore.Http;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Middleware
{
    public static class FileHelper
    {
        public static async Task<APIResponse> UploadFile(IFormFile file, string path)
        {
            try
            {
                //var form = await Request.ReadFormAsync();
                //var key = form["key"].ToString();
                //var files = form.Files;

                // Kiểm tra input
                //if (string.IsNullOrWhiteSpace(key))
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));
                //}

                //if (files == null || files.Count == 0)
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));
                //}

                // Lấy đường dẫn từ ConfigSystem
                //var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                //if (string.IsNullOrWhiteSpace(uploadPath))
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));
                //}

                //// Đọc subPath từ form (nếu có) và ghép vào uploadPath
                //var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                //string targetFolder = uploadPath;
                //if (!string.IsNullOrWhiteSpace(subPathRaw))
                //{
                //    // Chuẩn hóa dấu phân cách và loại bỏ ký tự không hợp lệ trong từng segment
                //    var separator = Path.DirectorySeparatorChar;
                //    var segments = subPathRaw
                //        .Replace('/', separator)
                //        .Replace('\\', separator)
                //        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                //        .Select(seg =>
                //        {
                //            var invalidChars = Path.GetInvalidFileNameChars();
                //            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                //            // Ngăn chặn đường dẫn leo lên thư mục cha
                //            cleaned = cleaned.Replace("..", "").Trim();
                //            return cleaned;
                //        })
                //        .Where(s => !string.IsNullOrWhiteSpace(s))
                //        .ToArray();

                //    if (segments.Length > 0)
                //    {
                //        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                //    }
                //}

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //var uploadResults = object;

                if (file.Length > 0)
                {
                    // Tạo tên file unique
                    var fileExtension = Path.GetExtension(file.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var uniqueFileName = $"{originalFileName}_{DateTime.Now.ToString("ddMMyyyyHHmmss")}_{fileExtension}";

                    //var uniqueFileName = originalFileName;
                    var fullPath = Path.Combine(path, uniqueFileName);

                    // Lưu file
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return ApiResponseFactory.Success(uniqueFileName, $"Upload thành công!");
                }
                return ApiResponseFactory.Fail(null, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
