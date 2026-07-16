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
    /// Controller quản lý các Gate kiểm soát trong quy trình dự án
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateController : ControllerBase
    {
        private readonly ProjectGateRepo _projectGateRepo;

        public ProjectGateController(ProjectGateRepo projectGateRepo)
        {
            _projectGateRepo = projectGateRepo;
        }

        /// <summary>
        /// Lấy tất cả danh sách Gate, sắp xếp theo số thứ tự hiển thị (STT)
        /// </summary>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _projectGateRepo.GetAll().OrderBy(x => x.STT ?? int.MaxValue).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật danh sách các Gate
        /// </summary>
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<ProjectGate> dto)
        {
            try
            {
                if (dto == null || dto.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }

                foreach (var item in dto)
                {
                    // Kiểm tra trùng lặp mã GateCode
                    if (!string.IsNullOrEmpty(item.GateCode))
                    {
                        var duplicate = _projectGateRepo.GetAll(x => x.GateCode.ToLower() == item.GateCode.ToLower() && x.ID != item.ID);
                        if (duplicate.Count > 0)
                        {
                            return Ok(new APIResponse
                            {
                                status = 2,
                                message = $"Mã Gate '{item.GateCode}' đã tồn tại!"
                            });
                        }
                    }

                    if (item.ID <= 0)
                    {
                        await _projectGateRepo.CreateAsync(item);
                    }
                    else
                    {
                        await _projectGateRepo.UpdateAsync(item);
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
        /// Xóa danh sách các Gate theo ID (phương thức cứng vì bảng không có IsDeleted)
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

                var itemsToDelete = _projectGateRepo.GetAll(x => ids.Contains(x.ID));
                if (itemsToDelete.Count > 0)
                {
                    await _projectGateRepo.DeleteRangeAsync(itemsToDelete);
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
