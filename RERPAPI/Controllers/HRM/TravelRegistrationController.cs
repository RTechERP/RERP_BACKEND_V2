using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] TravelRegistration obj)
        {
            try
            {
                if (obj.ID <= 0) await _travelRegistrationRepo.CreateAsync(obj);
                else _travelRegistrationRepo.Update(obj);

                return Ok(ApiResponseFactory.Success(1, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

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
        public IActionResult GetByEmployee(int employeeId)
        {
            try
            {
                var data = _travelRegistrationRepo
                    .GetAll(x => x.OwnerEmployeeID == employeeId
                            && !x.IsDeleted)
                    .OrderBy(x => x.Relationship);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
