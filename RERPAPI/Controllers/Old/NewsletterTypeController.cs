using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterTypeController : ControllerBase
    {
        private readonly NewsletterTypeRepo _newsletterTypeRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        PhasedAllocationPersonRepo _phaseRepo;
        PhasedAllocationPersonDetailRepo _phaseDetailRepo;

        public NewsletterTypeController(NewsletterTypeRepo newsletterTypeRepo, vUserGroupLinksRepo vUserGroupLinksRepo, PhasedAllocationPersonRepo phaseRepo, PhasedAllocationPersonDetailRepo phasedAllocationPersonDetailRepo
)
        {
            _phaseRepo = phaseRepo;
            _newsletterTypeRepo = newsletterTypeRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _phaseDetailRepo = phasedAllocationPersonDetailRepo;
        }


        [HttpGet]
        //[RequiresPermission("N2,N23,N34,N1,N80")]
        public IActionResult GetNewsletterType()
        {
            try
            {
                var lstNewsletterType = _newsletterTypeRepo.GetAll(x => x.IsDeleted != true).OrderBy(x => x.STT);

                return Ok(ApiResponseFactory.Success(lstNewsletterType, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        //[RequiresPermission("N2,N23,N34,N1,N80")]
        public async Task<IActionResult> SaveData([FromBody] NewsletterType newsletterType)
        {
            try
            {
                var now = DateTime.Now;
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var today = now.Date;
                var yesterday = today.AddDays(-1);
                // Các quyền được phép thao tác theo EmployeeID tùy ý
                string[] allowCodes = new[] { "N23", "N1", "N2", "N34", "N80" };

                // Kiểm tra user có thuộc nhóm quyền HR hay không
                var vUserHR = _vUserGroupLinksRepo
                    .GetAll()
                    .FirstOrDefault(x => (x.Code == "N23" || x.Code == "N1" || x.Code == "N2" || x.Code == "N34") && x.UserID == currentUser.ID);

                
                
                if (newsletterType.ID <= 0)
                {   
                    newsletterType.CreatedBy = currentUser.LoginName;
                    newsletterType.CreatedDate = now;
                    await _newsletterTypeRepo.CreateAsync(newsletterType);
                }
                else
                {

                    newsletterType.UpdatedBy = currentUser.LoginName;
                    newsletterType.UpdatedDate = now;
                    await _newsletterTypeRepo.UpdateAsync(newsletterType);
                }
                return Ok(ApiResponseFactory.Success(newsletterType, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
