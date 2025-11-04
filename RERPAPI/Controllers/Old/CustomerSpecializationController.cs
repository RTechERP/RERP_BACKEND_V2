using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerSpecializationController : ControllerBase
    {
        CustomerSpecializationRepo _customerSpecializationRepo = new CustomerSpecializationRepo();
        [HttpPost("save-data")]
        public async Task<IActionResult> Post(CustomerSpecialization model)
        {
            try
            {

                //TN.Binh update 19/10/25
                if (!CheckCustomerSpecializationCode(model))
                {
                    return Ok(new { status = 0, message = $"Mã ngành nghề [{model.Code}] đã tồn tại!" });
                }
                //end update 
                if (model.ID > 0)
                {
                    await _customerSpecializationRepo.UpdateAsync(model);
                }
                else
                {
                    await _customerSpecializationRepo.CreateAsync(model);
                }

                return Ok(ApiResponseFactory.Success("", "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // TN.Binh update 27/10/25 

        #region check trùng mã code
        private bool CheckCustomerSpecializationCode(CustomerSpecialization dto)
        {
            bool check = true;
            var exists = _customerSpecializationRepo.GetAll(x => x.Code == dto.Code
                            && x.ID != dto.ID && dto.IsDeleted !=true).ToList();
            if (exists.Count > 0) check = false;
            return check;
        }
        //end update
        #endregion

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var data = _customerSpecializationRepo.GetAll();
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("search")]
        public IActionResult Search(string? keyword)
        {
            try
            {
                keyword = keyword?.Trim()?.ToLower() ?? "";

                var query = _customerSpecializationRepo.GetAll()
             .Where(x => x.IsDeleted != true);

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Code) && x.Code.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(keyword))
                    );
                }

                return Ok(ApiResponseFactory.Success(query, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [HttpGet("get-detail")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                var data = _customerSpecializationRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(data, ""));


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
