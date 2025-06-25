using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.IO;
using System.IO.Compression;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        //UserRepo userRepo = new UserRepo();

        //[HttpPost("login")]
        //public async Task<IActionResult> Login(User user)
        //{
        //    try
        //    {
        //        var user = U
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string controllerName, [FromQuery] string subPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(controllerName))
                    return BadRequest(new { status = 0, message = "Controller name is required." });

                if (string.IsNullOrWhiteSpace(subPath))
                    return BadRequest(new { status = 0, message = "Sub path is required." });

                // Get base path from config and append controller name
                var basePath = Config.Path();
                var folderPath = Path.Combine(basePath, controllerName);

                // Combine with the provided subPath
                var fullPath = Path.Combine(folderPath, subPath);

                if (!System.IO.File.Exists(fullPath))
                    return NotFound(new { status = 0, message = "File not found." });

                var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                var fileName = Path.GetFileName(fullPath);
                var contentType = "application/octet-stream";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }
        [HttpGet("download-folder")]
        public IActionResult DownloadFiles([FromQuery] string controllerName, [FromQuery] string? subPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(controllerName))
                    return BadRequest(new { status = 0, message = "Controller name is required." });

                var basePath = Config.Path();
                var folderPath = Path.Combine(basePath, controllerName);

                if (!Directory.Exists(folderPath))
                    return NotFound(new { status = 0, message = "Controller folder not found." });

                // Nếu subPath không có, nén toàn bộ thư mục controller thành zip
                if (string.IsNullOrWhiteSpace(subPath))
                {
                    var zipFileName = $"{controllerName}_files_{DateTime.Now:yyyyMMddHHmmss}.zip";
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                            foreach (var filePath in files)
                            {
                                var entryName = Path.GetRelativePath(folderPath, filePath);
                                archive.CreateEntryFromFile(filePath, entryName);
                            }
                        }
                        memoryStream.Position = 0;
                        return File(memoryStream.ToArray(), "application/zip", zipFileName);
                    }
                }

                // Nếu có subPath, trả về file đơn lẻ như cũ
                var fullPath = Path.Combine(folderPath, subPath);

                if (!System.IO.File.Exists(fullPath))
                    return NotFound(new { status = 0, message = "File not found." });

                var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                var fileName = Path.GetFileName(fullPath);
                var contentType = "application/octet-stream";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }
    }
}
