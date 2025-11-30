using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.Old.POKH
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerPartController : ControllerBase
    {
        private readonly RTCContext _context;
        private readonly CustomerRepo _customerRepo;

        public CustomerPartController(RTCContext context, CustomerRepo customerRepo)
        {
            _context = context;
            _customerRepo = customerRepo;
        }
        [HttpGet("get-part")]
        public IActionResult LoadPart(int id)
        {
            //var s = (from a in db.CustomerParts join b in db.Customers on a.CustomerID equals b.ID select a).ToList();
            //return Ok(new
            //{
            //    status = 1,
            //    data = s
            //});
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetCustomerPart", new string[] { "@ID" }, new object[] { id });
                //List<dynamic> listPart = list[0];
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-customer")]
        public IActionResult LoadCustomer()
        {
            try
            {
                List<Customer> list = _customerRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveCustomerParts([FromBody] CustomerPartSaveModel model)
        {
            try
            {
                if (model.AddedParts != null)
                {
                    foreach (var part in model.AddedParts)
                    {
                        if (part.CustomerID == 0)
                        {
                            throw new Exception("CustomerID không được để trống");
                        }
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Xử lý xóa
                        if (model.DeletedPartIds != null && model.DeletedPartIds.Any())
                        {
                            foreach (var id in model.DeletedPartIds)
                            {
                                var partToDelete = new CustomerPart { ID = id };
                                _context.CustomerParts.Attach(partToDelete);
                                _context.CustomerParts.Remove(partToDelete);
                            }
                        }
                        // Xử lý thêm mới
                        if (model.AddedParts != null && model.AddedParts.Any())
                        {
                            _context.CustomerParts.AddRange(model.AddedParts);
                        }
                        // Xử lý cập nhật
                        if (model.UpdatedParts != null && model.UpdatedParts.Any())
                        {
                            foreach (var part in model.UpdatedParts)
                            {
                                var existingPart = _context.CustomerParts.Find(part.ID);
                                if (existingPart != null)
                                {
                                    _context.Entry(existingPart).CurrentValues.SetValues(part);
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
