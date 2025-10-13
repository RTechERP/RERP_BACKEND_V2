using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingRegistrationCategoryController : ControllerBase
    {
        private TrainingRegistrationCategoryRepo _trainingRegistrationCategoryRepo = new TrainingRegistrationCategoryRepo();

        [HttpGet]
        public IActionResult GetAll()
        {
            List<TrainingRegistrationCategory> lst = _trainingRegistrationCategoryRepo.GetAll(x => x.IsDeleted == false);
            return Ok(new
            {
                status = 0,
                data = lst
            });
        }
    }
}