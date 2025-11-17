using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly RTCContext _context;
        private readonly IConfiguration _configuration;

        //UserRepo _userRepo = new UserRepo();
        vUserGroupLinksRepo _vUserGroupLinksRepo;


        private readonly EmployeeOnLeaveRepo _onLeaveRepo;
        private readonly EmployeeWFHRepo _wfhRepo;
        private readonly ConfigSystemRepo _configSystemRepo;

        public HomeController(IOptions<JwtSettings> jwtSettings, RTCContext context, IConfiguration configuration, EmployeeOnLeaveRepo onLeaveRepo, vUserGroupLinksRepo vUserGroupLinksRepo, EmployeeWFHRepo employeeWFHRepo, ConfigSystemRepo configSystemRepo)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _configuration = configuration;
            _onLeaveRepo = onLeaveRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _wfhRepo = employeeWFHRepo;
            _configSystemRepo = configSystemRepo;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.LoginName) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đăng nhập và Mật khẩu!"));
                }

                //1. Check user
                string loginName = user.LoginName ?? "";
                string password = MaHoaMD5.EncryptPassword(user.PasswordHash ?? "");

                var login = SQLHelper<object>.ProcedureToList("spLogin", new string[] { "@LoginName", "@Password" }, new object[] { loginName, password });
                var hasUsers = SQLHelper<object>.GetListData(login, 0);

                if (hasUsers.Count <= 0 || hasUsers[0].ID <= 0)
                {
                    //_response.status = 0;
                    //_response.message = "Sai tên đăng nhập hoặc mật khẩu!";
                    return Unauthorized(ApiResponseFactory.Fail(null, "Sai tên đăng nhập hoặc mật khẩu!"));
                }

                var hasUser = SQLHelper<object>.GetListData(login, 0)[0];

                //2. Tạo Claims
                var claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Sub,hasUser.ID.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName,hasUser.LoginName ?? ""),
                };

                var dictionary = (IDictionary<string, object>)hasUser;
                foreach (var item in dictionary)
                {
                    //if (item.Key.ToLower() == "passwordhash") continue;

                    var claim = new Claim(item.Key.ToLower(), item.Value?.ToString() ?? "");
                    claims.Add(claim);
                }


                //3. Tạo token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims.ToArray(),
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                //4.Lưu session trên server
                HttpContext.Session.SetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey"), ObjectMapper.GetCurrentUser(claims.ToDictionary(x => x.Type, x => x.Value)));

                return Ok(new
                {
                    access_token = tokenString,
                    expires = token.ValidTo.AddHours(+7)
                });
            }
            catch (Exception ex)
            {
                //_response.status = 0;
                //_response.message = ex.Message;
                //_response.error = ex.ToString();
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [Authorize]
        [ApiKeyAuthorize]
        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                //string key = _configuration.GetValue<string>("SessionKey") ?? "";
                //CurrentUser currentUser = HttpContext.Session.GetObject<CurrentUser>(key);

                return Ok(ApiResponseFactory.Success(currentUser, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        /// <summary>
        /// Upload file chung cho hệ thống
        /// </summary>
        [HttpPost("upload")]
        [Authorize]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                var file = form.Files.FirstOrDefault();

                // Kiểm tra input
                if (string.IsNullOrWhiteSpace(key))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "File không được để trống!"));
                }

                // Lấy đường dẫn từ ConfigSystem
                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));
                }

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Tạo tên file unique để tránh trùng lặp
                var fileExtension = Path.GetExtension(file.FileName);
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uniqueFileName = $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                var fullPath = Path.Combine(uploadPath, uniqueFileName);

                // Lưu file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về thông tin file đã upload
                var result = new
                {
                    OriginalFileName = file.FileName,
                    SavedFileName = uniqueFileName,
                    FilePath = fullPath,
                    FileSize = file.Length,
                    file.ContentType,
                    UploadTime = DateTime.Now
                };

                return Ok(ApiResponseFactory.Success(result, "Upload file thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }
        [HttpGet("download-by-key")]
        [Authorize]
        public IActionResult DownloadByKey(
      [FromQuery] string key,
      [FromQuery] string? subPath,
      [FromQuery] string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống!"));

                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequest(ApiResponseFactory.Fail(null, "FileName không được để trống!"));

                // Lấy đường dẫn gốc theo key
                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

                // Chuẩn hóa subPath giống UploadMultipleFiles
                string targetFolder = uploadPath;
                if (!string.IsNullOrWhiteSpace(subPath))
                {
                    var separator = Path.DirectorySeparatorChar;
                    var segments = subPath
                        .Replace('/', separator)
                        .Replace('\\', separator)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(seg =>
                        {
                            var invalidChars = Path.GetInvalidFileNameChars();
                            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                            cleaned = cleaned.Replace("..", "").Trim(); // chống leo thư mục
                            return cleaned;
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                }

                // Chuẩn hóa tên file
                var safeFileName = new string(fileName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray())
                    .Replace("..", "")
                    .Trim();

                var fullPath = Path.Combine(targetFolder, safeFileName);

                // Đảm bảo đường dẫn nằm trong root uploadPath
                var rootNormalized = Path.GetFullPath(uploadPath);
                var fullNormalized = Path.GetFullPath(fullPath);
                if (!fullNormalized.StartsWith(rootNormalized, StringComparison.OrdinalIgnoreCase))
                    return BadRequest(ApiResponseFactory.Fail(null, "Đường dẫn không hợp lệ"));

                // Nếu không tồn tại và tên file không có extension -> thử dò fileName.*
                if (!System.IO.File.Exists(fullPath) && string.IsNullOrWhiteSpace(Path.GetExtension(safeFileName)))
                {
                    var match = Directory.GetFiles(targetFolder, safeFileName + ".*").FirstOrDefault();
                    if (match != null)
                        fullPath = match;
                }

                if (!System.IO.File.Exists(fullPath))
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy file"));

                // Xác định content-type
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fullPath, out var contentType))
                    contentType = "application/octet-stream";

                var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var downloadName = Path.GetFileName(fullPath);

                return File(stream, contentType, downloadName);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi download file: {ex.Message}"));
            }
        }
        /// <summary>
        /// Upload nhiều file cùng lúc
        /// </summary>
        [HttpPost("upload-multiple")]
        //[Authorize]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadMultipleFiles()
        {
            try
            {
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
                        var uniqueFileName = $"{originalFileName}{fileExtension}";
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
                    }
                }

                return Ok(ApiResponseFactory.Success(uploadResults, $"Upload thành công {uploadResults.Count} file!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }

        [HttpGet("employee-onleave-and-wfh")]
        public IActionResult GetEmployeeOnleaveAndWFHByDate()
        {
            try
            {
                var a = _onLeaveRepo.GetAll();
                //var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                //CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                DateTime dateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddSeconds(-1);
                DateTime dateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddSeconds(+1);

                //dateStart = new DateTime(2025, 10, 08, 0, 0, 0).AddSeconds(-1);
                //dateEnd = new DateTime(2025, 10, 08, 23, 59, 59).AddSeconds(+1);

                string keyword = "";
                int employeeID = 0;
                int isApproved = -1;
                int type = 0;

                var dataOnleaves = SQLHelper<object>.ProcedureToList("spGetEmployeeOnLeaveInWeb",
                                                            new string[] { "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID", "@IsApproved", "@Type" },
                                                            new object[] { dateStart, dateEnd, keyword, employeeID, isApproved, type });

                var dataWfhs = SQLHelper<object>.ProcedureToList("spGetEmployeeWFHInWeb",
                                                            new string[] { "@DateStart", "@DateEnd", "@EmployeeID", "@IsApproved", "@TimeWFH", "@Keyword" },
                                                            new object[] { dateStart, dateEnd, employeeID, isApproved, type, keyword });

                var data = new
                {
                    employeeOnleaves = SQLHelper<object>.GetListData(dataOnleaves, 0)
                                            .GroupBy(x => new
                                            {
                                                DepartmentID = (int)x.DepartmentID,
                                                DepartmentName = (string)x.DepartmentName,
                                                DepartmentSTT = (int)x.DepartmentSTT
                                            })
                                            .OrderBy(g => g.Key.DepartmentSTT)
                                            .Select(g => new
                                            {
                                                g.Key.DepartmentName,
                                                Employees = g.Select(item => new
                                                {
                                                    item.EmployeeLeave,
                                                    TimeOnLeaveText = item.TimeOnLeave == 1 ? " (S)" : item.TimeOnLeave == 2 ? " (C)" : ""
                                                }).ToList()
                                            }).ToList(),

                    employeeWfhs = SQLHelper<object>.GetListData(dataWfhs, 0)
                                        .GroupBy(x => new
                                        {
                                            DepartmentID = (int)x.DepartmentID,
                                            DepartmentName = (string)x.DepartmentName,
                                            DepartmentSTT = (int)x.DepartmentSTT
                                        })
                                        .OrderBy(g => g.Key.DepartmentSTT)
                                        .Select(g => new
                                        {
                                            g.Key.DepartmentName,
                                            Employees = g.Select(item => new
                                            {
                                                item.EmployeeName,
                                                TimeWFHText = item.TimeWFH == 1 ? " (S)" : item.TimeWFH == 2 ? " (C)" : "",
                                                IsApprovedBGDText = (bool)item.IsApprovedBGD == true ? "" : " - BGĐ chưa duyệt",
                                            }).ToList()
                                        }).ToList(),
                };

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        //[HttpGet("download")]
        //public IActionResult DownloadFile([FromQuery] string controllerName, [FromQuery] string subPath) {

        //        return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString()


        //}

        //[HttpGet("download-folder")]
        //public IActionResult DownloadFiles([FromQuery] string controllerName, [FromQuery] string? subPath)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(controllerName))
        //            return BadRequest(new { status = 0, message = "Controller name is required." });

        //        var basePath = Config.Path();
        //        var folderPath = Path.Combine(basePath, controllerName);

        //        if (!Directory.Exists(folderPath))
        //            return NotFound(new { status = 0, message = "Controller folder not found." });

        //        // Nếu subPath không có, nén toàn bộ thư mục controller thành zip
        //        if (string.IsNullOrWhiteSpace(subPath))
        //        {
        //            var zipFileName = $"{controllerName}_files_{DateTime.Now:yyyyMMddHHmmss}.zip";
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //                {
        //                    var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        //                    foreach (var filePath in files)
        //                    {
        //                        var entryName = Path.GetRelativePath(folderPath, filePath);
        //                        archive.CreateEntryFromFile(filePath, entryName);
        //                    }
        //                }
        //                memoryStream.Position = 0;
        //                return File(memoryStream.ToArray(), "application/zip", zipFileName);
        //            }
        //        }

        //        // Nếu có subPath, trả về file đơn lẻ như cũ
        //        var fullPath = Path.Combine(folderPath, subPath);

        //        if (!System.IO.File.Exists(fullPath))
        //            return NotFound(new { status = 0, message = "File not found." });

        //        var fileBytes = System.IO.File.ReadAllBytes(fullPath);
        //        var fileName = Path.GetFileName(fullPath);
        //        var contentType = "application/octet-stream";
        //        return File(fileBytes, contentType, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
        //    }
        //}

        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return BadRequest("File path is required.");

                if (!System.IO.File.Exists(path))
                    return NotFound("File not found.");

                // Lấy tên file từ đường dẫn
                var fileName = Path.GetFileName(path);

                // Đọc file thành byte[]
                var fileBytes = System.IO.File.ReadAllBytes(path);

                // Xác định kiểu content (MIME)
                var contentType = GetContentType(path);

                // Trả file về client
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error downloading file: {ex.Message}");
            }
        }

        // Hàm phụ: xác định ContentType theo phần mở rộng file
        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".txt" => MediaTypeNames.Text.Plain,
                ".pdf" => MediaTypeNames.Application.Pdf,
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".csv" => "text/csv",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => MediaTypeNames.Application.Octet
            };
        }


        //[HttpGet("open-folder")]
        //[Authorize]
        //public IActionResult OpenFolder(string path)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(path)) return BadRequest(ApiResponseFactory.Fail(null, $"Vui lòng nhập đường dẫn thư mục!"));

        //        // Kiểm tra thư mục tồn tại
        //        if (!Directory.Exists(path)) return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy thư mục [{path}]!"));

        //        // Mở thư mục bằng File Explorer (chạy trên máy server)
        //        Process.Start(new ProcessStartInfo()
        //        {
        //            FileName = "explorer.exe",
        //            Arguments = path,
        //            UseShellExecute = true
        //        });

        //        return Ok(ApiResponseFactory.Success(path, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex,ex.Message));
        //    }
        //}
    }
}
