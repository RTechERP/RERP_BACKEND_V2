using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeContractTypeController : ControllerBase
    {
        private EmployeeContractTypeRepo _employeeContractTypeRepo;
        public EmployeeContractTypeController(EmployeeContractTypeRepo employeeContractTypeRepo)
        {
            _employeeContractTypeRepo = employeeContractTypeRepo;
        }
        [HttpGet]
        [RequiresPermission("N1,N2")]
        public IActionResult GetAll()
        {
            try
            {
                var employeeContractTypes = _employeeContractTypeRepo.GetAll(x=> x.IsDeleted != true);
                return Ok(new
                {
                    status = 1,
                    data = employeeContractTypes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpGet("{id}")]
        [RequiresPermission("N1,N2")]
        public IActionResult GetEmployeeContractTypeByID(int id)
        {
            try
            {
                var employeeContractType = _employeeContractTypeRepo.GetByID(id);
                return Ok(new
                {
                    status = 1,
                    data = employeeContractType
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost]
        [RequiresPermission("N1,N2")]
        public async Task<IActionResult> SaveEmployeeContractType([FromBody] EmployeeLoaiHDLD employeeContractType)
        {
            try
            {
                
                List<EmployeeLoaiHDLD> employeeContractTypes = _employeeContractTypeRepo.GetAll();
                if (employeeContractTypes.Any(x => (x.Name == employeeContractType.Name || x.Code == employeeContractType.Code) && x.ID != employeeContractType.ID))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên hoặc mã loại hợp đồng đã tồn tại"));
                }

                if(String.IsNullOrWhiteSpace(employeeContractType.Name) || String.IsNullOrWhiteSpace(employeeContractType.Code))
                {
                    return BadRequest(ApiResponseFactory.Fail(null,"Không được để trống mã hoặc tên hợp đồng !"));
                }

                employeeContractType.Code = employeeContractType.Code.Trim();
                employeeContractType.Name = employeeContractType.Name.Trim();

                if (employeeContractType.ID <= 0)
                {
                    employeeContractType.CreatedDate = DateTime.Now;
                    await _employeeContractTypeRepo.CreateAsync(employeeContractType);
                }
                else
                {
                    employeeContractType.UpdatedDate = DateTime.Now;
                    await _employeeContractTypeRepo.UpdateAsync(employeeContractType);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
