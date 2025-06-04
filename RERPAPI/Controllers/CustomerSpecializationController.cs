using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerSpecializationController : ControllerBase
    {
        CustomerSpecializationRepo customerSpecializationRepo = new CustomerSpecializationRepo();
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<CustomerSpecialization> customerSpecializations = customerSpecializationRepo.GetAll();
               
                return Ok(new
                {
                    status = 1,
                    data = customerSpecializations
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


        [HttpGet("getCustomerSpecializationById")]
        public IActionResult GetCustomerSpecializationByID(int customerSpecializationID)
        {
            try
            {
                var customerSpecialization = customerSpecializationRepo.GetByID(customerSpecializationID);
                return Ok(new
                {
                    status = 1,
                    data = customerSpecialization
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

        [HttpPost("saveCustomerSpecialization")]
        public async Task<IActionResult> SaveCustomerSpecialization([FromBody]CustomerSpecialization customerSpecialization)
        {
            try
            {
                List<CustomerSpecialization> listData = customerSpecializationRepo.GetAll();
                customerSpecialization.STT = listData.Count + 1;
                customerSpecialization.CreatedDate = DateTime.Now;
                customerSpecialization.UpdatedDate = DateTime.Now;
                if (customerSpecialization.ID <= 0)
                {
                    await customerSpecializationRepo.CreateAsync(customerSpecialization);
                } else
                {
                    customerSpecializationRepo.UpdateFieldsByID(customerSpecialization.ID, customerSpecialization);
                }
                    return Ok(new
                {
                    status = 1,
                    data = customerSpecialization,
                    message = "Ngành nghề được lưu thành công"
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


        [HttpDelete("deleteCustomerSpecialization")]
        public IActionResult DeleteCustomerSpecialization(int customerSpecializationID)
        {
            try
            {
                var customerSpecialization = customerSpecializationRepo.GetByID(customerSpecializationID);
                List<Customer> checkList = SQLHelper<Customer>.FindByAttribute("CustomerSpecializationID", customerSpecializationID).ToList();
                if(checkList.Count > 0)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Không thể xóa vì mã này đã được khách hàng sử dụng."
                    });
                }
                if (customerSpecialization != null)
                {
                    customerSpecializationRepo.Delete(customerSpecialization.ID);
                    return Ok(new
                    {
                        status = 1,
                        message = "Customer Specialization deleted successfully."
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Customer Specialization not found."
                    });
                }
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
