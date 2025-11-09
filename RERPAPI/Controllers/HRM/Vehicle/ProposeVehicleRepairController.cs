using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Asset;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;

namespace RERPAPI.Controllers.HRM.Vehicle
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProposeVehicleRepairController : ControllerBase
    {
        private readonly ProposeVehicleRepairRepo _proposeVehicleRepairRepo;
        private readonly ProposeVehicleRepairDetailRepo _proposeVehicleRepairDetailRepo;

        public ProposeVehicleRepairController(
            ProposeVehicleRepairRepo proposeVehicleRepairRepo,
            ProposeVehicleRepairDetailRepo proposeVehicleRepairDetailRepo)
        {
            _proposeVehicleRepairRepo = proposeVehicleRepairRepo;
            _proposeVehicleRepairDetailRepo = proposeVehicleRepairDetailRepo;
        }
        //Lấy danh sách sửa chữa, bảo dưỡng
        [HttpPost("get-propose-vehicles-repair")]
        public IActionResult GetProposeVehicleRepair([FromBody] VehicleRepairRequestParam request)
        {
            try
            {
                var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                string procedureName = "spGetProposeVehicleRepair";
                string[] paramNames = new string[] { "@PageSize", "@PageNumber", "@DateStart", "@DateEnd", "@FilterText", "@EmployeeID", "@VehicleID", "@TypeID" };
                object[] paramValues = new object[] { request.Size, request.Page, request.DateStart ?? firstDay, request.DateEnd ?? lastDay, request.FilterText, request.EmployeeID, request.VehicleID, request.TypeID };
                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var propose = SQLHelper<object>.GetListData(data, 0);
                var TotalPage = SQLHelper<object>.GetListData(data, 1);
                return Ok(ApiResponseFactory.Success(new { propose, TotalPage }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Lấy danh sách chi tiết của đề xuất
        [HttpGet("get-propose-vehicle-repair-detail")]
        public IActionResult GetVehicleRepairType(int proposerVehicleRepairID)
        {
            try
            {
                string procedureName = "spGetProposeVehicleRepairDetail";
                string[] paramNames = new string[] { "@ProposerVehicleRepairID" };
                object[] paramValues = new object[] { proposerVehicleRepairID };
                var proposeDetail = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var dataList = SQLHelper<dynamic>.GetListData(proposeDetail, 0);
                return Ok(ApiResponseFactory.Success(new { dataList }, "Lấy chi tiết đề xuất thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        public class ProposeVehicleRepairDTO
        {
            public ProposeVehicleRepair? proposeVehicleRepair { get; set; }
            public List<ProposeVehicleRepairDetail>? proposeVehicleRepairDetails { get; set; }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProposeVehicleRepairDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu "));
                }
           
                if (dto.proposeVehicleRepair != null)
                {
                    if(dto.proposeVehicleRepair.IsDeleted==false||dto.proposeVehicleRepair.IsDeleted==null)
                    {
                        if (!_proposeVehicleRepairRepo.Validate(dto.proposeVehicleRepair, out string message))
                        {
                            return BadRequest(ApiResponseFactory.Fail(  null, message));
                        }
                    }    
                 

                    if (dto.proposeVehicleRepair.ID <= 0)
                    {
                        var maxSTT = _proposeVehicleRepairRepo.GetAll().Max(x => x.STT) + 1??0 ;
                        dto.proposeVehicleRepair.STT = maxSTT;
                        await _proposeVehicleRepairRepo.CreateAsync(dto.proposeVehicleRepair);
                    }          
                    else
                        await _proposeVehicleRepairRepo.UpdateAsync(dto.proposeVehicleRepair);


                }
                if (dto.proposeVehicleRepairDetails != null && dto.proposeVehicleRepairDetails.Any())
                {
                    foreach (var item in dto.proposeVehicleRepairDetails)
                    {
                        if (item.ID <= 0)
                        {
                            item.VehicleRepairProposeID = dto.proposeVehicleRepair.ID;
                            await _proposeVehicleRepairDetailRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _proposeVehicleRepairDetailRepo.UpdateAsync(item);
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
        [HttpPost("save-approve")]
        public async Task<IActionResult> SaveApprove([FromBody] List<ProposeVehicleRepairDetail>? proposeVehicleRepairDetails)
        {
            try
            {
               
                if (proposeVehicleRepairDetails != null && proposeVehicleRepairDetails.Any())
                {
                    foreach (var item in proposeVehicleRepairDetails)
                    {
                        if (item.ID <= 0)
                        {
                          
                            await _proposeVehicleRepairDetailRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _proposeVehicleRepairDetailRepo.UpdateAsync(item);
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
