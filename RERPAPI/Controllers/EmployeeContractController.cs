using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeContractController : Controller
    {
        EmployeeContractRepo employeeContractRepo = new EmployeeContractRepo();

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<EmployeeContract> employeeContracts = employeeContractRepo.GetAll();
                return Ok(new
                {
                    status = 1,
                    data = employeeContracts
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


        [HttpGet("getEmployeeContract")]
        public IActionResult GetEmployeeContract(int employeeID, int employeeContractTypeID, string filterText)
        {
            try
            {
                filterText = string.IsNullOrWhiteSpace(filterText) ? "" : filterText;
                var employeeContracts = SQLHelper<object>.ProcedureToList("spGetEmployeeContract",
                                       new string[] { "@EmployeeID", "@EmployeeContractTypeID", "@FilterText" },
                                                          new object[] { employeeID, employeeContractTypeID, filterText });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(employeeContracts, 0)
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

        [HttpGet("getEmployeeContractByID")]
        public IActionResult GetEmployeeContractByID(int employeeContractID)
        {
            try
            {
                var employeeContract = employeeContractRepo.GetByID(employeeContractID);
                return Ok(new
                {
                    status = 1,
                    data = employeeContract
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

        [HttpPost("saveEmployeeContract")]
        public async Task<IActionResult> SaveEmployeeContract([FromBody] EmployeeContract employeeContract)
        {
            try
            {
               if(employeeContract.ID <= 0)
                {
                    await employeeContractRepo.CreateAsync(employeeContract);
                } else
                {
                    employeeContractRepo.UpdateFieldsByID(employeeContract.ID, employeeContract);
                }

                
                return Ok(new
                {
                    status = 1,
                    data = employeeContract,
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
