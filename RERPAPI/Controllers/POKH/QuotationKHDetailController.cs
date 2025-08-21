using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RERPAPI.Controllers.PO
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationKHDetailController : ControllerBase
    {
        //loadUser();
        //loadCustomer(); // Dùng bên customerPart
        //loadProject();  // Dùng bên pokh
        //loadProduct();  // Dùng bên RID
        //loadContact();
        UserRepo _userRepo = new UserRepo();
        CustomerRepo _customerRepo = new CustomerRepo();
        CustomerContactRepo _customerContactRepo = new CustomerContactRepo();
        QuotationKHRepo _quotationKHRepo = new QuotationKHRepo();
        QuotationKHDetailRepo _quotationDetailKHRepo = new QuotationKHDetailRepo();
        [HttpGet("get-users")]
        public IActionResult GetUser()
        {
            try
            {
                var list = _userRepo.GetAll().Select(x => new
                {
                    x.ID,
                    x.FullName,
                    x.Code,
                    UserInfo = x.Code + " - " + x.FullName
                }).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-contacts")]
        public IActionResult GetCustomerContact(int id)
        {
            try
            {
                var list = _customerContactRepo.GetAll().Where(x => x.CustomerID == id).Select(x => new
                {
                    x.ContactPhone,
                    x.ContactEmail,
                    x.ContactName,
                    x.ID
                }).ToList();
                return Ok(ApiResponseFactory.Success(list, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("generate-code")]
        public IActionResult GenerateQuotationKHCode(int customerId, string createDate)
        {
            try
            {
                // Lấy tên viết tắt khách hàng
                string? customer = _customerRepo.GetAll()
                    .Where(x => x.ID == customerId)
                    .Select(x => x.CustomerShortName)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(customer))
                {
                    return Ok(new
                    {
                        status = 0,
                        message = "Không tìm thấy tên viết tắt khách hàng",
                        code = ""
                    });
                }

                // Lấy danh sách các báo giá có mã chứa tên khách hàng
                var quotations = _quotationKHRepo.GetAll()
                    .Where(x => !string.IsNullOrEmpty(x.QuotationCode) && x.QuotationCode.Contains(customer))
                    .OrderByDescending(x => x.ID)
                    .ToList();

                int stt = 1;
                if (quotations.Count > 0)
                {
                    var lastQuotation = quotations.First();
                    var codeParts = (lastQuotation.QuotationCode ?? "").Split('_');
                    if (codeParts.Length > 0 && int.TryParse(codeParts.Last(), out int lastStt))
                    {
                        stt = lastStt + 1;
                    }
                }

                // Tạo code mới
                string code = $"RTC_QUO_{customer}_{DateOnly.Parse(createDate):ddMMyyyy}_{stt}";
                return Ok(ApiResponseFactory.Success(code, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveDataAsync([FromBody] QuotationDTO dto)
        {
            try
            {
                if(dto.quotationKHs.ID <= 0)
                {
                    await _quotationKHRepo.CreateAsync(dto.quotationKHs);
                }
                else
                {
                    await _quotationKHRepo.UpdateAsync(dto.quotationKHs);
                }
                if (dto.DeletedDetailIds != null && dto.DeletedDetailIds.Count > 0)
                {
                    foreach (var item in dto.DeletedDetailIds)
                    {
                        var detailToDelete = _quotationDetailKHRepo.GetByID(item);
                        if (detailToDelete != null)
                        {
                            detailToDelete.IsDeleted = true;
                            //detailToDelete.UpdatedBy = User.Identity.Name; // Mở comment nếu có phân quyền người dùng
                            await _quotationDetailKHRepo.UpdateAsync(detailToDelete);
                        }
                    }
                }
                if (dto.quotationKHDetails.Count > 0)
                {
                    foreach (var item in dto.quotationKHDetails)
                    {
                        if (item.ID <= 0)
                        {
                            item.QuotationKHID = dto.quotationKHs.ID;
                            await _quotationDetailKHRepo.CreateAsync(item);
                        }
                        else
                        {
                            await _quotationDetailKHRepo.UpdateAsync(item);
                        }
                    }
                } 
                return Ok(ApiResponseFactory.Success(null,"Success"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
