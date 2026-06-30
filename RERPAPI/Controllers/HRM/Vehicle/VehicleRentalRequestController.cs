using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using RERPAPI.Attributes;

namespace RERPAPI.Controllers.HRM.Vehicle
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleRentalRequestController : ControllerBase
    {
        private readonly VehicleRentalRequestRepo _vehicleRentalRequestRepo;
        private readonly RTCContext _context;

        public VehicleRentalRequestController(VehicleRentalRequestRepo vehicleRentalRequestRepo, RTCContext context)
        {
            _vehicleRentalRequestRepo = vehicleRentalRequestRepo;
            _context = context;
        }
        [RequiresPermission("N1,N34")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                var data = await connection.QueryAsync<VehicleRentalRequestDto>(
                    "spGetVehicleRentalRequests",
                    commandType: CommandType.StoredProcedure
                );
                return Ok(ApiResponseFactory.Success(data.ToList(), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N34")]
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] VehicleRentalRequestSearchParam param)
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@Keyword", param.Keyword ?? "");
                parameters.Add("@StartDate", param.StartDate);
                parameters.Add("@EndDate", param.EndDate);
                parameters.Add("@EmployeeRequestID", param.EmployeeRequestID);
                parameters.Add("@EmployeeID", param.EmployeeID);
                parameters.Add("@DepartmentID", param.DepartmentID);

                var data = await connection.QueryAsync<VehicleRentalRequestDto>(
                    "spGetVehicleRentalRequests",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return Ok(ApiResponseFactory.Success(data.ToList(), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N34")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<VehicleRentalRequest> dto)
        {
            try
            {
                if (dto == null || dto.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ"));
                }

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var item in dto)
                {
                    if (item.ID <= 0)
                    {
                        item.EmployeeID = currentUser.EmployeeID;
                        await _vehicleRentalRequestRepo.CreateAsync(item);
                    }
                    else
                    {
                        await _vehicleRentalRequestRepo.UpdateAsync(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N34")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));
                }

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                foreach (var id in ids)
                {
                    var entity = await _vehicleRentalRequestRepo.GetByIDAsync(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        await _vehicleRentalRequestRepo.UpdateAsync(entity);
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
