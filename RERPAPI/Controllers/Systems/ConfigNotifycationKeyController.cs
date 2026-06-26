using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Systems;

namespace RERPAPI.Controllers.Systems
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigNotifycationKeyController : ControllerBase
    {
        private readonly ConfigNotificationKeyRepo _configNotificationKeyRepo;
        private readonly ConfigNotificationKeyLinkRepo _configNotificationKeyLinkRepo;

        public ConfigNotifycationKeyController(ConfigNotificationKeyRepo configNotificationKeyRepo, ConfigNotificationKeyLinkRepo configNotificationKeyLinkRepo)
        {
            _configNotificationKeyRepo = configNotificationKeyRepo;
            _configNotificationKeyLinkRepo = configNotificationKeyLinkRepo;
        }
        //Lấy danh sách key
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var listData = _configNotificationKeyRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(listData, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Lưu dữ liệu
        [RequiresPermission("N1")]
        [HttpPost("save-data")]
        public async Task<IActionResult> Save(List<ConfigNotificationKey> list)
        {
            try
            {
                int result = 0;
                if (list == null) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var validateResult = _configNotificationKeyRepo.Validate(item);
                        if (validateResult.status == 0)
                        {
                            return BadRequest(validateResult);
                        }
                        if (item.ID > 0)
                        {
                            result = await _configNotificationKeyRepo.UpdateAsync(item);
                        }
                        else
                        {
                            result = await _configNotificationKeyRepo.CreateAsync(item);
                        }
                    }
                }
                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công!"));
                }
                else
                {
                    return Ok(ApiResponseFactory.Success(null, "Không có bản ghi nào được lưu"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1")]
        [HttpPost("deleted")]
        public async Task<IActionResult> Delete(List<int> list)
        {
            try
            {
                if (list == null) return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var config = _configNotificationKeyRepo.GetByID(item);
                        config.IsDeleted = true;
                        await _configNotificationKeyRepo.UpdateAsync(config);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //lấy link duyệt theo nhân viên
        [HttpGet("get-by-employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(int employeeId)
        {
            try
            {
                var allKeys = _configNotificationKeyRepo.GetAll(x => x.IsDeleted != true).ToList();
                var existingLinks = _configNotificationKeyLinkRepo.GetAll(x => x.EmployeeID == employeeId).ToList();

                var resultList = new List<object>();

                foreach (var key in allKeys)
                {
                    var link = existingLinks.FirstOrDefault(x => x.ConfigNotificationKeyID == key.ID);
                    if (link == null)
                    {
                        link = new ConfigNotificationKeyLink
                        {
                            EmployeeID = employeeId,
                            ConfigNotificationKeyID = key.ID,
                            IsActive = true
                        };
                        await _configNotificationKeyLinkRepo.CreateAsync(link);
                    }

                    resultList.Add(new
                    {
                        LinkID = link.ID,
                        EmployeeID = link.EmployeeID,
                        ConfigNotificationKeyID = key.ID,
                        KeyName = key.KeyName,
                        KeyContent = key.KeyContent,
                        IsActive = link.IsActive
                    });
                }

                return Ok(ApiResponseFactory.Success(resultList, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Update trạng thái link duyệt
        [HttpPost("update-link-status")]
        public async Task<IActionResult> UpdateLinkStatus([FromBody] ConfigNotificationKeyLink request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                if (request.ID > 0)
                {
                    await _configNotificationKeyLinkRepo.UpdateAsync(request);
                }
                else
                {
                    await _configNotificationKeyLinkRepo.CreateAsync(request);
                }

                return Ok(ApiResponseFactory.Success(request, "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //Kiểm tra xem nhân viên có bật thông báo cho 1 mã cấu hình không
        [HttpGet("check-notification")]
        public IActionResult CheckNotification([FromQuery] string code, [FromQuery] int employeeId)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var key = _configNotificationKeyRepo.GetAll(x => x.KeyCode == code && x.IsDeleted != true).FirstOrDefault();
                if (key == null) return Ok(ApiResponseFactory.Success(true, "Không tìm thấy mã cấu hình, mặc định bật"));

                var link = _configNotificationKeyLinkRepo.GetAll(x => x.ConfigNotificationKeyID == key.ID && x.EmployeeID == currentUser.EmployeeID&& x.IsActive!=true).FirstOrDefault();
                if (link == null)
                {
                    // Nếu chưa có cài đặt thì mặc định là true như hàm GetByEmployee
                    return Ok(ApiResponseFactory.Success(true, "Thành công"));
                }

                return Ok(ApiResponseFactory.Success(link.IsActive, "Thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}