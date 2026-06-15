using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DrawingController : ControllerBase
    {
        private readonly DrawingRepo _drawingRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly ProjectTypeRepo _projectTypeRepo;
        private readonly DrawingLogRepo _drawingLogRepo;

        public DrawingController(
            DrawingRepo drawingRepo,
            ConfigSystemRepo configSystemRepo,
            EmployeeRepo employeeRepo,
            ProjectTypeRepo projectTypeRepo,
            DrawingLogRepo drawingLogRepo)
        {
            _drawingRepo = drawingRepo;
            _configSystemRepo = configSystemRepo;
            _employeeRepo = employeeRepo;
            _projectTypeRepo = projectTypeRepo;
            _drawingLogRepo = drawingLogRepo;
        }

        #region Get data

        [HttpGet("get-project-type")]
        public IActionResult GetProjectType()
        {
            try
            {
                var rs = _projectTypeRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult GetEmployees()
        {
            try
            {
                var result = _employeeRepo.GetAll(x => x.Status == 0) // Giả sử Status = 0 là đang hoạt động
                    .Select(x => new
                    {
                        ID = x.ID,
                        UserID = x.UserID,
                        FullName = x.FullName,
                        Code = x.Code
                    }).ToList();

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-data")]
        public IActionResult GetData(int projectID = 0, string keyword = "")
        {
            try
            {
                var query = _drawingRepo.GetAll()
                    .Where(x => x.IsDeleted != true);

                if (projectID > 0)
                    query = query.Where(x => x.ProjectID == projectID);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(x =>
                        x.DrawingName.ToLower().Contains(keyword) ||
                        x.Version.ToLower().Contains(keyword));
                }

                var drawings = query.OrderByDescending(x => x.ID).ToList();

                var employeeIds = drawings
                    .SelectMany(x => new int?[] { x.DesignByID, x.CheckedByID, x.ApprovedByID })
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .Distinct()
                    .ToList();

                var employees = _employeeRepo
                    .GetAll(x => employeeIds.Contains(x.ID))
                    .Select(x => new
                    {
                        x.ID,
                        x.FullName
                    })
                    .ToList();

                var projectTypeIds = drawings
                    .Select(x => x.ProjectTypeID)
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .Distinct()
                    .ToList();

                var projectTypes = _projectTypeRepo
                    .GetAll(x => projectTypeIds.Contains(x.ID))
                    .Select(x => new
                    {
                        x.ID,
                        x.ProjectTypeCode,
                        x.ProjectTypeName
                    })
                    .ToList();

                var data = drawings.Select(x =>
                {
                    var designEmp = employees.FirstOrDefault(e => e.ID == x.DesignByID);
                    var checkedEmp = employees.FirstOrDefault(e => e.ID == x.CheckedByID);
                    var approvedEmp = employees.FirstOrDefault(e => e.ID == x.ApprovedByID);
                    var projectType = projectTypes.FirstOrDefault(p => p.ID == x.ProjectTypeID);
                    return new
                    {
                        x.ID,
                        x.DrawingName,
                        x.Version,
                        x.ServerPath,
                        x.ProjectID,

                        x.ProjectTypeID,
                        ProjectTypeCode = projectType?.ProjectTypeCode ?? "",
                        ProjectTypeName = projectType?.ProjectTypeName ?? "",

                        x.DesignByID,
                        DesignByName = designEmp?.FullName ?? "",
                        x.DesignDate,

                        x.CheckedByID,
                        CheckedByName = checkedEmp?.FullName ?? "",
                        x.CheckedDate,

                        x.ApprovedByID,
                        ApprovedByName = approvedEmp?.FullName ?? "",
                        x.ApprovedDate,

                        IsDesigned = x.DesignByID != null && x.DesignDate != null,
                        IsChecked = x.CheckedByID != null && x.CheckedDate != null,
                        IsApproved = x.ApprovedByID != null && x.ApprovedDate != null,

                        x.CreatedBy,
                        x.CreatedDate,
                        x.UpdatedBy,
                        x.UpdatedDate
                    };
                }).ToList();

                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-by-id")]
        public IActionResult GetById(int id)
        {
            try
            {
                var drawing = _drawingRepo.GetByID(id);

                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                return Ok(ApiResponseFactory.Success(drawing, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Get data

        #region Save / Delete

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] Drawing request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                if (string.IsNullOrWhiteSpace(request.DrawingName))
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên bản vẽ không được để trống"));

                if (string.IsNullOrWhiteSpace(request.Version))
                    return BadRequest(ApiResponseFactory.Fail(null, "Version không được để trống"));

                if (request.ProjectID == null || request.ProjectID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "ProjectID không hợp lệ"));

                if (request.ProjectTypeID == null || request.ProjectTypeID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "ProjectTypeID không hợp lệ"));

                var duplicate = _drawingRepo.GetAll(x =>
                    x.ID != request.ID &&
                    x.ProjectID == request.ProjectID &&
                    x.DrawingName == request.DrawingName &&
                    x.Version == request.Version &&
                    x.IsDeleted != true)
                    .FirstOrDefault();

                if (duplicate != null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản vẽ và version này đã tồn tại trong dự án"));

                if (request.ID > 0)
                {
                    var old = _drawingRepo.GetByID(request.ID);

                    if (old == null || old.IsDeleted == true)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ cần sửa"));

                    var oldDrawing = new Drawing
                    {
                        ID = old.ID,
                        DrawingName = old.DrawingName,
                        Version = old.Version,
                        ServerPath = old.ServerPath,
                        ProjectID = old.ProjectID,
                        ProjectTypeID = old.ProjectTypeID,

                        //DesignByID = old.DesignByID,
                        //DesignDate = old.DesignDate,

                        //CheckedByID = old.CheckedByID,
                        //CheckedDate = old.CheckedDate,

                        //ApprovedByID = old.ApprovedByID,
                        //ApprovedDate = old.ApprovedDate,

                        CreatedBy = old.CreatedBy,
                        CreatedDate = old.CreatedDate,
                        UpdatedBy = old.UpdatedBy,
                        UpdatedDate = old.UpdatedDate,
                        IsDeleted = old.IsDeleted
                    };

                    if (old == null || old.IsDeleted == true)
                        return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ cần sửa"));

                    if (old.CheckedDate != null && old.DesignByID != request.DesignByID)
                        return BadRequest(ApiResponseFactory.Fail(null, "Bản vẽ đã được check, không được sửa người thiết kế"));

                    old.DrawingName = request.DrawingName;
                    old.Version = request.Version;
                    old.ProjectID = request.ProjectID;
                    old.ProjectTypeID = request.ProjectTypeID;
                    old.DesignByID = request.DesignByID;
                    old.DesignDate = request.DesignDate;

                    if (request.DesignByID != null && old.DesignDate == null)
                    {
                        old.DesignDate = DateTime.Now;
                    }

                    if (request.DesignByID == null)
                        old.DesignDate = null;

                    await _drawingRepo.UpdateAsync(old);

                    var contentLog = _drawingLogRepo.GenerateLog(oldDrawing, old);

                    if (!string.IsNullOrWhiteSpace(contentLog))
                    {
                        await _drawingLogRepo.AddLog(
                            old.ID,
                            contentLog,
                            "Cập nhật bản vẽ"
                        );
                    }

                    return Ok(ApiResponseFactory.Success(old, "Cập nhật bản vẽ thành công"));
                }
                else
                {
                    request.IsDeleted = false;

                    if (request.DesignByID != null && request.DesignDate == null)
                        request.DesignDate = DateTime.Now;

                    await _drawingRepo.CreateAsync(request);

                    await _drawingLogRepo.AddLog(
                        request.ID,
                        "Tạo mới bản vẽ.",
                        "Thêm mới bản vẽ"
                    );

                    return Ok(ApiResponseFactory.Success(request, "Thêm bản vẽ thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var drawing = _drawingRepo.GetByID(id);

                if (drawing == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                drawing.IsDeleted = true;
                drawing.UpdatedDate = DateTime.Now;

                await _drawingRepo.UpdateAsync(drawing);

                await _drawingLogRepo.AddLog(
                    drawing.ID,
                    "Xóa bản vẽ.",
                    "Xóa bản vẽ"
                );

                return Ok(ApiResponseFactory.Success(null, "Xóa bản vẽ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Save / Delete

        #region Upload PDF

        [HttpPost("upload-pdf")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPdf(int drawingID)
        {
            try
            {
                var form = await Request.ReadFormAsync();

                var key = form["key"].ToString();
                var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                var files = form.Files;

                if (drawingID <= 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "DrawingID không hợp lệ"));

                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key không được để trống"));

                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "File không được để trống"));

                var drawing = _drawingRepo.GetByID(drawingID);

                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);

                if (string.IsNullOrWhiteSpace(uploadPath))
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));

                string targetFolder = uploadPath;

                if (!string.IsNullOrWhiteSpace(subPathRaw))
                {
                    var separator = Path.DirectorySeparatorChar;

                    var segments = subPathRaw
                        .Replace('/', separator)
                        .Replace('\\', separator)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                        .Select(seg =>
                        {
                            var invalidChars = Path.GetInvalidFileNameChars();
                            var cleaned = new string(seg.Where(c => !invalidChars.Contains(c)).ToArray());
                            cleaned = cleaned.Replace("..", "").Trim();
                            return cleaned;
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                }
                else
                {
                    targetFolder = Path.Combine(uploadPath, "Drawing", drawing.ProjectID.ToString(), drawing.ID.ToString());
                }

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                var uploadedFiles = new List<object>();

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;

                    var extension = Path.GetExtension(file.FileName);

                    if (!extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                        return BadRequest(ApiResponseFactory.Fail(null, "Chỉ được upload file PDF"));

                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var safeFileName = string.Join("_", originalFileName.Split(Path.GetInvalidFileNameChars()));
                    var uniqueFileName = $"{safeFileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";

                    var fullPath = Path.Combine(targetFolder, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    drawing.ServerPath = fullPath;
                    drawing.UpdatedDate = DateTime.Now;

                    await _drawingRepo.UpdateAsync(drawing);

                    await _drawingLogRepo.AddLog(
                        drawing.ID,
                        $"Upload file PDF: {uniqueFileName}.",
                        "Upload file PDF"
                    );

                    uploadedFiles.Add(new
                    {
                        FileName = uniqueFileName,
                        ServerPath = fullPath,
                        DrawingID = drawing.ID
                    });
                }

                return Ok(ApiResponseFactory.Success(uploadedFiles, "Upload file PDF thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }

        #endregion Upload PDF

        #region Sign workflow

        [HttpPost("check")]
        public async Task<IActionResult> Check(int id, int employeeID)
        {
            try
            {
                var drawing = _drawingRepo.GetByID(id);

                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                if (drawing.DesignByID == null || drawing.DesignDate == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản vẽ chưa có người thiết kế"));

                var oldDrawing = new Drawing
                {
                    ID = drawing.ID,
                    DrawingName = drawing.DrawingName,
                    Version = drawing.Version,
                    ServerPath = drawing.ServerPath,
                    ProjectID = drawing.ProjectID,
                    ProjectTypeID = drawing.ProjectTypeID,
                    DesignByID = drawing.DesignByID,
                    DesignDate = drawing.DesignDate,
                    CheckedByID = drawing.CheckedByID,
                    CheckedDate = drawing.CheckedDate,
                    ApprovedByID = drawing.ApprovedByID,
                    ApprovedDate = drawing.ApprovedDate,
                    IsDeleted = drawing.IsDeleted
                };

                drawing.CheckedByID = employeeID;
                drawing.CheckedDate = DateTime.Now;
                //drawing.UpdatedDate = DateTime.Now;

                await _drawingRepo.UpdateAsync(drawing);

                var contentLog = _drawingLogRepo.GenerateLog(oldDrawing, drawing);

                if (!string.IsNullOrWhiteSpace(contentLog))
                {
                    await _drawingLogRepo.AddLog(
                        drawing.ID,
                        contentLog,
                        "Check bản vẽ"
                    );
                }

                return Ok(ApiResponseFactory.Success(null, "Check bản vẽ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("approve")]
        public async Task<IActionResult> Approve(int id, int employeeID)
        {
            try
            {
                var drawing = _drawingRepo.GetByID(id);

                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                if (drawing.CheckedByID == null || drawing.CheckedDate == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Bản vẽ chưa được check"));

                var oldDrawing = new Drawing
                {
                    ID = drawing.ID,
                    DrawingName = drawing.DrawingName,
                    Version = drawing.Version,
                    ServerPath = drawing.ServerPath,
                    ProjectID = drawing.ProjectID,
                    ProjectTypeID = drawing.ProjectTypeID,
                    DesignByID = drawing.DesignByID,
                    DesignDate = drawing.DesignDate,
                    CheckedByID = drawing.CheckedByID,
                    CheckedDate = drawing.CheckedDate,
                    ApprovedByID = drawing.ApprovedByID,
                    ApprovedDate = drawing.ApprovedDate,
                    IsDeleted = drawing.IsDeleted
                };

                drawing.ApprovedByID = employeeID;
                drawing.ApprovedDate = DateTime.Now;
                //drawing.UpdatedDate = DateTime.Now;

                await _drawingRepo.UpdateAsync(drawing);

                var contentLog = _drawingLogRepo.GenerateLog(oldDrawing, drawing);

                if (!string.IsNullOrWhiteSpace(contentLog))
                {
                    await _drawingLogRepo.AddLog(
                        drawing.ID,
                        contentLog,
                        "Approve bản vẽ"
                    );
                }

                return Ok(ApiResponseFactory.Success(null, "Approve bản vẽ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Sign workflow

        #region Get Signatures

        [HttpGet("get-signatures")]
        public IActionResult GetSignatures(int drawingID)
        {
            try
            {
                var drawing = _drawingRepo.GetByID(drawingID);

                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));

                var signatureBasePath = _configSystemRepo.GetUploadPathByKey("SIGNATURE_PATH");

                if (string.IsNullOrWhiteSpace(signatureBasePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Key ConfigSystem SIGNATURE_PATH không tồn tại"));

                var employeeIds = new List<int>();

                if (drawing.DesignByID.HasValue)
                    employeeIds.Add(drawing.DesignByID.Value);

                if (drawing.CheckedByID.HasValue)
                    employeeIds.Add(drawing.CheckedByID.Value);

                if (drawing.ApprovedByID.HasValue)
                    employeeIds.Add(drawing.ApprovedByID.Value);

                var employees = _employeeRepo
                    .GetAll(x => employeeIds.Contains(x.ID))
                    .ToList();

                object GetSignature(int? employeeID, DateTime? signedDate, string step)
                {
                    if (employeeID == null)
                    {
                        return new
                        {
                            Step = step,
                            EmployeeID = (int?)null,
                            EmployeeCode = "",
                            EmployeeName = "",
                            SignedDate = (DateTime?)null,
                            SignatureFileName = "",
                            SignaturePath = "",
                            IsSigned = false
                        };
                    }

                    var emp = employees.FirstOrDefault(x => x.ID == employeeID.Value);

                    if (emp == null)
                    {
                        return new
                        {
                            Step = step,
                            EmployeeID = employeeID,
                            EmployeeCode = "",
                            EmployeeName = "",
                            SignedDate = signedDate,
                            SignatureFileName = "",
                            SignaturePath = "",
                            IsSigned = false
                        };
                    }

                    var signatureFileName = $"{emp.Code}.jpg";
                    var signaturePath = Path.Combine(signatureBasePath, signatureFileName);

                    return new
                    {
                        Step = step,
                        EmployeeID = emp.ID,
                        EmployeeCode = emp.Code,
                        EmployeeName = emp.FullName,
                        SignedDate = signedDate,
                        SignatureFileName = signatureFileName,
                        SignaturePath = signaturePath,
                        IsSigned = signedDate != null,
                        FileExists = System.IO.File.Exists(signaturePath)
                    };
                }

                var data = new
                {
                    DrawingID = drawing.ID,
                    drawing.DrawingName,
                    drawing.Version,
                    drawing.ServerPath,

                    Signatures = new[]
                    {
                        GetSignature(drawing.DesignByID, drawing.DesignDate, "DESIGN"),
                        GetSignature(drawing.CheckedByID, drawing.CheckedDate, "CHECK"),
                        GetSignature(drawing.ApprovedByID, drawing.ApprovedDate, "APPROVE")
                    }
                };

                return Ok(ApiResponseFactory.Success(data, "Lấy chữ ký bản vẽ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Get Signatures

        #region Save PDF

        [HttpGet("view-signed-pdf")]
        public IActionResult ViewSignedPdf(int drawingID)
        {
            try
            {
                var drawing = _drawingRepo.GetByID(drawingID);
                if (drawing == null || drawing.IsDeleted == true)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản vẽ"));
                if (!System.IO.File.Exists(drawing.ServerPath))
                    return BadRequest(ApiResponseFactory.Fail(null, "File PDF không tồn tại trên server"));
                var signatureBasePath = _configSystemRepo.GetUploadPathByKey("SIGNATURE_PATH");
                var employeeIds = new List<int>();
                if (drawing.DesignByID.HasValue) employeeIds.Add(drawing.DesignByID.Value);
                if (drawing.CheckedByID.HasValue) employeeIds.Add(drawing.CheckedByID.Value);
                if (drawing.ApprovedByID.HasValue) employeeIds.Add(drawing.ApprovedByID.Value);
                var employees = _employeeRepo.GetAll(x => employeeIds.Contains(x.ID)).ToList();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(drawing.ServerPath))
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        void DrawSignature(int? empID, string step)
                        {
                            if (empID == null) return;
                            var emp = employees.FirstOrDefault(x => x.ID == empID.Value);
                            if (emp == null) return;
                            string sigPath = Path.Combine(signatureBasePath, $"{emp.Code}.jpg");
                            if (!System.IO.File.Exists(sigPath)) return;
                            // Toạ độ tính từ top left
                            float x = 0, y = 0, width = 65, height = 30;
                            if (step == "DESIGN") { x = 266; y = 787; }
                            else if (step == "CHECK") { x = 353; y = 787; }
                            else if (step == "APPROVE") { x = 430; y = 787; }
                            Image img = Image.GetInstance(sigPath);
                            img.ScaleAbsolute(width, height);
                            int numberOfPages = reader.NumberOfPages;
                            for (int i = 1; i <= numberOfPages; i++)
                            {
                                // Lật tọa độ Y do itextsharp lấy gốc từ bottom left
                                Rectangle pageSize = reader.GetPageSize(i);
                                float pdfY = pageSize.Height - (y + height);
                                img.SetAbsolutePosition(x, pdfY);

                                // Lấy layer trên cùng của trang i và dán ảnh vào
                                PdfContentByte canvas = stamper.GetOverContent(i);
                                canvas.AddImage(img);
                            }
                        }
                        DrawSignature(drawing.DesignByID, "DESIGN");
                        DrawSignature(drawing.CheckedByID, "CHECK");
                        DrawSignature(drawing.ApprovedByID, "APPROVE");
                    }

                    return File(ms.ToArray(), "application/pdf");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi tạo PDF: " + ex.Message));
            }
        }

        #endregion Save PDF

        #region Get Log

        [HttpGet("get-log")]
        public IActionResult GetLog(int drawingID)
        {
            try
            {
                var logs = _drawingLogRepo
                    .GetAll(x => x.DrawingID == drawingID && x.IsDeleted != true)
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new
                    {
                        x.ID,
                        x.DrawingID,
                        x.TypeLog,
                        x.ContentLog,
                        x.CreatedBy,
                        x.CreatedDate
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(logs, "Lấy log bản vẽ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion Get Log
    }
}