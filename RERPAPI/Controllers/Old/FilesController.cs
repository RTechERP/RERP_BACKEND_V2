using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(
        [FromQuery] string pathUpload,
        [FromQuery] string pathPattern)
        {
            try
            {
                if (Request.Form.Files.Count == 0)
                    return BadRequest("No file uploaded.");

                // Ghép đường dẫn đầy đủ
                string targetPath = Path.Combine(pathUpload, pathPattern);

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                var uploadedFiles = new List<object>();

                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileName = Path.GetFileName(formFile.FileName);
                        string fullPath = Path.Combine(targetPath, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        uploadedFiles.Add(new
                        {
                            FileName = fileName,
                            SavePath = fullPath,
                            FileSize = formFile.Length
                        });
                    }
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Files uploaded successfully",
                    Files = uploadedFiles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, ex.Message });
            }
        }
    }
}
