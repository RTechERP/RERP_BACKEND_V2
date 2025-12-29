using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaxCompanyController : ControllerBase
    {
        private readonly TaxCompanyRepo _taxCompanyRepo;
        
        public TaxCompanyController(TaxCompanyRepo taxCompanyRepo)
        {
            _taxCompanyRepo = taxCompanyRepo;
        }

        [HttpGet("get-tax-companies")]
        public IActionResult GetTaxCompanies()
        {
            try
            {
                var data = _taxCompanyRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-tax-company")]
        public async Task<IActionResult> SaveTaxCompany(TaxCompany model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên công ty"));
                if (string.IsNullOrEmpty(model.Code))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập Mã công ty"));

                var models = _taxCompanyRepo.GetAll( x => x.Code == model.Code.Trim() && x.IsDeleted != true && x.ID != model.ID);
                if (models.Count > 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Mã công ty đã tồn tại"));

                if (model.ID > 0)
                {
                    await _taxCompanyRepo.UpdateAsync(model);
                }
                else
                {
                    var exist = _taxCompanyRepo.GetAll(x => x.Code == model.Code.Trim() && x.ID != model.ID && x.IsDeleted == true).FirstOrDefault();
                    if(exist != null)
                    {
                        return Ok(new
                        {
                            success = false,
                            needConfirm = true,
                            message =
                                                $"Đã tồn tại!\n" +
                                                $"Mã công ty: [{exist.Code}]\n" +
                                                $"Tên công ty: [{exist.Name}].\n\n" +
                                                $"Bạn có muốn sử dụng lại công ty [{exist.Name}] không?",
                            data = new
                            {
                                id = exist.ID
                            }
                        });
                    }
                    else
                    {
                        await _taxCompanyRepo.CreateAsync(model);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu công ty thuế thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("restore-tax-company")]
        public async Task<IActionResult> RestoreTaxCompany(int id)
        {
            var company = await _taxCompanyRepo.GetByIDAsync(id);
            if (company == null)
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy bản ghi cần sửa"));

            company.IsDeleted = false;
            await _taxCompanyRepo.UpdateAsync(company);

            return Ok(ApiResponseFactory.Success(null, "Khôi phục công ty thành công"));
        }

        [HttpGet("get-tax-company-by-id")]
        public async Task<IActionResult> GetTaxCompanyById(int id)
        {
            try
            {
                var company = await _taxCompanyRepo.GetByIDAsync(id);
                if (company == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy công ty"));

                return Ok(ApiResponseFactory.Success(company, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
