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
    /// Controller quản lý các mẫu bước/công đoạn thực hiện (ProjectGateStepTemplate)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepTemplateController : ControllerBase
    {
        private readonly ProjectGateStepTemplateRepo _templateRepo;

        public ProjectGateStepTemplateController(ProjectGateStepTemplateRepo templateRepo)
        {
            _templateRepo = templateRepo;
        }

        /// <summary>
        /// Lấy tất cả danh sách mẫu bước/công đoạn
        /// </summary>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _templateRepo.GetAll().OrderBy(x => x.Code).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật danh sách các mẫu bước/công đoạn
        /// </summary>
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectGateStepTemplate> dto)
        {
            try
            {
                if (dto == null || dto.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }

                foreach (var item in dto)
                {
                    // Kiểm tra trùng lặp mã Code
                    if (!string.IsNullOrEmpty(item.Code))
                    {
                        var duplicate = _templateRepo.GetAll(x => x.Code.ToLower() == item.Code.ToLower() && x.ID != item.ID);
                        if (duplicate.Count > 0)
                        {
                            return Ok(new APIResponse
                            {
                                status = 2,
                                message = $"Mã mẫu '{item.Code}' đã tồn tại!"
                            });
                        }
                    }

                    if (item.ID <= 0)
                    {
                        await _templateRepo.CreateAsync(item);
                    }
                    else
                    {
                        await _templateRepo.UpdateAsync(item);
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
        /// Xóa danh sách các mẫu bước/công đoạn theo ID
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

                var itemsToDelete = _templateRepo.GetAll(x => ids.Contains(x.ID));
                if (itemsToDelete.Count > 0)
                {
                    await _templateRepo.DeleteRangeAsync(itemsToDelete);
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
