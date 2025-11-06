using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.IssueSolution
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueCauseController : ControllerBase
    {
        private readonly IssueCauseRepo _issueCauseRepo = new IssueCauseRepo();


        [HttpGet()]
        public IActionResult GetAllIssueCause()
        {
            try
            {
                var listIssueCause = _issueCauseRepo.GetAll().Where(x => x.IsDeleted != true).ToList();
                return Ok(ApiResponseFactory.Success(listIssueCause, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-detail")]
        public IActionResult GetIssueCauseDetail(int id)
        {
            try
            {
                var issueCause = _issueCauseRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(issueCause, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save")]
        public async Task<IActionResult> Save(IssueCause model)
        {
            try
            {
                if (model.ID <= 0)
                {
                    var existing = _issueCauseRepo.GetAll().FirstOrDefault(x => x.IssueCauseCode == model.IssueCauseCode && x.IsDeleted == false);
                    if(existing != null)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Mã trạng thái đã tồn tại."));
                    }    
                    await _issueCauseRepo.CreateAsync(model);
                }
                else
                {
                    await _issueCauseRepo.UpdateAsync(model);
                }

                return Ok(ApiResponseFactory.Success(null, "Success"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
