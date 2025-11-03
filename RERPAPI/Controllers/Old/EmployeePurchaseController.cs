using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeePurchaseController : ControllerBase
    {
        private readonly EmployeePurchaseRepo employeepurchaseRepo;
        private readonly TaxCompanyRepo taxcompanyRepo = new TaxCompanyRepo();

        /// <summary>


        public EmployeePurchaseController()
        {
            employeepurchaseRepo = new EmployeePurchaseRepo();
        }

        /// <summary>
        /// Lấy danh sách tất cả loại cuộc họp
        /// </summary>
        [HttpGet("employeepurchases")]
        public IActionResult GetEmployeePurchase(int employeeID, int taxCompanyID, string? keyword)
        {
            try
            {
                var employeePurchases = SQLHelper<object>.ProcedureToList("spGetEmployeePurchase",
                                                new string[] { "@EmployeeID", "@TaxCompanyID", "@Keyword" },
                                                new object[] { employeeID, taxCompanyID, keyword ?? "" });
                var data = SQLHelper<object>.GetListData(employeePurchases, 0);

                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employeepurchase/{id}")]

        public IActionResult GetByID(int id)
        {
            try
            {
                EmployeePurchase employeePurchase = employeepurchaseRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(employeePurchase, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employees")]
        public IActionResult GetEmployee()
        {
            try
            {
                var employees = SQLHelper<object>.ProcedureToList("spGetEmployee",
                                                new string[] { "@Status" },
                                                new object[] { 0 });


                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(employees, 0)
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



        [HttpGet("taxcompanies")]
        public IActionResult GetAll()
        {
            try
            {
                var taxcompanies = taxcompanyRepo.GetAll(p => p.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(taxcompanies, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }


        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] EmployeePurchase employeepurchase)
        {
            try
            {
                if (employeepurchase.ID <= 0)
                    await employeepurchaseRepo.CreateAsync(employeepurchase);
                else
                    await employeepurchaseRepo.UpdateAsync(employeepurchase);
                return Ok(ApiResponseFactory.Success(employeepurchase, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("checkduplicate/{employeeId}/{companyId}/{currentId?}")]
        public async Task<IActionResult> CheckDuplicate(int employeeId, int companyId, int currentId = 0)
        {
            try
            {
                bool check = false;

                var employeePurchase = employeepurchaseRepo.GetAll()
                    .Where(x => x.ID != currentId &&
                               x.EmployeeID == employeeId &&
                               x.TaxCompayID == companyId
                               //&& x.IsDelete == false
                               );

                if (employeePurchase.Count() > 0) check = true;

                return Ok(new
                {
                    status = 1,
                    data = check
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}