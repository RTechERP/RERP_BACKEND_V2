using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.GeneralCategory.TrainingRegistration
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingRegistrationFileController : ControllerBase
    {
        private TrainingRegistrationFileRepo _trainingRegistrationRepo;

        public TrainingRegistrationFileController(TrainingRegistrationFileRepo trainingRegistrationRepo)
        {
            _trainingRegistrationRepo = trainingRegistrationRepo;
        }

        [HttpGet("get-by-training-registration-id")]
        public IActionResult GetAll(int trainingRegistrationID)
        {
            try
            {
                List<TrainingRegistrationFile> lstFile = _trainingRegistrationRepo.GetAll(x => x.TrainingRegistrationID == trainingRegistrationID && x.IsDeleted == false);
                return Ok(new
                {
                    status = 1,
                    data = lstFile,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.ToString(),
                    error = ex.Message
                });
            }
        }
    }
}