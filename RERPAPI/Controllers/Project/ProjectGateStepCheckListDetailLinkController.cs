using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly CurrentUser _currentUser;

        public ProjectGateStepCheckListDetailLinkController(
            ProjectGateStepCheckListDetailLinkRepo stepCheckListDetailLinkRepo,
            ProjectGateStepCheckListDetailRepo stepCheckListDetailRepo,
            ProjectGateStepFileRepo stepFileRepo,
            CurrentUser currentUser)
        {
            _stepCheckListDetailLinkRepo = stepCheckListDetailLinkRepo;
            _stepCheckListDetailRepo = stepCheckListDetailRepo;
            _stepFileRepo = stepFileRepo;
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
                        var standardName = Path.GetFileNameWithoutExtension(ruleDef.FileName).Trim().ToLower();
                        var uploadName = Path.GetFileNameWithoutExtension(dto.FileName).Trim().ToLower();

                        if (!uploadName.Contains(standardName))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Tên tệp tin không đúng quy chuẩn. Tên tệp yêu cầu có chứa: {ruleDef.FileName}"));
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

                var newFile = new ProjectGateStepFile
                {
                    ProjectGateStepCheckListDetailLinkID = checkListLinkId,
                    FileName = dto.FileName,
                    FilePath = dto.FilePath,
                    FileSize = dto.FileSize,
                    ContentType = dto.ContentType,
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
    }
}
