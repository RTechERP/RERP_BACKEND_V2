using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepCheckListDetailLinkController : ControllerBase
    {
        private readonly ProjectGateStepCheckListDetailLinkRepo _stepCheckListDetailLinkRepo;
        private readonly ProjectGateStepCheckListDetailRepo _stepCheckListDetailRepo;
        private readonly ProjectGateStepFileRepo _stepFileRepo;
        private readonly ProjectGateStepLinkRepo _stepLinkRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly CurrentUser _currentUser;

        public ProjectGateStepCheckListDetailLinkController(
            ProjectGateStepCheckListDetailLinkRepo stepCheckListDetailLinkRepo,
            ProjectGateStepCheckListDetailRepo stepCheckListDetailRepo,
            ProjectGateStepFileRepo stepFileRepo,
            ProjectGateStepLinkRepo stepLinkRepo,
            ProjectRepo projectRepo,
            ConfigSystemRepo configSystemRepo,
            CurrentUser currentUser)
        {
            _stepCheckListDetailLinkRepo = stepCheckListDetailLinkRepo;
            _stepCheckListDetailRepo = stepCheckListDetailRepo;
            _stepFileRepo = stepFileRepo;
            _stepLinkRepo = stepLinkRepo;
            _projectRepo = projectRepo;
            _configSystemRepo = configSystemRepo;
            _currentUser = currentUser;
        }

        [HttpPost("SaveFile/{checkListLinkId}")]
        public async Task<IActionResult> SaveFile(int checkListLinkId, [FromBody] ProjectGateStepFileDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.FileName) || string.IsNullOrWhiteSpace(dto.FilePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Thông tin file không hợp lệ"));

                var checkListLink = _stepCheckListDetailLinkRepo.GetByID(checkListLinkId);
                if (checkListLink == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy Quy tắc liên kết"));

                var ruleDef = _stepCheckListDetailRepo.GetByID(checkListLink.ProjectGateStepCheckListDetailID);
                string? projectCode = null;
                string? projectName = null;

                var stepLink = _stepLinkRepo.GetByID(checkListLink.ProjectGateStepLinkID);
                if (stepLink != null && stepLink.ProjectID.HasValue && stepLink.ProjectID.Value > 0)
                {
                    var proj = _projectRepo.GetByID(stepLink.ProjectID.Value);
                    if (proj != null)
                    {
                        projectCode = proj.ProjectCode;
                        projectName = proj.ProjectName;
                    }
                }

                if (ruleDef != null)
                {
                    // 1. Kiểm tra định dạng (FileFormat / Type)
                    if (ruleDef.IsFile && !string.IsNullOrWhiteSpace(ruleDef.FileFormat))
                    {
                        var ext = Path.GetExtension(dto.FileName)?.TrimStart('.').ToLower();
                        var allowedFormats = ruleDef.FileFormat.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(f => f.Trim().TrimStart('*').TrimStart('.').ToLower())
                                               .ToList();

                        if (allowedFormats.Any() && !allowedFormats.Contains("*") && !allowedFormats.Contains("tất cả") && !allowedFormats.Contains(ext))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Tệp tin không đúng định dạng yêu cầu. Định dạng được phép: {ruleDef.FileFormat}"));
                        }
                    }

                    // 2. Kiểm tra tên quy chuẩn (FileName / StandardFileName)
                    if (ruleDef.IsFile && !string.IsNullOrWhiteSpace(ruleDef.FileName))
                    {
                        if (!IsFileNameMatchStandard(dto.FileName, ruleDef.FileName, projectCode, projectName))
                        {
                            var resolvedName = GetResolvedStandardFileName(ruleDef.FileName, projectCode, projectName, dto.FileName);
                            return BadRequest(ApiResponseFactory.Fail(null, $"Tên tệp tin không đúng quy chuẩn. Quy chuẩn yêu cầu: \"{resolvedName}\" (Mẫu: {ruleDef.FileName})"));
                        }
                    }

                    // 3. Kiểm tra giới hạn số lượng (FileQuantity)
                    if (ruleDef.IsFile && ruleDef.FileQuantity > 0)
                    {
                        var currentCount = _stepFileRepo.GetAll(f => f.ProjectGateStepCheckListDetailLinkID == checkListLinkId && (f.IsDeleted == false || f.IsDeleted == null)).Count();
                        if (currentCount >= ruleDef.FileQuantity)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Số lượng tệp đính kèm vượt quá giới hạn cho phép. Tối đa: {ruleDef.FileQuantity} file."));
                        }
                    }
                }

                // Đảm bảo FileName lưu vào DB luôn là OriginalFileName sạch (không chứa hậu tố unique do server sinh ra nếu lỡ bị truyền vào)
                string cleanFileName = dto.FileName;
                var fileExt = Path.GetExtension(cleanFileName);
                var baseName = Path.GetFileNameWithoutExtension(cleanFileName);
                baseName = Regex.Replace(baseName, @"(_[0-9]{14}_[a-fA-F0-9]{8})$", "");
                if (!string.IsNullOrWhiteSpace(projectCode))
                {
                    baseName = Regex.Replace(baseName, @"(_" + Regex.Escape(projectCode.Trim()) + @"_[0-9]{14}_[a-fA-F0-9]{8})$", "");
                }
                cleanFileName = $"{baseName}{fileExt}";

                var newFile = new ProjectGateStepFile
                {
                    ProjectGateStepCheckListDetailLinkID = checkListLinkId,
                    FileName = cleanFileName,
                    FilePath = dto.FilePath,
                    FileSize = dto.FileSize,
                    ContentType = dto.ContentType,
                    Status = 1,
                    IsDeleted = false,
                    CreatedBy = User.Identity?.Name ?? "System",
                    CreatedDate = DateTime.Now,
                    EmployeeID = _currentUser.EmployeeID > 0 ? _currentUser.EmployeeID : (int?)null
                };

                await _stepFileRepo.CreateAsync(newFile);
                return Ok(ApiResponseFactory.Success(newFile.ID, "Lưu thông tin file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Chuyển chuỗi tiếng Việt có dấu thành không dấu, viết hoa chữ cái đầu mỗi từ (PascalCase) và loại bỏ ký tự đặc biệt, dấu câu
        /// Ví dụ: "Máy đóng gói, tự động - 2026" -> "MayDongGoiTuDong2026"
        /// </summary>
        private static string SanitizeProjectName(string? str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            // 1. Chuyển tiếng Việt có dấu thành không dấu
            string normalized = str.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            string nonAccent = sb.ToString().Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd')
                .Replace('Đ', 'D');

            // 2. Tách từ theo khoảng trắng và các ký tự đặc biệt / dấu câu
            var words = Regex.Split(nonAccent, @"[^a-zA-Z0-9]+");
            var result = new StringBuilder();
            foreach (var w in words)
            {
                if (string.IsNullOrEmpty(w)) continue;
                if (w.Length == 1)
                {
                    result.Append(char.ToUpperInvariant(w[0]));
                }
                else
                {
                    result.Append(char.ToUpperInvariant(w[0])).Append(w.Substring(1));
                }
            }

            string res = result.ToString();
            return !string.IsNullOrEmpty(res) ? res : Regex.Replace(nonAccent, @"[^a-zA-Z0-9]", "");
        }

        /// <summary>
        /// Sinh chuỗi tên file quy chuẩn mẫu với các biến động được thay thế bằng thông tin thực tế của dự án để người dùng dễ quan sát và copy
        /// </summary>
        private static string GetResolvedStandardFileName(string templateFileName, string? projectCode, string? projectName, string? uploadFileName = null)
        {
            if (string.IsNullOrWhiteSpace(templateFileName)) return string.Empty;

            string safeProjCode = !string.IsNullOrWhiteSpace(projectCode) ? projectCode.Trim() : "MãDựÁn";
            string sanitizedProjName = SanitizeProjectName(projectName);
            string cleanProjName = !string.IsNullOrWhiteSpace(sanitizedProjName) ? sanitizedProjName : "TenDuAn";

            string resolved = templateFileName.Trim();
            resolved = Regex.Replace(resolved, @"\{(?i)(projectcode|maduan)\}", safeProjCode);
            resolved = Regex.Replace(resolved, @"\{(?i)(projectname|tenduan)\}", cleanProjName);
            resolved = Regex.Replace(resolved, @"\{(?i)(rv|revision|ver|version)\}", "Rv01");
            resolved = Regex.Replace(resolved, @"\{(?i)xx\}", "01");
            resolved = Regex.Replace(resolved, @"\{(?i)(gatecode|magate)\}", "GateCode");
            resolved = Regex.Replace(resolved, @"\{(?i)(stepcode|macongdoan)\}", "StepCode");
            resolved = Regex.Replace(resolved, @"\{(?i)(any|text|all)\}", "TenFile");
            resolved = resolved.Replace("*", "");
            resolved = Regex.Replace(resolved, @"([-_])(?i)(rv|revision|ver|version)$", "$1RV01");

            if (!string.IsNullOrWhiteSpace(uploadFileName) && !resolved.Contains('.'))
            {
                string ext = Path.GetExtension(uploadFileName);
                if (!string.IsNullOrWhiteSpace(ext))
                {
                    resolved = $"{resolved}{ext}";
                }
            }

            return resolved;
        }

        /// <summary>
        /// Lấy phần tên cơ bản của file (loại bỏ đuôi mở rộng như .jpg, .png, .pdf nếu có, nhưng không cắt nhầm dấu chấm trong mã dự án như 1.25.023)
        /// </summary>
        private static string GetFileNameBase(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;
            fileName = fileName.Trim();

            // Chỉ cắt phần mở rộng nếu sau dấu chấm cuối cùng là extension hợp lệ (1-6 ký tự chữ/số không chứa gạch ngang/dưới và không phải số nguyên)
            int lastDot = fileName.LastIndexOf('.');
            if (lastDot > 0 && lastDot < fileName.Length - 1)
            {
                string potentialExt = fileName.Substring(lastDot + 1);
                if (potentialExt.Length <= 6 && Regex.IsMatch(potentialExt, @"^[a-zA-Z0-9]+$") && !int.TryParse(potentialExt, out _))
                {
                    return fileName.Substring(0, lastDot).Trim();
                }
            }

            return fileName;
        }

        /// <summary>
        /// Kiểm tra tên file upload có khớp với mẫu quy chuẩn (hỗ trợ cả chuỗi tĩnh lẫn template regex: {ProjectCode}, {ProjectName}, {Rv}, {StepCode}, *)
        /// </summary>
        private static bool IsFileNameMatchStandard(string uploadFileName, string templateFileName, string? projectCode, string? projectName)
        {
            if (string.IsNullOrWhiteSpace(templateFileName)) return true;

            var uploadBase = GetFileNameBase(uploadFileName);
            var templateBase = GetFileNameBase(templateFileName);

            // Bóc tách hậu tố unique do server upload sinh ra (_yyyyMMddHHmmss_guid) nếu có
            uploadBase = Regex.Replace(uploadBase, @"(_[0-9]{14}_[a-fA-F0-9]{8})$", "");
            if (!string.IsNullOrWhiteSpace(projectCode))
            {
                uploadBase = Regex.Replace(uploadBase, @"(_" + Regex.Escape(projectCode.Trim()) + @"_[0-9]{14}_[a-fA-F0-9]{8})$", "");
            }

            if (!Regex.IsMatch(templateBase, @"\{.*?\}|\*"))
            {
                return uploadBase.IndexOf(templateBase, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            // Chuẩn hóa ProjectCode & ProjectName
            string safeProjectCode = !string.IsNullOrWhiteSpace(projectCode) ? Regex.Escape(projectCode.Trim()) : @"[a-zA-Z0-9_\-\.]+";
            string sanitizedProjName = SanitizeProjectName(projectName);
            string rawCleanProjName = !string.IsNullOrWhiteSpace(projectName) ? Regex.Replace(projectName.Trim(), @"\s+", "") : "";

            string projNamePattern;
            if (!string.IsNullOrWhiteSpace(sanitizedProjName) && !string.IsNullOrWhiteSpace(rawCleanProjName) && !sanitizedProjName.Equals(rawCleanProjName, StringComparison.OrdinalIgnoreCase))
            {
                projNamePattern = $"(?:{Regex.Escape(sanitizedProjName)}|{Regex.Escape(rawCleanProjName)})";
            }
            else if (!string.IsNullOrWhiteSpace(sanitizedProjName))
            {
                projNamePattern = Regex.Escape(sanitizedProjName);
            }
            else
            {
                projNamePattern = @"[a-zA-Z0-9_\-\.]+";
            }

            // Chuẩn hóa hậu tố -RV ở cuối template thành {Rv} nếu người dùng cấu hình không có ngoặc nhọn
            string pattern = templateBase;
            pattern = Regex.Replace(pattern, @"([-_])(?i)(rv|revision|ver|version)$", "$1{Rv}");

            // Tách theo các token placeholder {...} và wildcard * để ráp biểu thức Regex hoàn chỉnh
            var parts = Regex.Split(pattern, @"(\{[^}]+\}|\*)");
            var patternBuilder = new StringBuilder();
            patternBuilder.Append('^');

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                if (part == "*")
                {
                    patternBuilder.Append(".*");
                }
                else if (part.StartsWith("{") && part.EndsWith("}"))
                {
                    string token = part.Substring(1, part.Length - 2).Trim();
                    if (Regex.IsMatch(token, @"^(?i)(projectcode|maduan)$"))
                    {
                        patternBuilder.Append(safeProjectCode);
                    }
                    else if (Regex.IsMatch(token, @"^(?i)(projectname|tenduan)$"))
                    {
                        patternBuilder.Append(projNamePattern);
                    }
                    else if (Regex.IsMatch(token, @"^(?i)(rv|revision|xx|ver|version)$"))
                    {
                        patternBuilder.Append(@"(?:Rv|rv|RV|v|V)?\d*");
                    }
                    else if (Regex.IsMatch(token, @"^(?i)(gatecode|magate)$"))
                    {
                        patternBuilder.Append(@"[a-zA-Z0-9_\-]+");
                    }
                    else if (Regex.IsMatch(token, @"^(?i)(stepcode|macongdoan)$"))
                    {
                        patternBuilder.Append(@"[a-zA-Z0-9_\-]+");
                    }
                    else if (Regex.IsMatch(token, @"^(?i)(any|text|all)$"))
                    {
                        patternBuilder.Append(@".*");
                    }
                    else
                    {
                        patternBuilder.Append(@"[a-zA-Z0-9_\-\.]+");
                    }
                }
                else
                {
                    // Phần tĩnh: chỉ escape regex, giữ nguyên dấu - và _ (không cần thay thế linh hoạt vì pattern đã IgnoreCase)
                    string escapedStatic = Regex.Escape(part);
                    patternBuilder.Append(escapedStatic);
                }
            }

            patternBuilder.Append('$');

            var finalRegex = new Regex(patternBuilder.ToString(), RegexOptions.IgnoreCase);
            return finalRegex.IsMatch(uploadBase);
        }

        [HttpGet("GetCheckLists/{stepLinkId}")]
        public async Task<IActionResult> GetCheckLists(int stepLinkId)
        {
            try
            {
                var (checklists, files) = await SqlDapper<ProjectGateStepCheckListLinkDto>.QueryMultipleAsync<ProjectGateStepCheckListLinkDto, ProjectGateStepFileDto>(
                    "spGetProjectGateStepCheckLists",
                    new { StepLinkID = stepLinkId }
                );

                var filesDict = files.GroupBy(f => f.ProjectGateStepCheckListDetailLinkID)
                                     .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var cl in checklists)
                {
                    if (filesDict.TryGetValue(cl.ID, out var fList))
                    {
                        cl.Files = fList;
                    }
                    else
                    {
                        cl.Files = new List<ProjectGateStepFileDto>();
                    }
                }

                return Ok(ApiResponseFactory.Success(checklists, "Lấy danh sách checklist thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("GetFilesByStep/{stepLinkId}")]
        public async Task<IActionResult> GetFilesByStep(int stepLinkId)
        {
            try
            {
                var files = await SqlDapper<ProjectGateStepFileDto>.ProcedureToListTAsync(
                    "spGetProjectGateStepFilesByStep",
                    new { StepLinkID = stepLinkId }
                );

                return Ok(ApiResponseFactory.Success(files, "Lấy danh sách file của công đoạn thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("GetFiles/{checkListLinkId}")]
        public async Task<IActionResult> GetFiles(int checkListLinkId)
        {
            try
            {
                var files = await SqlDapper<ProjectGateStepFileDto>.ProcedureToListTAsync(
                    "spGetProjectGateStepFiles",
                    new { CheckListLinkId = checkListLinkId }
                );

                return Ok(ApiResponseFactory.Success(files, "Lấy danh sách file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("DeleteFile/{fileId}")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            try
            {
                var file = _stepFileRepo.GetByID(fileId);
                if (file == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy file"));

                var currentUser = User.Identity?.Name;
                bool isOwner = false;
                if (file.EmployeeID.HasValue && _currentUser.EmployeeID > 0)
                {
                    isOwner = file.EmployeeID.Value == _currentUser.EmployeeID;
                }
                else if (!string.IsNullOrEmpty(file.CreatedBy) && !string.IsNullOrEmpty(currentUser))
                {
                    isOwner = string.Equals(file.CreatedBy, currentUser, StringComparison.OrdinalIgnoreCase);
                }

                if (!isOwner)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể xóa file của nhân viên khác"));
                }

                file.IsDeleted = true;
                file.UpdatedBy = currentUser ?? "System";
                file.UpdatedDate = DateTime.Now;
                await _stepFileRepo.UpdateAsync(file);

                return Ok(ApiResponseFactory.Success(true, "Xóa file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("CompleteRules")]
        public async Task<IActionResult> CompleteRules([FromBody] CompleteRuleDto dto)
        {
            try
            {
                if (dto == null || dto.DetailLinkIDs == null || !dto.DetailLinkIDs.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                var links = _stepCheckListDetailLinkRepo.GetAll(c => dto.DetailLinkIDs.Contains(c.ID)).ToList();
                if (!links.Any())
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy Quy tắc liên kết nào"));

                foreach (var link in links)
                {
                    link.IsCompleted = dto.IsCompleted;
                    await _stepCheckListDetailLinkRepo.UpdateAsync(link);
                }

                return Ok(ApiResponseFactory.Success(true, "Cập nhật trạng thái hoàn thành thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("CheckRequiredFiles")]
        public IActionResult CheckRequiredFiles([FromBody] List<int> detailLinkIds)
        {
            try
            {
                if (detailLinkIds == null || !detailLinkIds.Any())
                {
                    return Ok(ApiResponseFactory.Success(new List<FileCheckViolationDto>(), "Kiểm tra file thành công"));
                }

                var links = _stepCheckListDetailLinkRepo.GetAll(c => detailLinkIds.Contains(c.ID) && c.IsDeleted != true).ToList();
                if (!links.Any())
                {
                    return Ok(ApiResponseFactory.Success(new List<FileCheckViolationDto>(), "Kiểm tra file thành công"));
                }

                var detailIds = links.Select(l => l.ProjectGateStepCheckListDetailID).Distinct().ToList();
                var details = _stepCheckListDetailRepo.GetAll(d => detailIds.Contains(d.ID) && d.IsDeleted != true && d.IsFile == true).ToDictionary(d => d.ID);

                var fileCounts = _stepFileRepo.GetAll(f => detailLinkIds.Contains(f.ProjectGateStepCheckListDetailLinkID) && (f.IsDeleted == false || f.IsDeleted == null))
                    .GroupBy(f => f.ProjectGateStepCheckListDetailLinkID)
                    .ToDictionary(g => g.Key, g => g.Count());

                var violations = new List<FileCheckViolationDto>();
                foreach (var link in links)
                {
                    if (details.TryGetValue(link.ProjectGateStepCheckListDetailID, out var detail))
                    {
                        int fileCount = fileCounts.TryGetValue(link.ID, out var count) ? count : 0;
                        if (fileCount < detail.FileQuantity)
                        {
                            violations.Add(new FileCheckViolationDto
                            {
                                DetailLinkID = link.ID,
                                Description = detail.FileRule,
                                FileName = detail.FileName,
                                RequiredQuantity = detail.FileQuantity,
                                UploadedQuantity = fileCount
                            });
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(violations, "Kiểm tra file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("ApproveRule/{detailLinkId}")]
        public async Task<IActionResult> ApproveRule(int detailLinkId, [FromBody] ApproveRuleDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                var link = _stepCheckListDetailLinkRepo.GetByID(detailLinkId);
                if (link == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy Quy tắc liên kết"));

                link.IsApprovedTBP = dto.IsApprovedTBP;
                link.ApprovedTBPBy = dto.ApprovedTBPBy;
                link.ApprovedTBPDate = DateTime.Now;
                link.UpdatedDate = DateTime.Now;
                link.UpdatedBy = _currentUser.LoginName ?? User.Identity?.Name ?? "TBP";
                await _stepCheckListDetailLinkRepo.UpdateAsync(link);

                return Ok(ApiResponseFactory.Success(true, "Phê duyệt quy tắc thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Upload nhiều file đính kèm cho bước quy trình Project Gate với định dạng tên file:
        /// [Tên_file_gốc]_[Mã_dự_án]_[yyyyMMddHHmmss]_[Mã_ngẫu_nhiên].[Đuôi_mở_rộng]
        /// </summary>
        [HttpPost("upload-multiple")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadMultipleFiles()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var key = form["key"].ToString();
                if (string.IsNullOrWhiteSpace(key)) key = "Projects";

                var files = form.Files;
                if (files == null || files.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách file không được để trống!"));
                }

                var uploadPath = _configSystemRepo.GetUploadPathByKey(key);
                if (string.IsNullOrWhiteSpace(uploadPath))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: {key}"));
                }

                var subPathRaw = form["subPath"].ToString()?.Trim() ?? "";
                var projectCode = form["projectCode"].ToString()?.Trim() ?? "";
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
                            return cleaned.Replace("..", "").Trim();
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (segments.Length > 0)
                    {
                        targetFolder = Path.Combine(uploadPath, Path.Combine(segments));
                    }
                }

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var uploadResults = new List<object>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);

                        var projCodePart = (!string.IsNullOrWhiteSpace(projectCode) && !originalFileName.Contains(projectCode, StringComparison.OrdinalIgnoreCase))
                            ? $"_{projectCode}"
                            : "";
                        var uniqueFileName = $"{originalFileName}{projCodePart}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                        var fullPath = Path.Combine(targetFolder, uniqueFileName);

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

        [HttpPost("UpdateFileStatus/{fileId}")]
        public async Task<IActionResult> UpdateFileStatus(int fileId, [FromQuery] int status)
        {
            try
            {
                var file = _stepFileRepo.GetByID(fileId);
                if (file == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy file"));

                file.Status = status;
                file.UpdatedBy = User.Identity?.Name ?? "System";
                file.UpdatedDate = DateTime.Now;
                await _stepFileRepo.UpdateAsync(file);

                return Ok(ApiResponseFactory.Success(true, "Cập nhật trạng thái file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

