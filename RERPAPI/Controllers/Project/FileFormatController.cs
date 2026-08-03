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
    /// Controller quản lý danh mục định dạng file (FileFormat)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileFormatController : ControllerBase
    {
        private readonly FileFormatRepo _fileFormatRepo;

        public FileFormatController(FileFormatRepo fileFormatRepo)
        {
            _fileFormatRepo = fileFormatRepo;
        }

        /// <summary>
        /// Lấy tất cả danh sách định dạng file chưa bị xóa, sắp xếp theo số thứ tự (STT)
        /// </summary>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _fileFormatRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null)
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
        /// Thêm mới hoặc cập nhật danh sách các định dạng file
        /// </summary>
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<FileFormat> dto)
        {
            try
            {
                if (dto == null || dto.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }

                foreach (var item in dto)
                {
                    // Kiểm tra trùng lặp đuôi mở rộng (Extension)
                    if (!string.IsNullOrEmpty(item.Extension))
                    {
                        var ext = item.Extension.Trim().ToLower();
                        var duplicate = _fileFormatRepo.GetAll(x => (x.IsDeleted == false || x.IsDeleted == null) &&
                                                                   x.Extension != null && x.Extension.Trim().ToLower() == ext &&
                                                                   x.ID != item.ID);
                        if (duplicate.Count > 0)
                        {
                            return Ok(new APIResponse
                            {
                                status = 2,
                                message = $"Đuôi mở rộng '{item.Extension}' đã tồn tại!"
                            });
                        }
                    }

                    if (item.ID <= 0)
                    {
                        await _fileFormatRepo.CreateAsync(item);
                    }
                    else
                    {
                        await _fileFormatRepo.UpdateAsync(item);
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
        /// Xóa mềm danh sách định dạng file theo danh sách ID
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

                foreach (var id in ids)
                {
                    var entity = _fileFormatRepo.GetByID(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        await _fileFormatRepo.UpdateAsync(entity);
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
