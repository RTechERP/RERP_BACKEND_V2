using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericCourseEntity;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize]
    public class TravelRegistrationController : ControllerBase
    {
        private readonly CurrentUser _currentUser;
        private readonly TravelRegistrationRepo _travelRegistrationRepo;
        public TravelRegistrationController(CurrentUser currentUser, TravelRegistrationRepo travelRegistrationRepo)
        {
            _currentUser = currentUser;
            _travelRegistrationRepo = travelRegistrationRepo;
        }
        [RequiresPermission("N34,N2")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = _travelRegistrationRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N34,N2")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetByID(int ID)
        {
            try
            {
                var data = _travelRegistrationRepo.GetByID(ID);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N34,N2")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] TravelRegistration obj)
        {
            try
            {
                TravelRegistration? exist = null;
                if (obj.ID > 0)
                {
                    var code = obj.EmployeeCode.Trim().ToLower();
                    var fullName = obj.EmployeeName.Trim().ToLower();
                    // exist = _travelRegistrationRepo.GetByID(obj.ID);
                    exist = _travelRegistrationRepo.GetAll(x => !x.IsDeleted && x.EmployeeCode.ToLower() == code && x.EmployeeName.ToLower() == fullName).FirstOrDefault();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(obj.EmployeeCode))
                    {
                        var code = obj.EmployeeCode.Trim().ToLower();
                        var fullName = obj.EmployeeName.Trim().ToLower();
                        exist = _travelRegistrationRepo.GetAll(x => !x.IsDeleted && x.EmployeeCode.ToLower() == code&&x.EmployeeName.ToLower()==fullName).FirstOrDefault();
                    }

                    if (exist == null && !string.IsNullOrWhiteSpace(obj.EmployeeName))
                    {
                        var name = obj.EmployeeName.Trim().ToLower();
                        if (obj.OwnerEmployeeID > 0)
                        {
                            exist = _travelRegistrationRepo.GetAll(x => !x.IsDeleted 
                                && x.OwnerEmployeeID == obj.OwnerEmployeeID 
                                && x.EmployeeName.ToLower() == name
                                && (string.IsNullOrEmpty(obj.Relationship) || x.Relationship == obj.Relationship)).FirstOrDefault();
                        }
                        else if (obj.EmployeeID > 0)
                        {
                            exist = _travelRegistrationRepo.GetAll(x => !x.IsDeleted 
                                && x.EmployeeID == obj.EmployeeID 
                                && x.EmployeeName.ToLower() == name).FirstOrDefault();
                        }
                    }
                }

                if (exist != null)
                {
                    obj.ID = exist.ID;
                    obj.CreatedDate = exist.CreatedDate;
                    obj.CreatedBy = exist.CreatedBy;
                    obj.IsDeleted = exist.IsDeleted;
                    if (obj.IsPublish == null) obj.IsPublish = exist.IsPublish;
                    obj.UpdatedDate = DateTime.Now;
                    obj.UpdatedBy = _currentUser.FullName;
                    await _travelRegistrationRepo.UpdateAsync(obj);
                }
                else
                {
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = _currentUser.FullName;
                    await _travelRegistrationRepo.CreateAsync(obj);
                }

                return Ok(ApiResponseFactory.Success(1, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N34,N2")]
        [HttpGet("delete-by-id")]
        public async Task<IActionResult> DeleteByID(int ID)
        {
            try
            {
                TravelRegistration model = _travelRegistrationRepo.GetByID(ID);
                model.IsDeleted = true;
                await _travelRegistrationRepo.UpdateAsync(model);
                return Ok(ApiResponseFactory.Success("", "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("confirm-travel-registration")]
        public async Task<IActionResult> ConfirmTravelRegistration(int employeeId, int confirmStatus)
        {
            try
            {
                var list = _travelRegistrationRepo.GetAll(x =>
                        x.OwnerEmployeeID == employeeId
                        && !x.IsDeleted)
                    .ToList();

                foreach (var item in list)
                {
                    item.ConfirmStatus = confirmStatus;
                    item.ConfirmDate = DateTime.Now;
                    item.ConfirmBy = _currentUser.FullName;
                }

                await _travelRegistrationRepo.UpdateRangeAsync(list);

                return Ok(ApiResponseFactory.Success("", "Xác nhận thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-by-employee")]
        public IActionResult GetByEmployee()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                int employeeId = currentUser.EmployeeID;

                var data = _travelRegistrationRepo
                    .GetAll(x => x.OwnerEmployeeID == employeeId
                            && x.IsPublish == true  
                            && !x.IsDeleted)
                    .OrderBy(x => x.Relationship);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N34,N2")]
        [HttpGet("update-publish")]
        public async Task<IActionResult> UpdatePublish(bool isPublish)
        {
            try
            {
                var list = _travelRegistrationRepo.GetAll(x => !x.IsDeleted).ToList();
                foreach (var item in list)
                {
                    item.IsPublish = isPublish;
                    item.UpdatedBy = _currentUser.FullName;
                    item.UpdatedDate = DateTime.Now;
                }
                await _travelRegistrationRepo.UpdateRangeAsync(list);
                return Ok(ApiResponseFactory.Success(1, "Cập nhật công bố thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
