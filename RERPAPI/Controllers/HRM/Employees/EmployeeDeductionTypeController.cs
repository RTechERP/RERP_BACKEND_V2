using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeDeductionTypeController : ControllerBase
    {
        private readonly EmployeeDeductionTypeRepo _employeeDeductionTypeRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;

        public EmployeeDeductionTypeController(EmployeeDeductionTypeRepo employeeDeductionTypeRepo, vUserGroupLinksRepo vUserGroupLinksRepo)
        {
            _employeeDeductionTypeRepo = employeeDeductionTypeRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
        }
       //API lấy danh sách tiền phatjt
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _employeeDeductionTypeRepo.GetAll(x=>x.IsDeleted!=true).ToList();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API lấy dữ liệu để sửa
        [RequiresPermission("N1,N2")]
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var data = _employeeDeductionTypeRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API lưu loại tiền phạt
        [RequiresPermission("N1,N2")]
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] EmployeeDeductionType model)
        {
            try
            {
                if (model.ID == 0)
                {
               
                  await _employeeDeductionTypeRepo.CreateAsync(model);
                }
                else
                {
                    var existing = _employeeDeductionTypeRepo.GetByID(model.ID);
                    if (existing == null) return NotFound(ApiResponseFactory.Fail(null,"Không tìm thấy dữ liệu"));
                    existing.DeductionTypeCode = model.DeductionTypeCode;
                    existing.DeductionTypeName = model.DeductionTypeName;
                    existing.MoneyLevel1 = model.MoneyLevel1;
                    existing.MoneyLevel2 = model.MoneyLevel2;
                    existing.Note = model.Note;
                    await _employeeDeductionTypeRepo.UpdateAsync(existing);
                }
                return Ok(ApiResponseFactory.Success(model, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API xóa loại phạt
        [RequiresPermission("N1,N2")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existing = _employeeDeductionTypeRepo.GetByID(id);
                if (existing == null) return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu"));
                existing.IsDeleted = true;
              await  _employeeDeductionTypeRepo.UpdateAsync(existing);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API check tiền phạt theo chức vụ
        [RequiresPermission("N1,N2")]
        [HttpGet("check-amount-level")]
        public IActionResult CheckAmountLevel([FromQuery] int employeeID)
        {
            try
            {
                var arrParamName = new string[] { "@EmployeeID" };
                var arrParamValue = new object[] { employeeID };
                var employeeBussiness = SQLHelper<object>.ProcedureToList("spCheckEmployeeDeductionCost", arrParamName, arrParamValue);
                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);

                return Ok(new
                {
                    status = 1,
                    data = result
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
