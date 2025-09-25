using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System;
using RERPAPI.Model.DTO;

namespace RERPAPI.Controllers.HandoverMinutes
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandoverMinutesDetailController : ControllerBase
    {
        EmployeeRepo _employeeRepo = new EmployeeRepo();
        HandoverMinutesRepo _handoverMinutesRepo = new HandoverMinutesRepo();
        HandoverMinutesDetailRepo _handoverMinutesDetailRepo = new HandoverMinutesDetailRepo();
        DepartmentRepo _departmentRepo = new DepartmentRepo();
        POKHDetailRepo _pokhDetailRepo = new POKHDetailRepo();

        [HttpGet("get-employee")]
        public IActionResult GetEmployee()
        {
            try
            {
                List<Employee> employees = _employeeRepo.GetAll().Where(x => x.FullName != "").ToList();
                List<Department> departments = _departmentRepo.GetAll().ToList();
                var result = employees.Select(e => new
                {
                    Employee = e,
                    Department = departments.FirstOrDefault(d => d.ID == e.DepartmentID)
                });
                return Ok(ApiResponseFactory.Success(result,""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-pokh-detail")]
        public IActionResult GetPOKHDetail()
        {
            try
            {
                var product = SQLHelper<dynamic>.ProcedureToList("spGetPOKHDetail_New1", new string[] { }, new object[] { });
                return Ok(ApiResponseFactory.Success(product, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-handoverminutes")]
        public IActionResult GetHandoverMinutes()
        {
            try
            {
                var product = _handoverMinutesRepo.GetAll();
                return Ok(ApiResponseFactory.Success(product, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] HandoverMinutesDTO dto)
        {
            try
            {
                var model = _handoverMinutesRepo.GetByID(dto.ID) ?? new HandoverMinute();

                // Trường hợp soft delete
                if (dto.IsDeleted == true)
                {
                    model.IsDeleted = true;
                    model.UpdatedDate = DateTime.Now;
                    await _handoverMinutesRepo.UpdateAsync(model);
                    return Ok(new { status = 1, success = true, message = "Soft deleted successfully." });
                }
                model.DateMinutes = dto.DateMinutes;
                model.CustomerID = dto.CustomerID;
                model.CustomerAddress = dto.CustomerAddress;
                model.CustomerContact = dto.CustomerContact;
                model.CustomerPhone = dto.CustomerPhone;
                model.EmployeeID = dto.EmployeeID;
                model.Receiver = dto.Receiver;
                model.ReceiverPhone = dto.ReceiverPhone;
                model.AdminWarehouseID = dto.AdminWarehouseID;
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;
                //model.UpdatedBy = User.Identity.Name; // Mở comment nếu có phân quyền người dùng
                if (dto.ID > 0)
                {
                    await _handoverMinutesRepo.UpdateAsync(model);
                }
                else
                {
                    model.Code = $"BBBG.{DateTime.Now:yy}{DateTime.Now:MMdd}.{GetNextCodeNumber()}";
                    model.CreatedDate = DateTime.Now;
                    //model.CreatedBy = User.Idetity.Name; // Mở comment nếu có phân quyền người dùng
                    _handoverMinutesRepo.Create(model);
                }
                if(dto.DeletedDetailIds != null && dto.DeletedDetailIds.Count > 0)
                {
                    foreach (var item in dto.DeletedDetailIds)
                    {   
                        var detailToDelete = _handoverMinutesDetailRepo.GetByID(item);
                        if(detailToDelete != null)
                        {
                            detailToDelete.IsDeleted = true;
                            //detailToDelete.UpdatedBy = User.Identity.Name; // Mở comment nếu có phân quyền người dùng
                            detailToDelete.UpdatedDate = DateTime.Now;
                            await _handoverMinutesDetailRepo.UpdateAsync(detailToDelete);
                        }
                    }
                }
                if(dto.Details != null)
                {
                    for (int i = 0; i < dto.Details.Count; i++)
                    {
                        var detailRequest = dto.Details[i];
                        POKHDetail pOKHDetail = _pokhDetailRepo.GetByID(detailRequest.POKHDetailID) ?? new POKHDetail();
                        if (pOKHDetail == null) continue;
                        HandoverMinutesDetail detail = _handoverMinutesDetailRepo.GetByID(detailRequest.ID) ?? new HandoverMinutesDetail();
                        detail.STT = detailRequest.STT;
                        detail.HandoverMinutesID = model.ID;
                        detail.POKHID = pOKHDetail.POKHID;
                        detail.POKHDetailID = detailRequest.POKHDetailID;
                        detail.ProductSaleID = pOKHDetail.ProductID;
                        detail.Quantity = detailRequest.Quantity;
                        detail.ProductStatus = detailRequest.ProductStatus;
                        detail.Guarantee = detailRequest.Guarantee;
                        detail.DeliveryStatus = detailRequest.DeliveryStatus;
                        //detail.CreatedBy = GetCurrentUserName(); // Mở comment nếu có phân quyền người dùng
                        detail.CreatedDate = DateTime.Now;
                        //detail.UpdatedBy = GetCurrentUserName(); // Mở comment nếu có phân quyền người dùng
                        detail.UpdatedDate = DateTime.Now;
                        
                        if(detail.ID > 0)
                        {
                            await _handoverMinutesDetailRepo.UpdateAsync(detail);
                        }
                        else
                        {
                            _handoverMinutesDetailRepo.Create(detail);
                        }
                    }
                }    
                return Ok(ApiResponseFactory.Success(null,"Biên bản bàn giao được lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private string GetNextCodeNumber()
        {
            string prefix = $"BBBG.{DateTime.Now:yy}.{DateTime.Now:MMdd}.";

            List<string> existingCodes = _handoverMinutesRepo.GetAll()
                .Select(m => m.Code.Trim())
                .Where(c => c.StartsWith(prefix))
                .ToList();

            int maxNumber = 0;
            foreach (var code in existingCodes)
            {
                string[] parts = code.Split('.');
                if (parts.Length == 4 && int.TryParse(parts[3], out int num))
                {
                    maxNumber = Math.Max(maxNumber, num);
                }
            }

            return (maxNumber + 1).ToString("D3");
        }
    }
}
