using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity.AddNewBillExport;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRM.Vehicle;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    
    public class EconomicContractController : ControllerBase
    {
        private readonly EconomicContractTermRepo _economicContractTermRepo;
        private readonly EconomicContractTypeRepo _economicContractTypeRepo;
        private readonly EconomicContractFileRepo _economicContractFileRepo;
        private readonly EconomicContractRepo _economicContractRepo;

        public EconomicContractController(EconomicContractTermRepo economicContractTermRepo, EconomicContractTypeRepo economicContractTypeRepo, EconomicContractFileRepo economicContractFileRepo, EconomicContractRepo economicContractRepo)
        {
            _economicContractTermRepo = economicContractTermRepo;
            _economicContractTypeRepo = economicContractTypeRepo;
            _economicContractFileRepo = economicContractFileRepo;
            _economicContractRepo = economicContractRepo;
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("get-economic-contract")]
        public IActionResult GetEconomicContract([FromBody] EconomicContractRequestParam request)
        {
            try
            {
                var economicContract = SQLHelper<dynamic>.ProcedureToList(
                   "spGetEconomicContract",
                   new[] { "@DateStart", "@DateEnd", "@Keyword", "@TypeNCC", "@Type" },
                   new object[] {request.DateStart??DateTime.MinValue, request.DateEnd??DateTime.MaxValue,request.Keyword??"", request.TypeNCC??0,request.Type??0});
                var dataList = SQLHelper<dynamic>.GetListData(economicContract, 0);
                return Ok(ApiResponseFactory.Success(economicContract, "Lấy dữ liệu thành công"));
            }

            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("save-contract")]
        public async Task<IActionResult> SaveContract([FromBody] EconomicContract item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }
                else
                {
                    //if (term != null && term.IsDeleted != true)
                    //{
                    //    var validate = _economicContractTermRepo.Validate(term);
                    //    if (validate.status == 0) return BadRequest(validate);
                    //}
                    if (item != null)
                    {
                        if (item.ID > 0)
                        {
                            await _economicContractRepo.UpdateAsync(item);
                        }
                        else
                        {
                            await _economicContractRepo.CreateAsync(item);
                        }
                        return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("delete-contract")]
        public IActionResult DeleteContract([FromBody] List<int> ids)
        {
            try
            {
                if (ids.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xóa"));
                }
                else
                {
                    foreach (var id in ids)
                    {
                        var term = _economicContractRepo.GetByID(id);
                        if (term == null)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy dữ liệu với ID: {id}"));
                        }
                        term.IsDeleted = true;
                        _economicContractRepo.Update(term);
                    }
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-economic-contract-term")]
        public IActionResult GetEconomicContractTerms()
        {
            try
            {
                var data = _economicContractTermRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("save-contract-term")]
        public async Task<IActionResult> SaveContractTerm([FromBody] EconomicContractTerm term)
        {
            try
            {
                if (term == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }
                else
                {
                    if (term != null && term.IsDeleted != true)
                    {
                        var validate = _economicContractTermRepo.Validate(term);
                        if (validate.status == 0) return BadRequest(validate);
                    }
                    if (term != null)
                    {
                        if (term.ID > 0)
                        {
                            await _economicContractTermRepo.UpdateAsync(term);
                        }
                        else
                        {                          
                            await _economicContractTermRepo.CreateAsync(term);
                        }
                        return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("delete-term")]
        public IActionResult DeleteTerm([FromBody] List<int> ids)
        {
            try
            {
                if (ids.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xóa"));
                }
                else
                {
                    foreach (var id in ids)
                    {
                        var term = _economicContractTermRepo.GetByID(id);
                        if (term == null)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy dữ liệu với ID: {id}"));
                        }
                        term.IsDeleted = true;
                        _economicContractTermRepo.Update(term);
                    }
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-economic-contract-type")]
        public IActionResult GetEconomicContractType()
        {
            try
            {
                var data = _economicContractTypeRepo.GetAll(x => x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("save-contract-type")]
        public async Task<IActionResult> SaveContractType([FromBody] EconomicContractType term)
        {
            try
            {
                if (term == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }
                else
                {
                    if (term != null && term.IsDeleted != true)
                    {
                        var validate = _economicContractTypeRepo.Validate(term);
                        if (validate.status == 0) return BadRequest(validate);
                    }
                    if (term != null)
                    {
                        if (term.ID > 0)
                        {
                            await _economicContractTypeRepo.UpdateAsync(term);
                        }
                        else
                        {
                            await _economicContractTypeRepo.CreateAsync(term);
                        }
                        return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("delete-type")]
        public IActionResult DeleteType([FromBody] List<int> ids)
        {
            try
            {
                if (ids.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để xóa"));
                }
                else
                {
                    foreach (var id in ids)
                    {
                        var term = _economicContractTypeRepo.GetByID(id);
                        if (term == null)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy dữ liệu với ID: {id}"));
                        }
                        term.IsDeleted = true;
                        _economicContractTypeRepo.Update(term);
                    }
                    return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpGet("get-file-by-contract-id")]
        public IActionResult GetFileByContractID(int contractID)
        {
            try
            {
                var data = _economicContractFileRepo.GetAll(x => x.EconomicContractID == contractID && x.IsDeleted != true);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2,N34")]
        [HttpPost("save-contract-file")]
        public async Task<IActionResult> SaveContractFile([FromBody] EconomicContractFile item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu"));
                }
                else
                {
                    if (item != null)
                    {
                        if (item.ID > 0)
                        {
                            await _economicContractFileRepo.UpdateAsync(item);
                        }
                        else
                        {
                            await _economicContractFileRepo.CreateAsync(item);
                        }
                        return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Lưu dữ liệu không thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
