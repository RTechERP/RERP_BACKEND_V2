using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MechanicalDrawingController : ControllerBase
    {
        private readonly MechanicalDrawingRepo _mechanicalDrawingRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly CurrentUser _currentUser;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly ProjectTypeRepo _projectTypeRepo;

        public MechanicalDrawingController(
            MechanicalDrawingRepo mechanicalDrawingRepo,
            ProjectRepo projectRepo,
            CurrentUser currentUser,
            ConfigSystemRepo configSystemRepo,
            ProjectTypeRepo projectTypeRepo)
        {
            _mechanicalDrawingRepo = mechanicalDrawingRepo;
            _projectRepo = projectRepo;
            _currentUser = currentUser;
            _configSystemRepo = configSystemRepo;
            _projectTypeRepo = projectTypeRepo;
        }

        [HttpGet("get-projects")]
        public IActionResult GetProjects()
        {
            try
            {
                var data = _projectRepo.GetAll(x => x.IsDeleted != true).OrderByDescending(x => x.CreatedDate).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-project-types")]
        public IActionResult GetProjectTypes()
        {
            try
            {
                var data = _projectTypeRepo.GetAll(x => x.IsDeleted != true).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[RequiresPermission("N114,N1")]
        [HttpGet("get-mechanical-drawings")]
        public IActionResult GetMechanicalDrawings(int page, int size, int? projectId, int? projectTypeId, string keyword = "", bool isDeleted = false)
        {
            try
            {
                var query = _mechanicalDrawingRepo.GetAll(x => isDeleted ? x.IsDeleted == true : x.IsDeleted != true);
                if (projectId.HasValue && projectId > 0)
                    query = query.Where(x => x.ProjectID == projectId).ToList();
                if (projectTypeId.HasValue && projectTypeId > 0)
                    query = query.Where(x => x.ProjectTypeID == projectTypeId).ToList();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var keywordLower = keyword.Trim().ToLower();
                    query = query.Where(x => x.Name != null && x.Name.ToLower().Contains(keywordLower)).ToList();
                }

                var total = query.Count;
                var list = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * size).Take(size).ToList();

                var projectIds = list.Select(x => x.ProjectID).Distinct().ToList();
                var projects = _projectRepo.GetAll(x => projectIds.Contains(x.ID)).ToList();

                var projectTypeIds = list.Select(x => x.ProjectTypeID).Where(x => x.HasValue).Select(x => x.Value).Distinct().ToList();
                var projectTypes = _projectTypeRepo.GetAll(x => projectTypeIds.Contains(x.ID)).ToList();

                var data = list.Select(x => {
                    var proj = projects.FirstOrDefault(p => p.ID == x.ProjectID);
                    var projType = projectTypes.FirstOrDefault(pt => pt.ID == x.ProjectTypeID);
                    return new {
                        x.ID,
                        x.Name,
                        x.ProjectID,
                        ProjectName = proj?.ProjectName ?? "",
                        x.ProjectTypeID,
                        ProjectTypeName = projType?.ProjectTypeName ?? "",
                        x.FilePath,
                        x.ThumbnailPath,
                        x.CreatedBy,
                        x.CreatedDate,
                        x.UpdatedBy,
                        x.UpdatedDate,
                        x.IsDeleted
                    };
                }).ToList();

                return Ok(ApiResponseFactory.Success(new { data, total }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-mechanical-drawing-detail")]
        public IActionResult GetMechanicalDrawingDetail(int id)
        {
            try
            {
                var data = _mechanicalDrawingRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveAsync(MechanicalDrawingSaveDTO req)
        {
            try
            {
                var validateMsg = Validate(req.mechanicalDrawing);
                if (!string.IsNullOrEmpty(validateMsg))
                    return BadRequest(ApiResponseFactory.Fail(null, validateMsg));

                var model = req.mechanicalDrawing;

                if (req.mechanicalDrawing.ID > 0)
                {
                    var exist = _mechanicalDrawingRepo.GetByID(req.mechanicalDrawing.ID);
                    if (exist == null)
                        return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi không tồn tại"));
                    exist.Name = model.Name?.Trim();
                    exist.ProjectID = model.ProjectID;
                    exist.ProjectTypeID = model.ProjectTypeID;
                    exist.FilePath = model.FilePath?.Trim();
                    await _mechanicalDrawingRepo.UpdateAsync(exist);
                    return Ok(ApiResponseFactory.Success(exist, "Cập nhật thành công"));
                }
                else
                {
                    var newModel = new MechanicalDrawing
                    {
                        Name = model.Name?.Trim(),
                        ProjectID = model.ProjectID,
                        ProjectTypeID = model.ProjectTypeID,
                        FilePath = model.FilePath?.Trim(),
                    };
                    await _mechanicalDrawingRepo.CreateAsync(newModel);
                    return Ok(ApiResponseFactory.Success(newModel, "Thêm mới thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete-mechanical-drawing")]
        public async Task<IActionResult> DeleteMechanicalDrawing(int id)
        {
            try
            {
                var model = _mechanicalDrawingRepo.GetByID(id);
                if (model == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi không tồn tại"));

                //if (_currentUser.IsAdmin == false && _currentUser.LoginName.Trim() != model.CreatedBy?.Trim())
                //    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền xóa bản ghi này"));

                model.IsDeleted = true;
                await _mechanicalDrawingRepo.UpdateAsync(model);

                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("restore-mechanical-drawing")]
        public async Task<IActionResult> RestoreMechanicalDrawing(int id)
        {
            try
            {
                var model = _mechanicalDrawingRepo.GetByID(id);
                if (model == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản ghi không tồn tại"));

                model.IsDeleted = false;
                await _mechanicalDrawingRepo.UpdateAsync(model);

                return Ok(ApiResponseFactory.Success(null, "Khôi phục thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("upload-file")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(int id)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var files = form.Files;

                if (id <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "File không được để trống"));

                var drawing = _mechanicalDrawingRepo.GetByID(id);
                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ cơ khí"));

                if (drawing.ProjectID == null || drawing.ProjectID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản vẽ chưa gắn với dự án, không thể upload"));

                var project = _projectRepo.GetByID(drawing.ProjectID.Value);
                if (project == null || project.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dự án tương ứng"));

                var uploadPath = _configSystemRepo.GetUploadPathByKey("DRAWING_PATH");
                //var uploadPath = _configSystemRepo.GetUploadPathByKey("MechanicalDrawing");
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy cấu hình đường dẫn cho key: DRAWING_PATH"));

                int year = project.CreatedDate?.Year ?? DateTime.Now.Year;
                string rawProjectCode = (project.ProjectCode ?? string.Empty).Trim();
                string projectCode = SanitizeFolderSegment(string.IsNullOrEmpty(rawProjectCode)
                    ? $"Project_{project.ID}"
                    : rawProjectCode);

                string targetFolder = Path.Combine(uploadPath, year.ToString(), projectCode, "TaiLieuChung", "GiaiPhap", "BanVeCoKhi");

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var file = files[0];
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var safeFileName = string.Join("_", originalFileName.Split(Path.GetInvalidFileNameChars()));
                    var uniqueFileName = $"{safeFileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    drawing.FilePath = fullPath;
                    drawing.UpdatedDate = DateTime.Now;
                    await _mechanicalDrawingRepo.UpdateAsync(drawing);

                    return Ok(ApiResponseFactory.Success(new
                    {
                        FileName = uniqueFileName,
                        FilePath = fullPath,
                        drawing.ID
                    }, "Upload file thành công"));
                }

                return BadRequest(ApiResponseFactory.Fail(null, "File trống"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }

        [HttpGet("get-file-path")]
        public IActionResult GetFilePath(int id)
        {
            try
            {
                var drawing = _mechanicalDrawingRepo.GetByID(id);
                if (drawing == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                if (string.IsNullOrWhiteSpace(drawing.FilePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản vẽ chưa có đường dẫn file"));

                return Ok(ApiResponseFactory.Success(new { FilePath = drawing.FilePath }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("download-file")]
        public IActionResult DownloadFile(int id)
        {
            try
            {
                var drawing = _mechanicalDrawingRepo.GetByID(id);
                if (drawing == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                if (string.IsNullOrWhiteSpace(drawing.FilePath) || !System.IO.File.Exists(drawing.FilePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "File không tồn tại trên server"));

                var memory = new MemoryStream();
                using (var stream = new FileStream(drawing.FilePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                var contentType = GetContentType(drawing.FilePath);
                return File(memory, contentType, Path.GetFileName(drawing.FilePath));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Preview file HTML trực tiếp từ đường dẫn trên server.
        /// Dùng PhysicalFile để stream file lớn (>50MB) mà không load hết vào memory.
        /// Hỗ trợ các định dạng: .html, .htm, .svg, .xml, .txt, .pdf, .json
        /// </summary>
        /// <summary>
        /// Serve thumbnail image. Returns 204 No Content if no thumbnail exists.
        /// </summary>
        [HttpGet("thumbnail/{id}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public IActionResult GetThumbnail(int id)
        {
            try
            {
                var drawing = _mechanicalDrawingRepo.GetByID(id);
                if (drawing == null)
                    return NotFound("Không tìm thấy bản vẽ");

                if (string.IsNullOrWhiteSpace(drawing.ThumbnailPath) || !System.IO.File.Exists(drawing.ThumbnailPath))
                    return NoContent(); // 204 - không có thumbnail

                Response.Headers["X-Content-Type-Options"] = "nosniff";
                return PhysicalFile(drawing.ThumbnailPath, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("preview-file/{id}")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public IActionResult PreviewFile(int id)
        {
            try
            {
                var drawing = _mechanicalDrawingRepo.GetByID(id);
                if (drawing == null)
                    return NotFound("Không tìm thấy bản vẽ");

                if (string.IsNullOrWhiteSpace(drawing.FilePath))
                    return NotFound("Bản vẽ chưa có file");

                if (!System.IO.File.Exists(drawing.FilePath))
                    return NotFound("File không tồn tại trên server");

                var ext = Path.GetExtension(drawing.FilePath).ToLowerInvariant();
                var previewableExts = new HashSet<string> { ".html", ".htm", ".svg", ".xml", ".txt", ".json", ".pdf", ".css", ".js" };
                if (!previewableExts.Contains(ext))
                    return BadRequest($"Định dạng file '{ext}' không hỗ trợ preview");

                var contentType = GetContentType(drawing.FilePath);

                // PhysicalFile dùng ZeroCopy (IIS/Kestrel serve trực tiếp từ disk)
                // -> Không load file vào RAM, phù hợp file lớn >50MB
                Response.Headers["X-Content-Type-Options"] = "nosniff";
                return PhysicalFile(drawing.FilePath, contentType);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".html", "text/html"},
                {".htm", "text/html"},
                {".svg", "image/svg+xml"},
                {".xml", "application/xml"},
                {".json", "application/json"},
                {".css", "text/css"},
                {".js", "application/javascript"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".dwg", "image/vnd.dwg"},
                {".dxf", "image/vnd.dxf"}
            };
        }

        private static string SanitizeFolderSegment(string segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
                return "Unknown";

            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(segment.Where(c => !invalidChars.Contains(c)).ToArray());
            cleaned = cleaned.Replace("..", "").Trim();

            return string.IsNullOrEmpty(cleaned) ? "Unknown" : cleaned;
        }

        private string? Validate(MechanicalDrawing model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return "Vui lòng nhập tên bản vẽ";

            if (!model.ProjectID.HasValue || model.ProjectID <= 0)
                return "Vui lòng chọn Dự án";

            return null;
        }

        public class MechanicalDrawingSaveDTO
        {
            public MechanicalDrawing mechanicalDrawing { get; set; }
        }

        /// <summary>
        /// Lưu thumbnail (ảnh base64) cho bản vẽ. Gọi từ frontend sau khi upload file.
        /// </summary>
        [HttpPost("save-thumbnail")]
        public async Task<IActionResult> SaveThumbnail([FromBody] SaveThumbnailRequest request)
        {
            try
            {
                if (request == null || request.DrawingId <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "ID không hợp lệ"));

                if (string.IsNullOrWhiteSpace(request.Base64Image))
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu ảnh trống"));

                var drawing = _mechanicalDrawingRepo.GetByID(request.DrawingId);
                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                if (drawing.ProjectID == null || drawing.ProjectID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản vẽ chưa gắn với dự án, không thể lưu thumbnail"));

                var project = _projectRepo.GetByID(drawing.ProjectID.Value);
                if (project == null || project.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dự án tương ứng"));

                // Parse base64 → lưu thành file PNG
                byte[] imageBytes;
                try
                {
                    var base64Data = request.Base64Image.Contains(',')
                        ? request.Base64Image.Split(',')[1]
                        : request.Base64Image;
                    imageBytes = Convert.FromBase64String(base64Data);
                }
                catch
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu base64 không hợp lệ"));
                }

                // Tạo đường dẫn thumbnail theo cùng cấu trúc thư mục mã dự án như upload-file
                var uploadPath = _configSystemRepo.GetUploadPathByKey("DRAWING_PATH");
                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy cấu hình đường dẫn"));

                int year = project.CreatedDate?.Year ?? DateTime.Now.Year;
                string rawProjectCode = (project.ProjectCode ?? string.Empty).Trim();
                string projectCode = SanitizeFolderSegment(string.IsNullOrEmpty(rawProjectCode)
                    ? $"Project_{project.ID}"
                    : rawProjectCode);

                string thumbnailFolder = Path.Combine(uploadPath, year.ToString(), projectCode, "TaiLieuChung", "GiaiPhap", "BanVeCoKhi", "Thumbnails");
                if (!Directory.Exists(thumbnailFolder))
                    Directory.CreateDirectory(thumbnailFolder);

                var thumbnailFileName = $"thumb_{drawing.ID}_{DateTime.Now:yyyyMMddHHmmssfff}.png";
                var thumbnailPath = Path.Combine(thumbnailFolder, thumbnailFileName);

                await System.IO.File.WriteAllBytesAsync(thumbnailPath, imageBytes);

                // Xóa thumbnail cũ nếu có
                if (!string.IsNullOrWhiteSpace(drawing.ThumbnailPath) && System.IO.File.Exists(drawing.ThumbnailPath))
                {
                    try { System.IO.File.Delete(drawing.ThumbnailPath); } catch { }
                }

                // Cập nhật đường dẫn thumbnail
                drawing.ThumbnailPath = thumbnailPath;
                drawing.UpdatedDate = DateTime.Now;
                await _mechanicalDrawingRepo.UpdateAsync(drawing);

                return Ok(ApiResponseFactory.Success(new { ThumbnailPath = thumbnailPath }, "Lưu thumbnail thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SaveThumbnailRequest
        {
            public int DrawingId { get; set; }
            public string Base64Image { get; set; }
        }
    }
}
