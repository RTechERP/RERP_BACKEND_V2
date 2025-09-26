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
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<CustomerSpecialization> customerSpecializations = customerSpecializationRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
               
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


        [HttpGet("{id}")]
        public IActionResult GetCustomerSpecializationByID(int id)
        {
            try
            {
                var customerSpecialization = customerSpecializationRepo.GetByID(id);
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

        [HttpPost]
        public async Task<IActionResult> SaveCustomerSpecialization([FromBody]CustomerSpecialization customerSpecialization)
        {
            try
            {
                List<CustomerSpecialization> listData = customerSpecializationRepo.GetAll();
                if (listData.Any(x => (x.Name == customerSpecialization.Name || x.Code == customerSpecialization.Code) && x.ID != customerSpecialization.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Tên hoặc mã ngành nghề đã tồn tại"
                    });
                }
                customerSpecialization.STT = listData.Count + 1;
                customerSpecialization.CreatedDate = DateTime.Now;
                customerSpecialization.UpdatedDate = DateTime.Now;
                if (customerSpecialization.ID <= 0)
                {
                    await customerSpecializationRepo.CreateAsync(customerSpecialization);
                } else
                {
                    await customerSpecializationRepo.UpdateAsync(customerSpecialization);
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


        //[HttpDelete("{id}")]
        //public IActionResult DeleteCustomerSpecialization(int id)
        //{
        //    try
        //    {
        //        var customerSpecialization = customerSpecializationRepo.GetByID(id);
        //        List<Customer> checkList = SQLHelper<Customer>.FindByAttribute("CustomerSpecializationID", id).ToList();
        //        if(checkList.Count > 0)
        //        {
        //            return BadRequest(new
        //            {
        //                status = 0,
        //                message = "Không thể xóa vì mã này đã được khách hàng sử dụng."
        //            });
        //        }
        //        if (customerSpecialization != null)
        //        {
        //            customerSpecializationRepo.Delete(customerSpecialization.ID);
        //            return Ok(new
        //            {
        //                status = 1,
        //                message = "Customer Specialization deleted successfully."
        //            });
        //        }
        //        else
        //        {
        //            return NotFound(new
        //            {
        //                status = 0,
        //                message = "Customer Specialization not found."
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}
    }
}
