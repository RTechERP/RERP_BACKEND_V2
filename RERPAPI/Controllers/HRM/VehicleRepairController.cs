using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleRepairController : ControllerBase
    {
        private readonly VehicleManagementRepo _vehicleManagementRepo;
        private readonly VehicleCategoryRepo _vehicleCategoryRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly VehicleRepairRepo _vehicleRepairRepo;
        private readonly VehicleRepairTypeRepo _vehicleRepairTypeRepo;

        public VehicleRepairController(
            VehicleManagementRepo vehicleManagementRepo,
            VehicleCategoryRepo vehicleCategoryRepo,
            EmployeeRepo employeeRepo,
            VehicleRepairRepo vehicleRepairRepo,
            VehicleRepairTypeRepo vehicleRepairTypeRepo)
        {
            _vehicleManagementRepo = vehicleManagementRepo;
            _vehicleCategoryRepo = vehicleCategoryRepo;
            _employeeRepo = employeeRepo;
            _vehicleRepairRepo = vehicleRepairRepo;
            _vehicleRepairTypeRepo = vehicleRepairTypeRepo;
        }
        //Lấy danh sách sửa chữa, bảo dưỡng
        [HttpPost("get-vehicles-repair")]
        public IActionResult GetVehicleRepair([FromBody] VehicleRepairRequestParam request)
        {
            try
            {
                
                var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                string procedureName = "spGetVehicleRepair";
                string[] paramNames = new string[] { "@PageSize", "@PageNumber", "@DateStart", "@DateEnd", "@FilterText", "@EmployeeID", "@VehicleID", "@TypeID" };
                object[] paramValues = new object[] { request.Size, request.Page, request.DateStart ?? firstDay, request.DateEnd ?? lastDay, request.FilterText, request.EmployeeID, request.VehicleID, request.TypeID };
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var vehicleRepair = SQLHelper<object>.GetListData(data, 0);
                var TotalPage = SQLHelper<object>.GetListData(data, 1);
                return Ok(ApiResponseFactory.Success(new { vehicleRepair, TotalPage  }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Lấy danh sách loại xe sửa chữa, bảo dưỡng
        [HttpGet("get-vehicle-repair-type")]
        public IActionResult GetVehicleRepairType()
        {
            try
            {
                var repairtype = _vehicleRepairTypeRepo.GetAll().FindAll(x => x.IsDeleted != true).ToList();
                return Ok(ApiResponseFactory.Success(repairtype, "Lấy loại sửa chữa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] VehicleRepairDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu "));
                }
                else
                {
                 
                    if (dto.vehicleRepair != null && dto.vehicleRepair.IsDeleted != true)
                    {
                        if (!_vehicleRepairRepo.Validate(dto.vehicleRepair, out string message))
                            return BadRequest(ApiResponseFactory.Fail(null, message));
                    }
                    if (dto.vehicleRepairType != null && dto.vehicleRepairType.IsDeleted != true)
                    {
                        if (!_vehicleRepairTypeRepo.Validate(dto.vehicleRepairType, out string message))
                            return BadRequest(ApiResponseFactory.Fail(null, message));
                    }
                    if (dto.vehicleRepair != null )
                    {
                        if (dto.vehicleRepair.ID > 0)
                        {
                            await _vehicleRepairRepo.UpdateAsync(dto.vehicleRepair);
                        }
                        else
                        {
                            var maxSTT = _vehicleRepairRepo.GetAll().Select(x => x.STT).Max() + 1;
                            dto.vehicleRepair.STT = maxSTT;
                            await _vehicleRepairRepo.CreateAsync(dto.vehicleRepair);
                        }
                        return Ok(ApiResponseFactory.Success(null, "Thêm thành công"));
                    }
                    if (dto.vehicleRepairType != null )
                    {
                        if (dto.vehicleRepairType.ID > 0)
                        {
                            await _vehicleRepairTypeRepo.UpdateAsync(dto.vehicleRepairType);
                        }
                        else
                        {
                            var maxStt = _vehicleRepairTypeRepo.GetAll().Select(x => x.STT).Max() + 1;
                            dto.vehicleRepairType.STT = maxStt;
                            await _vehicleRepairTypeRepo.CreateAsync(dto.vehicleRepairType);
                        }
                        return Ok(ApiResponseFactory.Success(null, "Thêm thành công"));
                    }
                  
                  
                    return BadRequest(ApiResponseFactory.Fail(null,"Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
