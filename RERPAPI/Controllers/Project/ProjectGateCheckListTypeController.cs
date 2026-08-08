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
    /// Controller quản lý danh mục loại checklist của Project Gate
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateCheckListTypeController : ControllerBase
    {
        private readonly ProjectGateCheckListTypeRepo _projectGateCheckListTypeRepo;

        public ProjectGateCheckListTypeController(ProjectGateCheckListTypeRepo projectGateCheckListTypeRepo)
        {
            _projectGateCheckListTypeRepo = projectGateCheckListTypeRepo;
        }

        /// <summary>
        /// Lấy tất cả danh sách loại checklist, sắp xếp theo số thứ tự hiển thị (STT)
        /// </summary>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _projectGateCheckListTypeRepo.GetAll(x => x.IsDeleted == false)
                                                       .OrderBy(x => x.STT ?? int.MaxValue)
                                                       .ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật danh sách loại checklist
        /// </summary>
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectGateCheckListType> dto)
        {
            try
            {
                if (dto == null || dto.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }

                foreach (var item in dto)
                {
                    // Kiểm tra trùng lặp mã TypeCode
                    if (!string.IsNullOrEmpty(item.TypeCode))
                    {
                        var duplicate = _projectGateCheckListTypeRepo.GetAll(x => x.TypeCode.ToLower() == item.TypeCode.ToLower() && x.ID != item.ID && x.IsDeleted == false);
                        if (duplicate.Count > 0)
                        {
                            return Ok(new APIResponse
                            {
                                status = 2,
                                message = $"Mã loại checklist '{item.TypeCode}' đã tồn tại!"
                            });
                        }
                    }

                    if (item.ID <= 0)
                    {
                        await _projectGateCheckListTypeRepo.CreateAsync(item);
                    }
                    else
                    {
                        await _projectGateCheckListTypeRepo.UpdateAsync(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xóa danh sách loại checklist (xóa mềm bằng cách cập nhật IsDeleted = true)
        /// </summary>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));
                }

                foreach (var item in ids)
                {
                    var entity = _projectGateCheckListTypeRepo.GetByID(item);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        await _projectGateCheckListTypeRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(ids, "Xóa dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
