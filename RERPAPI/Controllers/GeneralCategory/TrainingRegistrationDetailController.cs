using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingRegistrationDetailController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll(int trainingRegistrationID)
        {
            try
            {
                var dtAll = SQLHelper<dynamic>.ProcedureToList("spGetTrainingRegistrationDetail", new string[] { "@TrainingRegisterID" }, new object[]
                            {
                              trainingRegistrationID
                            });

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(dtAll, 0)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}