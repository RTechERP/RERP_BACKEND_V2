using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankListController : ControllerBase
    {
        private BankListRepo _bankListRepo;
        private PaymentOrderRepo _paymentOrderRepo;
        private SupplierSaleRepo _supplierSaleRepo;

        public BankListController(BankListRepo bankListRepo, PaymentOrderRepo paymentOrderRepo, SupplierSaleRepo supplierSaleRepo)
        {
            _bankListRepo = bankListRepo;
            _paymentOrderRepo = paymentOrderRepo;
            _supplierSaleRepo = supplierSaleRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetBankList()
        {
            try
            {
                List<BankList> bankList = _bankListRepo.GetAll(p => p.IsDeleted.HasValue && !p.IsDeleted.Value);

                return Ok(ApiResponseFactory.Success(bankList));
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankListById(int id)
        {
            try
            {
                BankList bankList = _bankListRepo.GetByID(id);
                if (bankList == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bank"));
                }
                return Ok(ApiResponseFactory.Success(bankList));
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        [RequiresPermission("N33,N35,N36,N55,N27")]
        public async Task<IActionResult> SaveData(BankList bankList)
        {
            try
            {
                int result = 0;
                if (bankList.ID > 0)
                {
                    result = await _bankListRepo.UpdateAsync(bankList);
                }
                else
                {
                    result = await _bankListRepo.CreateAsync(bankList);
                }
                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(bankList));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Failed to save bank list"));
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiresPermission("N33,N35,N36,N55,N27")]
        public async Task<IActionResult> DeleteBankList(int id)
        {
            try
            {
                bool hasPaymentOrders = _paymentOrderRepo.GetAll(p => p.BankListID == id && (!p.IsDelete.HasValue || !p.IsDelete.Value)).Any();
                if (hasPaymentOrders)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không thể xóa bank list vì đã được sử dụng tại đề nghị thanh toán!"));
                }
                BankList bankList = _bankListRepo.GetByID(id);
                if (bankList == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bank"));
                }
                bankList.IsDeleted = true;
                int result = await _bankListRepo.UpdateAsync(bankList);
                if (result > 0)
                {
                    return Ok(ApiResponseFactory.Success(null, "Bank list deleted successfully"));
                }
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Failed to delete bank list"));
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}