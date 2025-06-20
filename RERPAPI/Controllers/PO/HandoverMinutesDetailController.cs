using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System;
using RERPAPI.Model.DTO;

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandoverMinutesDetailController : ControllerBase
    {
        EmployeeRepo _employeeRepo = new EmployeeRepo();
        HandoverMinutesRepo _handoverMinutesRepo = new HandoverMinutesRepo();
        HandoverMinutesDetailRepo _handoverMinutesDetailRepo = new HandoverMinutesDetailRepo();
        DepartmentRepo _departmentRepo = new DepartmentRepo();
        CustomerRepo _customerRepo = new CustomerRepo();
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
                return Ok(new
                {
                    status = 1,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("get-pokh-detail")]
        public IActionResult GetPOKHDetail()
        {
            try
            {
                var product = SQLHelper<dynamic>.ProcedureToDynamicLists("spGetPOKHDetail_New1", new string[] { }, new object[] { });
                return Ok(new
                {
                    status = 1,
                    data = product
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("get-handoverminutes")]
        public IActionResult GetHandoverMinutes()
        {
            try
            {
                var product = _handoverMinutesRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = product
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpPost("save-data")]
        public IActionResult SaveData([FromBody] HandoverMinutesDTO dto )
        {
            try
            {
                HandoverMinute model = _handoverMinutesRepo.GetByID(dto.ID) ?? new HandoverMinute();
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
                //model.UpdatedBy = User.Identity.Name; // Mở comment nếu có phân quyền người dùng
                if (dto.ID > 0)
                {
                    _handoverMinutesRepo.UpdateFieldsByID(dto.ID, model);
                }
                else
                {
                    model.Code = $"BBBG.{DateTime.Now:yy}{DateTime.Now:MMdd}.{GetNextCodeNumber()}";
                    model.CreatedDate = DateTime.Now;
                    //model.CreatedBy = User.Identity.Name; // Mở comment nếu có phân quyền người dùng
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
                            _handoverMinutesDetailRepo.UpdateFieldsByID(item, detailToDelete);
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
                            _handoverMinutesDetailRepo.UpdateFieldsByID(detailRequest.ID, detail);
                        }
                        else
                        {
                            _handoverMinutesDetailRepo.Create(detail);
                        }
                    }
                }    
                return Ok(new
                {
                    status = 1,
                    message = "Biên bản bàn giao đã được lưu thành công.",
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
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
