using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Project
{
    public class SaveStepFormsDto
    {
        public List<ProjectGateStepForm> Items { get; set; } = new List<ProjectGateStepForm>();
        public List<int> DeletedIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// API quản lý biểu mẫu đính kèm (ProjectGateStepForm) cho từng công đoạn mẫu
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepFormController : ControllerBase
    {
        private readonly ProjectGateStepFormRepo _formRepo;
        private readonly ConfigSystemRepo _configSystemRepo;
        private readonly CurrentUser _currentUser;

        public ProjectGateStepFormController(
            ProjectGateStepFormRepo formRepo,
            ConfigSystemRepo configSystemRepo,
            CurrentUser currentUser)
        {
            _formRepo = formRepo;
            _configSystemRepo = configSystemRepo;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Lấy tất cả danh sách biểu mẫu đính kèm theo ID công đoạn mẫu (ProjectGateStepID)
        /// </summary>
        [HttpGet("get-by-step/{stepId}")]
        public IActionResult GetByStep(int stepId)
        {
            try
            {
                var data = _formRepo.GetAll(x => x.ProjectGateStepID == stepId && (x.IsDeleted == false || x.IsDeleted == null))
                                    .OrderBy(x => x.STT ?? int.MaxValue)
                                    .ThenBy(x => x.ID)
                                    .ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu biểu mẫu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu (Thêm mới, Cập nhật, Xóa) danh sách biểu mẫu theo công đoạn mẫu
        /// </summary>
        [HttpPost("save-by-step/{stepId}")]
        public async Task<IActionResult> SaveByStep(int stepId, [FromBody] SaveStepFormsDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                // 1. Xóa các mục bị xóa
                if (dto.DeletedIds != null && dto.DeletedIds.Count > 0)
                {
                    foreach (var id in dto.DeletedIds)
                    {
                        var entity = _formRepo.GetByID(id);
                        if (entity != null)
                        {
                            entity.IsDeleted = true;
                            entity.UpdatedDate = DateTime.Now;
                            entity.UpdatedBy = _currentUser.LoginName ?? User.Identity?.Name;
                            await _formRepo.UpdateAsync(entity);
                        }
                    }
                }

                // 2. Thêm mới / Cập nhật
                if (dto.Items != null && dto.Items.Count > 0)
                {
                    foreach (var item in dto.Items)
                    {
                        item.ProjectGateStepID = stepId;

                        if (item.ID <= 0)
                        {
                            item.IsDeleted = false;
                            item.CreatedDate = DateTime.Now;
                            item.CreatedBy = _currentUser.LoginName ?? User.Identity?.Name;
                            await _formRepo.CreateAsync(item);
                        }
                        else
                        {
                            var existing = _formRepo.GetByID(item.ID);
                            if (existing != null)
                            {
                                existing.STT = item.STT;
                                existing.FormName = item.FormName;
                                existing.FileName = item.FileName;
                                existing.FilePath = item.FilePath;
                                existing.FileSize = item.FileSize;
                                existing.Description = item.Description;
                                existing.UpdatedDate = DateTime.Now;
                                existing.UpdatedBy = _currentUser.LoginName ?? User.Identity?.Name;

                                await _formRepo.UpdateAsync(existing);
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu danh sách biểu mẫu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Upload file đính kèm cho biểu mẫu công đoạn
        /// </summary>
        [HttpPost("upload-file")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var files = form.Files;

                if (files == null || files.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn tệp tin để tải lên!"));
                }

                var file = files[0];
                if (file.Length == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Tệp tin không được để trống!"));
                }

                string departmentName = form.ContainsKey("departmentName") ? form["departmentName"].ToString() : string.Empty;

                var uploadPath = _configSystemRepo.GetUploadPathByKey("Projects");
                if (string.IsNullOrWhiteSpace(uploadPath))
                {
                    uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Forms");
                }

                string targetFolder = string.IsNullOrWhiteSpace(departmentName)
                    ? Path.Combine(uploadPath, "Forms")
                    : Path.Combine(uploadPath, "Forms", departmentName.Trim());
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var fileExtension = Path.GetExtension(file.FileName);
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uniqueFileName = $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                var fullPath = Path.Combine(targetFolder, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var result = new
                {
                    OriginalFileName = file.FileName,
                    SavedFileName = uniqueFileName,
                    FilePath = fullPath,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                };

                return Ok(ApiResponseFactory.Success(result, "Tải file đính kèm thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi upload file: {ex.Message}"));
            }
        }
    }
}
