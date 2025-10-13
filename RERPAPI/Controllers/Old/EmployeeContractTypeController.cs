using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeContractTypeController : ControllerBase
    {
        EmployeeContractTypeRepo employeeContractTypeRepo = new EmployeeContractTypeRepo();
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var employeeContractTypes = employeeContractTypeRepo.GetAll();
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
        public IActionResult GetEmployeeContractTypeByID(int id)
        {
            try
            {
                var employeeContractType = employeeContractTypeRepo.GetByID(id);
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
        public async Task<IActionResult> SaveEmployeeContractType([FromBody] EmployeeLoaiHDLD employeeContractType)
        {
            try
            {
                
                List<EmployeeLoaiHDLD> employeeContractTypes = employeeContractTypeRepo.GetAll();
                if (employeeContractTypes.Any(x => (x.Name == employeeContractType.Name || x.Code == employeeContractType.Code) && x.ID != employeeContractType.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Tên hoặc mã loại hợp đồng đã tồn tại"
                    });
                }
                if (employeeContractType.ID <= 0)
                {
                    employeeContractType.CreatedDate = DateTime.Now;
                    await employeeContractTypeRepo.CreateAsync(employeeContractType);
                }
                else
                {
                    employeeContractType.UpdatedDate = DateTime.Now;
                    await employeeContractTypeRepo.UpdateAsync(employeeContractType);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công"
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

    }
}
