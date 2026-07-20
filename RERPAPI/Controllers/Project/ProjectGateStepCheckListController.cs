using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Project
{
    /// <summary>
    /// API quản lý các quy tắc File chi tiết (CheckListDetail) liên kết trực tiếp với Công đoạn (ProjectGateStep)
    /// Route: api/ProjectGateStepCheckList
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepCheckListController : ControllerBase
    {
        private readonly ProjectGateStepCheckListDetailRepo _detailRepo;
        private readonly ProjectGateStepRepo _stepRepo;

        public ProjectGateStepCheckListController(
            ProjectGateStepCheckListDetailRepo detailRepo,
            ProjectGateStepRepo stepRepo)
        {
            _detailRepo = detailRepo;
            _stepRepo = stepRepo;
        }

        /// <summary>
        /// Lấy danh sách quy tắc chi tiết của 1 Công đoạn mẫu
        /// </summary>
        [HttpGet("get-details-by-checklist/{stepId}")]
        public IActionResult GetDetailsByStep(int stepId)
        {
            try
            {
                var data = _detailRepo
                    .GetAll(x => x.ProjectGateStepID == stepId && x.IsDeleted != true)
                    .OrderBy(x => x.STT)
                    .ThenBy(x => x.ID)
                    .ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu (thêm/sửa/xóa) danh sách quy tắc chi tiết cho Công đoạn mẫu
        /// </summary>
        [HttpPost("save-details/{stepId}")]
        public async Task<IActionResult> SaveDetails(int stepId, [FromBody] SaveCheckListDetailsDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                // Verify step exists
                var step = _stepRepo.GetAll(x => x.ID == stepId).FirstOrDefault();
                if (step == null)
                    return NotFound(ApiResponseFactory.Fail(null, $"Không tìm thấy Công đoạn mẫu ID={stepId}"));

                // 1. Process soft deletes
                if (dto.DeletedIds != null && dto.DeletedIds.Count > 0)
                {
                    var toDelete = _detailRepo.GetAll(x => dto.DeletedIds.Contains(x.ID) && x.ProjectGateStepID == stepId);
                    foreach (var item in toDelete)
                    {
                        item.IsDeleted = true;
                        item.UpdatedDate = DateTime.Now;
                        item.UpdatedBy = "system";
                        await _detailRepo.UpdateAsync(item);
                    }
                }

                // 2. Add / Update
                if (dto.Details != null && dto.Details.Count > 0)
                {
                    var existing = _detailRepo.GetAll(x => x.ProjectGateStepID == stepId && x.IsDeleted != true);

                    foreach (var d in dto.Details)
                    {
                        if (string.IsNullOrWhiteSpace(d.FileRule))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "Quy tắc không được để trống"));
                        }

                        if (d.ID <= 0)
                        {
                            // Add new
                            var newDetail = new ProjectGateStepCheckListDetail
                            {
                                ProjectGateStepID = stepId,
                                FileRule = d.FileRule,
                                FileFormat = d.FileFormat,
                                FileQuantity = d.FileQuantity >= 0 ? d.FileQuantity : 0,
                                IsCheck = d.IsCheck,
                                IsFile = d.IsFile,
                                STT = d.STT,
                                FileName = d.FileName,
                                CreatedDate = DateTime.Now,
                                CreatedBy = d.CreatedBy,
                                IsDeleted = false
                            };
                            await _detailRepo.CreateAsync(newDetail);
                        }
                        else
                        {
                            // Update existing
                            var item = existing.FirstOrDefault(x => x.ID == d.ID);
                            if (item != null)
                            {
                                item.FileRule = d.FileRule;
                                item.FileFormat = d.FileFormat;
                                item.FileQuantity = d.FileQuantity >= 0 ? d.FileQuantity : 0;
                                item.IsCheck = d.IsCheck;
                                item.IsFile = d.IsFile;
                                item.STT = d.STT;
                                item.FileName = d.FileName;
                                item.UpdatedDate = DateTime.Now;
                                item.UpdatedBy = d.CreatedBy;
                                await _detailRepo.UpdateAsync(item);
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu danh sách quy tắc thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
