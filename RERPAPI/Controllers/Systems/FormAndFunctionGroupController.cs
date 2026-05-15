    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Systems;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Systems
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FormAndFunctionGroupController : ControllerBase
    {
        private readonly FormAndFunctionGroupRepo _repo;
        private readonly FormAndFunctionRepo _functionRepo;

        public FormAndFunctionGroupController(FormAndFunctionGroupRepo repo, FormAndFunctionRepo functionRepo)
        {
            _repo = repo;
            _functionRepo = functionRepo;
        }


        [RequiresPermission(",", permissionFunction : "userPermissionForm")]
        // Lấy tất cả danh sách nhóm chức năng
        [HttpGet("")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _repo.GetAll(x => !x.IsHide).OrderBy(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy thông tin nhóm chức năng theo ID
        [RequiresPermission(",", permissionFunction : "userPermissionForm")]
        [HttpGet("get-by-id")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _repo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lưu thông tin nhóm chức năng (Thêm mới hoặc Cập nhật)
        [RequiresPermission(",", permissionFunction : "userPermissionForm")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] FormAndFunctionGroup model)
        {
            try
            {
                if (!model.IsHide)
                {
                    var validate = _repo.Validate(model);
                    if (validate.status == 0)
                    {
                        return BadRequest(validate);
                    }
                }

                if (model.ID <= 0)
                {
                    await _repo.CreateAsync(model);
                }
                else
                {
                    await _repo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(model, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



        // Lấy tất cả danh sách chức năng
        [RequiresPermission(",", permissionFunction : "userPermissionForm")]
        [HttpGet("get-functions")]
        public IActionResult GetAllFunctions()
        {
            try
            {
                var data = _functionRepo.GetAll(x => !x.IsHide).OrderBy(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy danh sách chức năng theo nhóm
        [RequiresPermission(",", permissionFunction : "userPermissionForm")]
        [HttpGet("get-by-group")]
        public IActionResult GetByGroup(int groupId)
        {
            try
            {
                var data = _functionRepo
                    .GetAll(x => !x.IsHide && x.FormAndFunctionGroupID == groupId)
                    .OrderBy(x => x.ID)
                    .ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission(",",permissionFunction : "userPermissionForm")]
        // Lưu thông tin chức năng (Thêm mới hoặc Cập nhật)
        [HttpPost("save-function")]
        public async Task<IActionResult> SaveFunction([FromBody] FormAndFunction model)
        {
            try
            {
                if (model.ID <= 0)
                {
                    await _functionRepo.CreateAsync(model);
                }
                else
                {
                    await _functionRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(model, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}

