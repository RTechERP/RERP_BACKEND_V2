using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static NPOI.HSSF.Util.HSSFColor;
using static RERPAPI.Model.DTO.ApproveTPDTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly RTCContext _context;
        private readonly IConfiguration _configuration;
        EmployeeOverTimeRepo _employeeOverTimeRepo;
        //UserRepo _userRepo = new UserRepo();
        vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly RoleConfig _roleConfig;
        private readonly EmployeePayrollDetailRepo _employeePayrollDetailRepo;

        private readonly EmployeeOnLeaveRepo _onLeaveRepo;
        private readonly EmployeeWFHRepo _wfhRepo;
        private readonly ConfigSystemRepo _configSystemRepo;

        //IRabbitMqPublisher _publisher;
        public HomeController(IOptions<JwtSettings> jwtSettings, RTCContext context, IConfiguration configuration, EmployeeOnLeaveRepo onLeaveRepo, vUserGroupLinksRepo vUserGroupLinksRepo, EmployeeWFHRepo employeeWFHRepo, ConfigSystemRepo configSystemRepo, EmployeeOverTimeRepo employeeOverTimeRepo, RoleConfig roleConfig, EmployeePayrollDetailRepo employeePayrollDetailRepo)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _configuration = configuration;
            _onLeaveRepo = onLeaveRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _wfhRepo = employeeWFHRepo;
            _configSystemRepo = configSystemRepo;
            _employeeOverTimeRepo = employeeOverTimeRepo;
            _roleConfig = roleConfig;
            _employeePayrollDetailRepo = employeePayrollDetailRepo;
            //_publisher = publisher;
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [ApiKeyAuthorize]
        [HttpPost("loginiden")]
        public IActionResult LoginIdentificaion([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.LoginName))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đăng nhập!"));
                }

                //1. Check user
                string loginName = user.LoginName ?? "";
                string apiKey = _configuration.GetValue<string>("ApiKey") ?? "";

                var login = SQLHelper<object>.ProcedureToList("spLogin", new string[] { "@LoginName", "@APIKey" }, new object[] { loginName, apiKey });
                var hasUsers = SQLHelper<object>.GetListData(login, 0);

                if (hasUsers.Count <= 0 || hasUsers[0].ID <= 0)
                {
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [Authorize]
        //[ApiKeyAuthorize]
        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                return Ok(ApiResponseFactory.Success(currentUser, ""));

                //string key = _configuration.GetValue<string>("SessionKey") ?? "";
                //CurrentUser currentUser = HttpContext.Session.GetObject<CurrentUser>(key);

                //return Ok(ApiResponseFactory.Success(currentUser, ""));
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
                        // var uniqueFileName = $"{originalFileName}{fileExtension}";



                        // Tạo tên file unique để tránh trùng lặp
                        var uniqueFileName = $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";


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

        [Authorize]
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

        //[HttpPost("send")]
        //public async Task<IActionResult> Send(EmployeeSendEmail e)
        //{
        //    await _publisher.PublishAsync(e);
        //    return Ok();
        //}



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
        //API Lấy danh sách bản ghi để duyệt TBP duyệt
        [HttpPost("get-approve-by-approve-tp")]

        public ActionResult GetApproveByApproveTP([FromBody] ApproveByApproveTPRequestParam request)

        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isBGD = currentUser.DepartmentID == 1 && currentUser.EmployeeID != 54;
                if (isBGD == true)
                {
                    request.IDApprovedTP = 0;

                }
                request.DateStart = request.DateStart.Value.ToLocalTime().Date;
                request.DateEnd = request.DateEnd.Value.ToLocalTime().Date.AddDays(+1).AddSeconds(-1);

                var approve = SQLHelper<dynamic>.ProcedureToList(
                    "spGetApprovedByApprovedTP_New",
                    new string[] { "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DeleteFlag", "@EmployeeID", "@TType", "@StatusHR", "@StatusBGD", "@IsBGD", "@UserTeamID", "@SeniorID", "@StatusSenior" },
                    new object[] { request.FilterText ?? "", request.DateStart, request.DateEnd, request.IDApprovedTP ?? 0, request.Status ?? 0, request.DeleteFlag ?? 0, request.EmployeeID ?? 0, request.TType ?? 0, request.StatusHR ?? 0, request.StatusBGD ?? 0, isBGD, request.UserTeamID ?? 0, request.SeniorID, request.StatusSenior });

                var listData = SQLHelper<dynamic>.GetListData(approve, 0);
                return Ok(ApiResponseFactory.Success(listData, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //private string? ValidateTBP(ApproveItemParam item, bool isApproved)
        //{
        //    // Id là int? => ép về int
        //    if ((item.ID ?? 0) <= 0)
        //        return "ID không hợp lệ.";

        //    if (item.DeleteFlag ?? false)
        //        return $"Nhân viên [{item.FullName}] đã tự xoá khai báo, không thể duyệt / hủy duyệt.";

        //    // IsApprovedHR là bool? => ép về bool
        //    if (!isApproved && (item.IsApprovedHR ?? false))
        //        return $"Nhân viên [{item.FullName}] đã được HR duyệt, không thể hủy duyệt TBP.";

        //    // IsCancelRegister là int? => ép về int
        //    if ((item.IsCancelRegister ) > 0)
        //        return $"Nhân viên [{item.FullName}] đã đăng ký hủy, không thể duyệt / hủy duyệt.";



        //    //if (!isApproved && !(item.IsApprovedTP ?? false))
        //    //    return $"Nhân viên [{item.FullName}] chưa được TBP duyệt, không thể hủy duyệt.";

        //    if (!isApproved && (item.IsApprovedBGD == true))
        //        return $"Nhân viên [{item.FullName}] đã được BGĐ duyệt, không thể hủy duyệt TBP.";

        //    return null;
        //}

        //private string? ValidateBgd(ApproveItemParam item, bool isApproved)
        //{
        //    if ((item.ID ?? 0) <= 0)
        //        return "ID không hợp lệ.";

        //    if (!(item.IsApprovedHR ?? false))
        //        return $"Nhân viên [{item.FullName}] chưa được HR duyệt, BGD không thể duyệt / hủy duyệt.";




        //    return null;
        //}

        //private string? ValidateSenior(ApproveItemParam item, bool isApproved)
        //{
        //    if ((item.ID ?? 0) <= 0)
        //        return "ID không hợp lệ.";

        //    //// TableName có thể null nhưng string.Equals static handle được null, nên giữ nguyên cũng ok
        //    //if (!string.Equals(item.TableName, "EmployeeOvertime", StringComparison.OrdinalIgnoreCase))
        //    //    return "Senior chỉ được duyệt cho đăng ký làm thêm (EmployeeOvertime).";

        //    if (item.IsApprovedBGD == true)
        //        return $"Nhân viên [{item.FullName}] đã được BGĐ duyệt, Senior không thể thay đổi.";



        //    //// Hủy duyệt mà chưa từng Senior duyệt
        //    //if (isApproved==false && (item.IsSeniorApproved != true))
        //    //    return $"Nhân viên [{item.FullName}] chưa được Senior duyệt, không thể hủy duyệt.";

        //    return null;
        //}
        //[HttpPost("save-approve-senior")]
        //public async Task<IActionResult> SaveApproveSenior(ApproveItemParam item, bool isApproved)
        //{
        //    try
        //    {
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
        //        string isApprovedText = isApproved ? "duyệt" : "hủy duyệt";
        //        if (item == null)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, $"Chưa có dữ liệu để {isApprovedText}"));
        //        }
        //        var dt = SQLHelper<dynamic>.ProcedureToList("spGetUserTeamLinkByLeaderID", new string[] { "@LeaderID" }, new object[] { currentUser.EmployeeID });
        //        var data = SQLHelper<object>.GetListData(dt, 0);
        //        var result = data.Cast<IDictionary<string, object>>().FirstOrDefault(x => x.ContainsKey("EmployeeID") && x["EmployeeID"] != null && Convert.ToInt32(x["EmployeeID"]) == item.EmployeeID);


        //        return Ok(ApiResponseFactory.Success(null, $"{isApprovedText} thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        //[RequiresPermission("N32")]
        //[HttpPost("approve-tbp")]
        //public IActionResult ApproveTBP([FromBody] ApproveRequestParam request)
        //{
        //    try
        //    {
        //        if (request == null || request.Items == null || request.Items.Count == 0)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phê duyệt không được để trống!"));
        //        }

        //        var notProcessed = new List<NotProcessedApprovalItem>();

        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        var currentUser = ObjectMapper.GetCurrentUser(claims);

        //        foreach (var item in request.Items)
        //        {
        //            var error = ValidateTBP(item, request.IsApproved ?? false);
        //            if (error != null)
        //            {
        //                notProcessed.Add(new NotProcessedApprovalItem
        //                {
        //                    Item = item,
        //                    Reason = error
        //                });
        //                continue;

        //            }

        //            SQLHelper<object>.ExcuteProcedure(
        //          "spUpdateTableByFieldNameAndID",
        //          new[] { "@TableName", "@FieldName", "@Value", "@ID", "@ValueUpdatedDate", "@ValueDecilineApprove", "@EvaluateResults" },
        //          new object[] { item.TableName, item.FieldName, item.IsApprovedTP, item.ID, item.ValueUpdatedDate, item.ValueDecilineApprove ?? "", item.EvaluateResults });

        //        }
        //        return Ok(ApiResponseFactory.Success(notProcessed, notProcessed.Count == 0 ? "Cập nhật thành công." : $"Cập nhật thành công, bỏ qua {notProcessed.Count} bản ghi."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        //[RequiresPermission("N32")]
        //[HttpPost("un-approve-tbp")]
        //public IActionResult UnApproveTBP([FromBody] ApproveRequestParam request)
        //{
        //    try
        //    {
        //        if (request == null || request.Items == null || request.Items.Count == 0)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phê duyệt không được để trống!"));
        //        }

        //        var notProcessed = new List<NotProcessedApprovalItem>();

        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        var currentUser = ObjectMapper.GetCurrentUser(claims);

        //        foreach (var item in request.Items)
        //        {
        //            if (item.TableName == "EmployeeWFH")
        //            {
        //                item.FieldName = "IsApproved";
        //            }
        //            if (item.TableName == "EmployeeOvertime")
        //            {
        //                item.FieldName = "IsApproved";
        //            }
        //            var error = ValidateTBP(item, request.IsApproved ?? false);
        //            if (error != null)
        //            {
        //                notProcessed.Add(new NotProcessedApprovalItem
        //                {
        //                    Item = item,
        //                    Reason = error
        //                });
        //                continue;

        //            }
        //            SQLHelper<object>.ExcuteProcedure("spUpdateTableByFieldNameAndID", new[] { "@TableName", "@FieldName", "@Value", "@ID", "@ValueUpdatedBy", "@ValueUpdatedDate", "@ValueDecilineApprove", "@Content", "@EvaluateResults" },
        //                new object[] { item.TableName, item.FieldName, item.IsApprovedTP.HasValue ? (item.IsApprovedTP.Value ? "1" : "0") : "0", item.ID, "", item.ValueUpdatedDate ?? "", item.ValueDecilineApprove ?? "", item.ReasonDeciline ?? "", item.EvaluateResults ?? "" });

        //        }
        //        return Ok(ApiResponseFactory.Success(notProcessed, notProcessed.Count == 0 ? "Cập nhật thành công." : $"Cập nhật thành công, bỏ qua {notProcessed.Count} bản ghi."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        //[RequiresPermission("N1")]
        //[HttpPost("approve-bgd")]
        //public IActionResult ApproveBGD([FromBody] ApproveRequestParam request)
        //{
        //    try
        //    {
        //        if (request == null || request.Items == null || request.Items.Count == 0)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phê duyệt không được để trống!"));
        //        }

        //        var notProcessed = new List<NotProcessedApprovalItem>();

        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        var currentUser = ObjectMapper.GetCurrentUser(claims);

        //        foreach (var item in request.Items)
        //        {
        //            var error = ValidateBgd(item, request.IsApproved ?? false);
        //            if (error != null)
        //            {
        //                notProcessed.Add(new NotProcessedApprovalItem
        //                {
        //                    Item = item,
        //                    Reason = error
        //                });
        //                continue;
        //            }
        //            SQLHelper<object>.ExcuteProcedure(
        //       "spUpdateTableByFieldNameAndID",
        //       new[] { "@TableName", "@FieldName", "@Value", "@ID", "@ValueUpdatedDate", "@ValueDecilineApprove", "@EvaluateResults" },
        //       new object[] { item.TableName, item.FieldName, request.IsApproved, item.ID, item.ValueUpdatedDate, item.ValueDecilineApprove ?? "", item.EvaluateResults }
        //   );

        //        }

        //        return Ok(ApiResponseFactory.Success(notProcessed, notProcessed.Count == 0 ? "Cập nhật thành công." : $"Cập nhật thành công, bỏ qua {notProcessed.Count} bản ghi."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        //[RequiresPermission("N85")]
        //[HttpPost("approve-senior")]
        //public IActionResult ApproveSenior([FromBody] ApproveRequestParam
        //    request)
        //{
        //    try
        //    {
        //        if (request == null || request.Items == null || request.Items.Count == 0)
        //        {
        //            return BadRequest(ApiResponseFactory.Fail(null, "Danh sách phê duyệt không được để trống!"));
        //        }
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        var currentUser = ObjectMapper.GetCurrentUser(claims);
        //        int seniorId = currentUser.EmployeeID;
        //        var notProcessed = new List<NotProcessedApprovalItem>();
        //        var dt = SQLHelper<dynamic>.ProcedureToList("spGetUserTeamLinkByLeaderID", new string[] { "@LeaderID" }, new object[] { currentUser.EmployeeID });
        //        var data = SQLHelper<object>.GetListData(dt, 0);
        //        var teamEmployeeIds = data.Cast<IDictionary<string, object>>().Where(x => x.ContainsKey("EmployeeID") && x["EmployeeID"] != null).Select(x => Convert.ToInt32(x["EmployeeID"])).ToHashSet(); // 
        //                                                                                                                                                                                                     //   var result = data.Cast<IDictionary<string, object>>().FirstOrDefault(x => x.ContainsKey("EmployeeID") && x["EmployeeID"] != null && Convert.ToInt32(x["EmployeeID"]) == request.Items.EmployeeID);
        //        foreach (var item in request.Items)
        //        {
        //            if ( !teamEmployeeIds.Contains(item.EmployeeID ?? 0))
        //            {
        //                notProcessed.Add(new NotProcessedApprovalItem   
        //                {
        //                    Item = item,
        //                    Reason = "Nhân viên không thuộc team của bạn"
        //                });
        //                continue;
        //            }
        //            var error = ValidateSenior(item, request.IsApproved ?? false);
        //            if (error != null)
        //            {
        //                notProcessed.Add(new NotProcessedApprovalItem
        //                {
        //                    Item = item,
        //                    Reason = error
        //                });
        //                continue;
        //            }
        //            SQLHelper<object>.ExcuteProcedure(
        //       "spUpdateTableByFieldNameAndID",
        //       new[] { "@TableName", "@FieldName", "@Value", "@ID", "@ValueUpdatedDate", "@ValueDecilineApprove", "@EvaluateResults" },
        //       new object[] { item.TableName, item.FieldName, item.IsSeniorApproved, item.ID, item.ValueUpdatedDate, item.ValueDecilineApprove ?? "", item.EvaluateResults });
        //        }
        //        return Ok(ApiResponseFactory.Success(notProcessed, notProcessed.Count == 0 ? "Cập nhật thành công." : $"Cập nhậtthành công, bỏ qua {notProcessed.Count} bản ghi."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        [HttpGet("get-personal-synthetic-by-month")]
        public IActionResult GetPersonalSyntheticByMonth(int year, int month)
        {
            try
            {

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);


                DateTime dateStart = new DateTime(year, month, 1, 0, 0, 0);
                DateTime dateEnd = dateStart.AddMonths(1).AddSeconds(-1);

                var listSummary = SQLHelper<object>.ProcedureToList("spGetPersonalSyntheticByMonth",
                    new string[] { "@Year", "@Month", "@EmployeeID" },
                    new object[] { year, month, currentUser.EmployeeID });

                var payrollData = SQLHelper<object>.ProcedureToList("spGetEmployeePayrollDetail",
                    new string[] { "@Year", "@Month", "@DepartmentID", "@EmployeeID", "@Keyword", "@IsPublish", "@IsAll" },
                    new object[] { year, month, currentUser.DepartmentID, currentUser.EmployeeID, "", 1, 0 });
                var payroll = SQLHelper<object>.GetListData(payrollData, 0);
                var rawFingerData = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeAttendance",
                    new string[] { "@DepartmentID", "@EmployeeID", "@FindText", "@DateStart", "@DateEnd" },
                    new object[] { currentUser.DepartmentID, currentUser.EmployeeID, "", dateStart, dateEnd });
                var listFingerDetails = SQLHelper<object>.GetListData(rawFingerData, 0);

                var summaryFinger = SQLHelper<object>.GetListData(rawFingerData, 1).FirstOrDefault();

                var fingers = new
                {

                    data = summaryFinger,
                    details = listFingerDetails
                };
                var rawChamCongData = SQLHelper<dynamic>.ProcedureToList("spGetChamCongNew",
                    new string[] { "@Month", "@Year", "@EmployeeID" },
                    new object[] { month, year, currentUser.EmployeeID });
                var chamcongInfo = SQLHelper<object>.GetListData(rawChamCongData, 0).FirstOrDefault();
                var dynamicListTable1 = SQLHelper<dynamic>.GetListData(rawChamCongData, 1);
                var rowChamCongChiTiet = dynamicListTable1 != null ? dynamicListTable1.FirstOrDefault() : null;
                var dynamicListTable2 = SQLHelper<dynamic>.GetListData(rawChamCongData, 2);
                var rowTotalWork = dynamicListTable2 != null ? dynamicListTable2.FirstOrDefault() : null;
                IDictionary<string, object> dictChamCong = null;
                if (rowChamCongChiTiet != null)
                {

                    if (rowChamCongChiTiet is IDictionary<string, object>)
                        dictChamCong = (IDictionary<string, object>)rowChamCongChiTiet;
                    else
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(rowChamCongChiTiet);
                        dictChamCong = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    }
                }
                int totalWorkDay = 0;
                if (rowTotalWork != null)
                {
                    IDictionary<string, object> dictTotal = null;
                    if (rowTotalWork is IDictionary<string, object>) dictTotal = (IDictionary<string, object>)rowTotalWork;
                    else dictTotal = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Newtonsoft.Json.JsonConvert.SerializeObject(rowTotalWork));

                    if (dictTotal != null && dictTotal.ContainsKey("TotalWorkDay"))
                        totalWorkDay = TextUtils.ToInt32(dictTotal["TotalWorkDay"]);
                }


                var listHeaderChamCong = new List<object>();
                var listDates = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                                          .Select(day => new DateTime(year, month, day))
                                          .ToList();

                foreach (var item in listDates)
                {
                    string key = $"D{item.Day}";
                    string rawValue = "";
                    if (dictChamCong != null && dictChamCong.ContainsKey(key))
                    {
                        rawValue = TextUtils.ToString(dictChamCong[key]);
                    }
                    var parts = rawValue.Split(';');
                    string textShow = parts.Length > 0 ? parts[0] : "";
                    int statusWork = parts.Length > 1 ? TextUtils.ToInt32(parts[1]) : 0;

                    listHeaderChamCong.Add(new
                    {
                        fieldname = key,
                        text = $"{item.Day}<br />{textShow}",
                        statuswork = statusWork,
                    });
                }
                var listChamcongDetail = new List<object>();
                DateTime firstDateInMonth = new DateTime(year, month, 1);
                DateTime lastDateInMonth = firstDateInMonth.AddMonths(1).AddDays(-1);
                DateTime dateValue = firstDateInMonth.AddDays(-(int)firstDateInMonth.DayOfWeek + (int)DayOfWeek.Monday);

                for (int i = 0; i < (7 * 6); i++)
                {
                    DateTime currentDate = dateValue.AddDays(i);
                    bool isValidDay = (currentDate >= firstDateInMonth && currentDate <= lastDateInMonth);

                    string key = $"D{currentDate.Day}";
                    string rawValue = "";
                    if (isValidDay && dictChamCong != null && dictChamCong.ContainsKey(key))
                    {
                        rawValue = TextUtils.ToString(dictChamCong[key]);
                    }

                    var parts = rawValue.Split(';');
                    int statusWork = parts.Length > 1 ? TextUtils.ToInt32(parts[1]) : 0;

                    listChamcongDetail.Add(new
                    {
                        value = currentDate,
                        fieldname = isValidDay ? key : "",
                        text = currentDate.Day,
                        disabled = !isValidDay,
                        statuswork = statusWork
                    });
                }

                var listChamcong = new
                {
                    header = listHeaderChamCong,
                    data = chamcongInfo,
                    totalworkday = totalWorkDay,
                    detail = listChamcongDetail
                };
                var result = new
                {
                    listSummary = listSummary,
                    fingers = fingers,
                    payroll = payroll,
                    listChamcong = listChamcong
                };

                return Ok(ApiResponseFactory.Success(result));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-user-team")]
        public IActionResult GetUserTeam()
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentQuarter = (DateTime.Now.Month - 1) / 3 + 1;
                var team = SQLHelper<object>.ProcedureToList("spGetUserTeam",
                                                new string[] { "@DepartmentID" },
                                                new object[] { 0 });
                var data = SQLHelper<object>.GetListData(team, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpGet("get-user-team-link-by-leader-id")]
        public IActionResult GetUserTeamLinkByLeaderID()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var team = SQLHelper<object>.ProcedureToList("spGetUserTeamLinkByLeaderID",
                                                new string[] { "@LeaderID" },
                                                new object[] { currentUser.EmployeeID });
                var data = SQLHelper<object>.GetListData(team, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [HttpGet("get-all-contact")]
        public IActionResult GetAllContact(int departmentID, string? keyword)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isPermissDownload = (_roleConfig.EmployeeDownloadContact?.Contains(currentUser.EmployeeID) ?? false) || currentUser.IsAdmin == true;
                keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
                var list = SQLHelper<EmployeeContactDTO>.ProcedureToListModel("spGetEmployee",
                                                                new string[] { "@Status", "@DepartmentID", "@Keyword" },
                                                                new object[] { 0, departmentID, keyword ?? "" })
                                                .Select(x => new
                                                {
                                                    x.STT,
                                                    x.FullName,
                                                    x.DepartmentName,
                                                    x.EmployeeTeamName,
                                                    x.ChucVu,
                                                    x.SDTCaNhan,
                                                    x.EmailCongTy,
                                                    x.StartWorking,
                                                    x.BirthOfDate,
                                                    x.Code
                                                });


                return Ok(ApiResponseFactory.Success(new { list, isPermissDownload }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all-team-new")]
        public IActionResult GetAllTeamNew(int deID)
        {

            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var orgCharts = SQLHelper<dynamic>.ProcedureToList("spGetOrganizationalChart",
                                                                            new string[] { "@TaxCompanyID", "@DepartmentID", "@Keyword" },
                                                                            new object[] { 1, deID, "" });
                var orgdChart = SQLHelper<dynamic>.ProcedureToList("spGetOrganizationalChartDetail", new string[] { "@ID" }, new object[] { 0 });
                var dt = SQLHelper<object>.GetListData(orgCharts, 0);
                var dtDetail = SQLHelper<object>.GetListData(orgdChart, 0);


                return Ok(ApiResponseFactory.Success(new { dt, dtDetail }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-quantity-approve")]
        public IActionResult GetQuantityApprove([FromBody] ApproveByApproveTPRequestParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                bool isBGD = currentUser.DepartmentID == 1 && currentUser.EmployeeID != 54;

                request.DateStart = request.DateStart.Value.ToLocalTime().Date;
                request.DateEnd = request.DateEnd.Value.ToLocalTime().Date.AddDays(+1).AddSeconds(-1);
                var approveResultSenior = SQLHelper<dynamic>.ProcedureToList(
                    "spGetApprovedByApprovedTP_New",
                    new[] { "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DeleteFlag", "@EmployeeID", "@TType", "@StatusHR", "@StatusBGD", "@IsBGD", "@UserTeamID", "@SeniorID", "@StatusSenior" },
                    new object[] { "", request.DateStart, request.DateEnd, 0, -1, 0, 0, 0, -1, 0, false, 0, currentUser.EmployeeID, 0 });
                var approveResultTP = SQLHelper<dynamic>.ProcedureToList(
                   "spGetApprovedByApprovedTP_New",
                   new[] { "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DeleteFlag", "@EmployeeID", "@TType", "@StatusHR", "@StatusBGD", "@IsBGD", "@UserTeamID", "@SeniorID", "@StatusSenior" },
                   new object[] { "", request.DateStart, request.DateEnd, currentUser.EmployeeID, 0, 0, 0, 0, -1, 0, false, 0, 0, -1 });
                var approveResultBGD = SQLHelper<dynamic>.ProcedureToList(
                   "spGetApprovedByApprovedTP_New",
                   new[] { "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DeleteFlag", "@EmployeeID", "@TType", "@StatusHR", "@StatusBGD", "@IsBGD", "@UserTeamID", "@SeniorID", "@StatusSenior" },
                   new object[] { "", request.DateStart, request.DateEnd, currentUser.EmployeeID, 0, 0, 0, 0, -1, 0, isBGD, 0, 0, -1 });
                var approveListSenior = SQLHelper<dynamic>.GetListData(approveResultSenior, 0);
                var approveListTP = SQLHelper<dynamic>.GetListData(approveResultTP, 0);
                var approveListBGD = SQLHelper<dynamic>.GetListData(approveResultBGD, 0);
                var result = new[]
                        {
                            new { Type = "Senior", Count = approveListSenior?.Count ?? 0 },
                            new { Type = "TP",     Count = approveListTP?.Count ?? 0 },
                            new { Type = "BGD",    Count = approveListBGD?.Count ?? 0 }
                        }.FirstOrDefault(x => x.Count > 0);
                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //[HttpPost("save-approve")]
        //public IActionResult GetQuantityApprove([FromBody] ApproveByApproveTPRequestParam request)
        //{
        //    try
        //    {
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        var currentUser = ObjectMapper.GetCurrentUser(claims);

        //        bool isBGD = currentUser.DepartmentID == 1 && currentUser.EmployeeID != 54;

        //        request.DateStart = request.DateStart.Value.ToLocalTime().Date;
        //        request.DateEnd = request.DateEnd.Value.ToLocalTime().Date.AddDays(+1).AddSeconds(-1);
        //        var approveResultSenior = SQLHelper<dynamic>.ProcedureToList(
        //            "spGetApprovedByApprovedTP_New",
        //            new[] { "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DeleteFlag", "@EmployeeID", "@TType", "@StatusHR", "@StatusBGD", "@IsBGD", "@UserTeamID", "@SeniorID", "@StatusSenior" },
        //            new object[] { "", request.DateStart, request.DateEnd, 0, -1, 0, 0, 0, -1, 0, false, 0, currentUser.EmployeeID, 0 });
        //        var approveResultTP = SQLHelper<dynamic>.ProcedureToList(
        //           "spGetApprovedByApprovedTP_New",
        //           new[] { "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DeleteFlag", "@EmployeeID", "@TType", "@StatusHR", "@StatusBGD", "@IsBGD", "@UserTeamID", "@SeniorID", "@StatusSenior" },
        //           new object[] { "", request.DateStart, request.DateEnd, currentUser.EmployeeID, 0, 0, 0, 0, -1, 0, false, 0, 0, -1 });
        //        var approveResultBGD = SQLHelper<dynamic>.ProcedureToList(
        //           "spGetApprovedByApprovedTP_New       ",
        //           new[] { "@FilterText", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DeleteFlag", "@EmployeeID", "@TType", "@StatusHR", "@StatusBGD", "@IsBGD", "@UserTeamID", "@SeniorID", "@StatusSenior" },
        //           new object[] { "", request.DateStart, request.DateEnd, currentUser.EmployeeID, 0, 0, 0, 0, -1, 0, isBGD, 0, 0, -1 });
        //        var approveListSenior = SQLHelper<dynamic>.GetListData(approveResultSenior, 0);
        //        var approveListTP = SQLHelper<dynamic>.GetListData(approveResultTP, 0);
        //        var approveListBGD = SQLHelper<dynamic>.GetListData(approveResultBGD, 0);
        //        var result = new[]
        //                {
        //                    new { Type = "Senior", Count = approveListSenior?.Count ?? 0 },
        //                    new { Type = "TP",     Count = approveListTP?.Count ?? 0 },
        //                    new { Type = "BGD",    Count = approveListBGD?.Count ?? 0 }
        //                }.FirstOrDefault(x => x.Count > 0);
        //        return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("confirm-payroll")]
        public IActionResult ConfirmPayroll([FromBody] ConfirmPayrollDTO dto)
        {
            try
            {
                var payroll = _employeePayrollDetailRepo.GetByID(dto.Id);
                if (payroll == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bảng lương"));

                payroll.Sign = dto.Sign;
                _employeePayrollDetailRepo.Update(payroll);

                return Ok(ApiResponseFactory.Success(null, "Xác nhận bảng lương thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-summary-employee-person")]
        public IActionResult GetProposeVehicleRepair([FromBody] SummaryPersonal request)
        {
            try
            {
                request.DateStart = request.DateStart.ToLocalTime().Date;
                request.DateEnd = request.DateEnd.ToLocalTime().Date.AddDays(+1).AddSeconds(-1);
                string procedureOnLeave = "spGetEmployeeOnLeaveInWeb";
                string procedureEarlyLate = "spGetEmployeeEarlyLateInWeb";
                string procedureOverTime = "spGetEmployeeOvertimeInWeb";
                string procedureBussiness = "spGetEmployeeBussinessInWeb";
                string procedureOnWFH = "spGetEmployeeWFHInWeb";
                string procedureENF = "spGetEmployeeNoFingerprintInWeb";
                string procedureNightShift = "spGetEmployeeNightShift";
                string[] paramNames = new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@IsApproved", "@Keyword" };
                object[] paramValues = new object[] { request.DateStart, request.DateEnd, request.DepartmentID ?? 0, request.EmployeeID ?? 0, request.IsApproved, request.Keyword ?? "" };
                string[] paramNamesNightShift = new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@IsApproved", "@Keyword", "@PageNumber", "@PageSize" };
                object[] paramValuesNightShift = new object[] { request.DateStart, request.DateEnd, request.DepartmentID ?? 0, request.EmployeeID ?? 0, request.IsApproved, request.Keyword ?? "", 1, 10000000 };
                string[] paramNamesEarlyLate = new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@IsApproved", "@Keyword", "@Type" };
                object[] paramValuesEarlyLate = new object[] { request.DateStart, request.DateEnd, request.DepartmentID ?? 0, request.EmployeeID ?? 0, request.IsApproved, request.Keyword ?? "", 0 };
                string[] paramNameBuissiness = new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@IsApproved", "@Keyword", "@Type", "@VehicleID", "@NotCheckIn" };
                object[] paramValueBuissiness = new object[] { request.DateStart, request.DateEnd, request.DepartmentID ?? 0, request.EmployeeID ?? 0, request.IsApproved, request.Keyword ?? "", 0, 0, -1 };

                //string[] paramNameBuissiness = new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@EmployeeID", "@IsApproved", "@Keyword", "@Type", "@NotCheckIn" };
                //    object[] paramValueBuissiness = new object[] { request.DateStart, request.DateEnd, request.DepartmentID ?? 0, request.EmployeeID ?? 0, request.IsApproved, request.Keyword ?? "", 0, -1};


                var dataOnLeave = SQLHelper<object>.ProcedureToList(procedureOnLeave, paramNames, paramValues);
                var dataEarlyLate = SQLHelper<object>.ProcedureToList(procedureEarlyLate, paramNamesEarlyLate, paramValuesEarlyLate);
                var dataOverTime = SQLHelper<object>.ProcedureToList(procedureOverTime, paramNamesEarlyLate, paramValuesEarlyLate);
                var dataBussiness = SQLHelper<object>.ProcedureToList(procedureBussiness, paramNameBuissiness, paramValueBuissiness);
                var dataWFH = SQLHelper<object>.ProcedureToList(procedureOnWFH, paramNames, paramValues);
                var dataENF = SQLHelper<object>.ProcedureToList(procedureENF, paramNames, paramValues);
                var dataNightShiftData = SQLHelper<dynamic>.ProcedureToList(procedureNightShift, paramNamesNightShift, paramValuesNightShift);

                var dataNightShift = SQLHelper<dynamic>.GetListData(dataNightShiftData, 0);

                return Ok(ApiResponseFactory.Success(new { dataOnLeave, dataEarlyLate, dataOverTime, dataBussiness, dataWFH, dataENF, dataNightShift }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-config-system-hr")]
        public IActionResult GetConfigSystem()
        {
            try
            {
                var data = _configSystemRepo.GetAll(x => x.KeyName== "EmployeeOvertime"||x.KeyName== "EmployeeBussiness");
                return Ok(ApiResponseFactory.Success(new {data }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }


        }
    }
}
