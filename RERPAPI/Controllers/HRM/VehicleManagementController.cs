using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleManagementController : ControllerBase
    {
        private readonly VehicleManagementRepo _vehicleManagementRepo;
        private readonly VehicleCategoryRepo _vehicleCategoryRepo;
        private readonly EmployeeRepo _employeeRepo;

        public VehicleManagementController(
            VehicleManagementRepo vehicleManagementRepo,
            VehicleCategoryRepo vehicleCategoryRepo,
            EmployeeRepo employeeRepo)
        {
            _vehicleManagementRepo = vehicleManagementRepo;
            _vehicleCategoryRepo = vehicleCategoryRepo;
            _employeeRepo = employeeRepo;
        }
        // GET: /api/vehiclemanagement
        [HttpGet("get-vehicles")]
        public IActionResult GetVehicles()
        {
            try
            {
                string procedureName = "spGetVehicleManagement";
                string[] paramNames = new string[] { };
                object[] paramValues = new object[] { };
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // GET: api/vehiclemanagement/vehiclecategory
        [HttpGet("get-vehicle-category")]
        public IActionResult GetVehicleCategory()
        {
            try
            {
                var data = _vehicleCategoryRepo.GetAll(x=>x.IsDelete!=true).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // GET: /api/vehiclemanagement/employee
        [HttpGet("get-employee")]
        public IActionResult GetEmployee()
        {
            try
            {
                string procedureName = "spGetEmployeeAndEmployeeApprover";
                string[] paramNames = new string[] { };
                object[] paramValues = new object[] { };
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);


                return Ok(new
                {
                    data = result,
                    Status = 1
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }
        // GET: /api/vehiclemanagement/employee
        [HttpGet("EmployeeSDT")]
        public IActionResult GetEmployeeSDT(int id)
        {
            try
            {
                var employee = _employeeRepo.GetByID(id); 
                return Ok(ApiResponseFactory.Success(employee, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // POST: api/vehiclemanagement
        [HttpPost("save-vehicle")]
        public async Task<IActionResult> SaveVehicle([FromBody] VehicleManagement vehicle)
        {
            try
            {
                if (vehicle != null && vehicle.IsDeleted != true)
                {
                    if (!_vehicleManagementRepo.Validate(vehicle, out string message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (vehicle.ID > 0)
                {
                    await _vehicleManagementRepo.UpdateAsync(vehicle);
                    return Ok(ApiResponseFactory.Success(vehicle, "Sửa phương tiện thành công"));
                }
                else
                {
                    var result = await _vehicleManagementRepo.CreateAsync(vehicle);
                    return Ok(ApiResponseFactory.Success(vehicle, "Thêm phương tiện thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // POST: api/vehiclemanagement/vehiclecategory
        [HttpPost("save-vehicle-category")]
        public async Task<IActionResult> SaveDataVehicleCategoryAsync([FromBody] VehicleCategory vehicleCategory)
        {
            try
            {
                if (vehicleCategory != null && vehicleCategory.IsDelete != true)
                {
                    if (!_vehicleCategoryRepo.Validate(vehicleCategory, out string message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (vehicleCategory.ID > 0)
                {
                    await _vehicleCategoryRepo.UpdateAsync(vehicleCategory);
                    return Ok(ApiResponseFactory.Success(vehicleCategory, "Sửa danh mục thành công"));
                }
                else
                {
                    var result = await _vehicleCategoryRepo.CreateAsync(vehicleCategory);
                    return Ok(ApiResponseFactory.Success(vehicleCategory, "Thêm danh mục thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
