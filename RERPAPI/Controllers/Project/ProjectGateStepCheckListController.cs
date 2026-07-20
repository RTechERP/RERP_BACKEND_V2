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
    /// API quản lý CheckList và CheckListDetail của từng bước Gate Step
    /// Route: api/ProjectGateStepCheckList
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepCheckListController : ControllerBase
    {
        private readonly ProjectGateStepCheckListRepo _checkListRepo;
        private readonly ProjectGateStepCheckListDetailRepo _detailRepo;

        public ProjectGateStepCheckListController(
            ProjectGateStepCheckListRepo checkListRepo,
            ProjectGateStepCheckListDetailRepo detailRepo)
        {
            _checkListRepo = checkListRepo;
            _detailRepo = detailRepo;
        }

        /// <summary>
        /// Lấy danh sách CheckList theo ProjectGateStepID (chỉ lấy bảng Master)
        /// </summary>
        [HttpGet("get-checklist-by-step/{stepId}")]
        public IActionResult GetCheckListByStep(int stepId)
        {
            try
            {
                var data = _checkListRepo
                    .GetAll(x => x.ProjectGateStepID == stepId)
                    .OrderBy(x => x.ID)
                    .ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách CheckListDetail theo ProjectGateStepCheckListID
        /// </summary>
        [HttpGet("get-details-by-checklist/{checkListId}")]
        public IActionResult GetDetailsByCheckList(int checkListId)
        {
            try
            {
                var data = _detailRepo
                    .GetAll(x => x.ProjectGateStepCheckListID == checkListId && x.IsDeleted != true)
                    .OrderBy(x => x.ID)
                    .ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách CheckList + CheckListDetail theo ProjectGateStepID
        /// </summary>
        [HttpGet("get-by-step/{stepId}")]
        public IActionResult GetByStep(int stepId)
        {
            try
            {
                var checkLists = _checkListRepo
                    .GetAll(x => x.ProjectGateStepID == stepId)
                    .OrderBy(x => x.ID)
                    .ToList();

                var checkListIds = checkLists.Select(x => x.ID).ToList();

                var details = _detailRepo
                    .GetAll(x => checkListIds.Contains(x.ProjectGateStepCheckListID) && x.IsDeleted != true)
                    .OrderBy(x => x.ProjectGateStepCheckListID)
                    .ThenBy(x => x.ID)
                    .ToList();

                // Group details by CheckListID
                var result = checkLists.Select(cl => new
                {
                    cl.ID,
                    cl.ProjectGateStepID,
                    cl.Type,
                    cl.ProjectGateCheckListType,
                    cl.Description,
                    cl.PathFolder,
                    Details = details
                        .Where(d => d.ProjectGateStepCheckListID == cl.ID)
                        .Select(d => new
                        {
                            d.ID,
                            d.ProjectGateStepCheckListID,
                            d.FileRule,
                            d.FileFormat,
                            d.FileQuantity,
                            d.IsCheck,
                            d.CreatedDate,
                            d.CreatedBy,
                            d.UpdatedDate,
                            d.UpdatedBy
                        }).ToList()
                }).ToList();

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu (thêm/sửa) CheckListDetail cho một CheckList cụ thể
        /// Body: List<ProjectGateStepCheckListDetailDto>
        /// </summary>
        [HttpPost("save-details/{checkListId}")]
        public async Task<IActionResult> SaveDetails(int checkListId, [FromBody] SaveCheckListDetailsDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                // Verify checklist exists
                var checkList = _checkListRepo.GetAll(x => x.ID == checkListId).FirstOrDefault();
                if (checkList == null)
                    return NotFound(ApiResponseFactory.Fail(null, $"Không tìm thấy CheckList ID={checkListId}"));

                // 1. Process soft deletes
                if (dto.DeletedIds != null && dto.DeletedIds.Count > 0)
                {
                    var toDelete = _detailRepo.GetAll(x => dto.DeletedIds.Contains(x.ID) && x.ProjectGateStepCheckListID == checkListId);
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
                    var existing = _detailRepo.GetAll(x => x.ProjectGateStepCheckListID == checkListId && x.IsDeleted != true);

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
                                ProjectGateStepCheckListID = checkListId,
                                FileRule = d.FileRule,
                                FileFormat = d.FileFormat,
                                FileQuantity = d.FileQuantity >= 0 ? d.FileQuantity : 0,
                                IsCheck = d.IsCheck,
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
                                item.UpdatedDate = DateTime.Now;
                                item.UpdatedBy = d.CreatedBy;
                                await _detailRepo.UpdateAsync(item);
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu CheckListDetail thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu danh sách CheckList (Master) theo step (Thêm/Sửa/Xóa an toàn không ảnh hưởng details của dòng khác)
        /// Body: List<ProjectGateStepCheckList>
        /// </summary>
        [HttpPost("save-checklist/{stepId}")]
        public async Task<IActionResult> SaveCheckList(int stepId, [FromBody] List<ProjectGateStepCheckList> dtos)
        {
            try
            {
                if (dtos == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                var existing = _checkListRepo.GetAll(x => x.ProjectGateStepID == stepId);
                var inputIds = dtos.Where(x => x.ID > 0).Select(x => x.ID).ToList();

                // 1. Delete checklists that are not in the input list (and their details)
                var toDelete = existing.Where(x => !inputIds.Contains(x.ID)).ToList();
                if (toDelete.Count > 0)
                {
                    var toDeleteIds = toDelete.Select(x => x.ID).ToList();
                    var detailsToDelete = _detailRepo.GetAll(x => toDeleteIds.Contains(x.ProjectGateStepCheckListID));
                    if (detailsToDelete.Count > 0)
                        await _detailRepo.DeleteRangeAsync(detailsToDelete);

                    await _checkListRepo.DeleteRangeAsync(toDelete);
                }

                // 2. Add / Update
                foreach (var dto in dtos)
                {
                    if (dto.ID <= 0)
                    {
                        // Add new
                        var newCl = new ProjectGateStepCheckList
                        {
                            ProjectGateStepID = stepId,
                            Type = dto.Type,
                            ProjectGateCheckListType = dto.ProjectGateCheckListType,
                            Description = dto.Description,
                            PathFolder = dto.PathFolder,
                        };
                        await _checkListRepo.CreateAsync(newCl);
                    }
                    else
                    {
                        // Update existing
                        var item = existing.FirstOrDefault(x => x.ID == dto.ID);
                        if (item != null)
                        {
                            item.Type = dto.Type;
                            item.ProjectGateCheckListType = dto.ProjectGateCheckListType;
                            item.Description = dto.Description;
                            item.PathFolder = dto.PathFolder;
                            await _checkListRepo.UpdateAsync(item);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu danh sách Checklist thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu toàn bộ CheckList + Details theo step (xóa cũ, insert mới)
        /// Body: List<CheckListWithDetailsDto>
        /// </summary>
        [HttpPost("save-by-step/{stepId}")]
        public async Task<IActionResult> SaveByStep(int stepId, [FromBody] List<CheckListWithDetailsDto> dtos)
        {
            try
            {
                if (dtos == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));

                // Get existing checklists for this step
                var existingCheckLists = _checkListRepo.GetAll(x => x.ProjectGateStepID == stepId);
                var existingIds = existingCheckLists.Select(x => x.ID).ToList();

                // Delete all old details first
                if (existingIds.Count > 0)
                {
                    var oldDetails = _detailRepo.GetAll(x => existingIds.Contains(x.ProjectGateStepCheckListID));
                    if (oldDetails.Count > 0)
                        await _detailRepo.DeleteRangeAsync(oldDetails);

                    // Delete old checklists
                    await _checkListRepo.DeleteRangeAsync(existingCheckLists);
                }

                // Insert new checklists + details
                foreach (var dto in dtos)
                {
                    var newCheckList = new ProjectGateStepCheckList
                    {
                        ProjectGateStepID = stepId,
                        Type = dto.Type,
                        ProjectGateCheckListType = dto.ProjectGateCheckListType,
                        Description = dto.Description,
                        PathFolder = dto.PathFolder,
                    };

                    await _checkListRepo.CreateAsync(newCheckList);
                    int newCheckListId = newCheckList.ID;

                    if (dto.Details != null && dto.Details.Count > 0)
                    {
                        var newDetails = dto.Details.Select(d => new ProjectGateStepCheckListDetail
                        {
                            ProjectGateStepCheckListID = newCheckListId,
                            FileRule = d.FileRule,
                            FileFormat = d.FileFormat,
                            FileQuantity = d.FileQuantity > 0 ? d.FileQuantity : 1,
                            IsCheck = d.IsCheck,
                            CreatedDate = DateTime.Now,
                            CreatedBy = d.CreatedBy
                        }).ToList();

                        await _detailRepo.CreateRangeAsync(newDetails);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu CheckList thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
