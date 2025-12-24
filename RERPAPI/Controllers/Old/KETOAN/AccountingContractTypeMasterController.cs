using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.KETOAN
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountingContractTypeMasterController : ControllerBase
    {
        private readonly AccountingContractTypeRepo _accountingContractTypeRepo;
        public AccountingContractTypeMasterController(AccountingContractTypeRepo accountingContractTypeRepo)
        {
            _accountingContractTypeRepo = accountingContractTypeRepo;
        }

        [HttpGet("get-data")]
        [RequiresPermission("N53,N52,N1,N36")]
        public IActionResult Get(string keywords = "")
        {
            try
            {
                keywords = keywords.Trim() ?? "";
                var data = _accountingContractTypeRepo.GetAll(x => string.IsNullOrEmpty(keywords) || x.TypeCode.Contains(keywords) || x.TypeName.Contains(keywords)).OrderBy(x => x.STT).ToList();

                //var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        [RequiresPermission("N53,N52,N1,N36")]
        public IActionResult Save(AccountingContractType request)
        {
            try
            {
                int idOld = request.ID;
                AccountingContractType model = idOld > 0 ? _accountingContractTypeRepo.GetByID(idOld) : new AccountingContractType();
                model.STT = request.STT;
                model.TypeCode = request.TypeCode;
                model.TypeName = request.TypeName;
                model.IsContractValue = request.IsContractValue;

                if(model.ID > 0 )
                {
                    _accountingContractTypeRepo.Update(model);
                }
                else
                {
                    _accountingContractTypeRepo.Create(model);
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
