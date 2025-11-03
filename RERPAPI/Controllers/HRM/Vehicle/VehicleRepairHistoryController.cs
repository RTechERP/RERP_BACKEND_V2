using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using static RERPAPI.Controllers.HRM.Vehicle.ProposeVehicleRepairController;

namespace RERPAPI.Controllers.HRM.Vehicle
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleRepairHistoryController : ControllerBase
    {

        VehicleRepairHistoryRepo _vehicleRepairHistoryRepo = new VehicleRepairHistoryRepo();
        VehicleRepairHistoryFileRepo _vsehicleRepairHistoryFileRepo = new VehicleRepairHistoryFileRepo();

        //Lấy danh sách theo dõi sửa chữa theo xe nội bộ
        [HttpGet("get-vehicle-repair-history")]
        public IActionResult GetVehicleRepairHistory(int managementVehicleID)
        {
            try
            {
                string procedureName = "spGetVehicleRepairHistory";
                string[] paramNames = new string[] { "@ManagementVehicleID" };
                object[] paramValues = new object[] { managementVehicleID };
                var repairHistory = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var dataList = SQLHelper<dynamic>.GetListData(repairHistory, 0);
               
                return Ok(ApiResponseFactory.Success(new { dataList }, "Lấy danh sách theo dõi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Lấy danh sách file  theo dõi sửa chữa theo xe nội bộ
        [HttpGet("get-vehicle-repair-history-file")]
        public IActionResult GetVehicleRepairHistoryFile(int vehicleRepairHistoryID)
        {
            try
            {
                string procedureName = "spGetVehicleRepairHistoryFile";
                string[] paramNames = new string[] { "@VehicleRepairHistoryID" };
                object[] paramValues = new object[] { vehicleRepairHistoryID };
                var fileList = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var dataList = SQLHelper<dynamic>.GetListData(fileList, 0);

                return Ok(ApiResponseFactory.Success(new { dataList }, "Lấy danh sách file theo dõi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Khởi tạo DTO truyền vào hàm save
        public class VehicleRepairHistoryDTO
        {
            public VehicleRepairHistory? vehicleRepairHistory { get; set; }
            public List<VehicleRepairHistoryFile>? vehicleRepairHistoryFiles { get; set; }
        }
        //Lưu dữ liệu
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] VehicleRepairHistoryDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu "));
                }

                if (dto.vehicleRepairHistory != null)
                {
                    if (dto.vehicleRepairHistory.IsDeleted == false || dto.vehicleRepairHistory.IsDeleted == null)
                    {
                        if (!_vehicleRepairHistoryRepo.Validate(dto.vehicleRepairHistory, out string message))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, message));
                        }
                    }
                    if (dto.vehicleRepairHistory.ID <= 0)
                    {
                        var maxSTT = _vehicleRepairHistoryRepo.GetAll().Max(x => x.STT) + 1 ?? 0;
                        dto.vehicleRepairHistory.STT = maxSTT;
                        await _vehicleRepairHistoryRepo.CreateAsync(dto.vehicleRepairHistory);
                    }
                    else
                        await _vehicleRepairHistoryRepo.UpdateAsync(dto.vehicleRepairHistory);


                }
                if (dto.vehicleRepairHistoryFiles != null && dto.vehicleRepairHistoryFiles.Any())
                {
                    foreach (var item in dto.vehicleRepairHistoryFiles)
                    {
                        if (item.ID <= 0)
                        {
                           
                            await _vsehicleRepairHistoryFileRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _vsehicleRepairHistoryFileRepo.UpdateAsync(item);
                        }

                    }
                }
                return Ok(ApiResponseFactory.Success(null, " Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
