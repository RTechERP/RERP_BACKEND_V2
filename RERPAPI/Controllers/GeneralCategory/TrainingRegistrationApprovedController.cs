using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingRegistrationApprovedController : ControllerBase
    {
        private readonly TrainingRegistrationApprovedRepo _trainingRegistrationApprovedRepo = new TrainingRegistrationApprovedRepo();
        private readonly TrainingRegistrationApprovedFlowRepo _trainingRegistrationApprovedFlowRepo = new TrainingRegistrationApprovedFlowRepo();
        [HttpGet("get-by-training-registration-id")]
        public IActionResult GetTrainingregistrationApproved(int trainingRegistrationID)
        {
            try
            {
                var approvedRegistrations = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTrainningRegistrationApproved",
                    new string[] { "@TrainingRegistrationID" },
                    new object[] { trainingRegistrationID });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(approvedRegistrations, 0),
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

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveRegistration([FromBody] TrainingRegistrationApproved model)
        {
            try
            {
                if (model == null || model.TrainingRegistrationID <= 0 || model.TrainingRegistrationApprovedFlowID<=0)
                {
                    return BadRequest(new { status = 0, message = "dữ liệu không hợp lệ" });
                }
                int flowID = (model.TrainingRegistrationApprovedFlowID ?? 0) - 1;
                if (model.StatusApproved == 1)
                {
                    TrainingRegistrationApproved previosApproved = _trainingRegistrationApprovedRepo.GetAll(x => x.TrainingRegistrationID == model.TrainingRegistrationID &&
                                                                            (x.IsDeleted == false || x.IsDeleted==null) &&
                                                                            x.TrainingRegistrationApprovedFlowID==flowID).FirstOrDefault();

                    if ( previosApproved == null)
                    {
                        return BadRequest(new { status = 0, message = "Không tìm thấy phê duyệt trước đó" });
                    }
                    if(previosApproved.StatusApproved <=0)
                    {
                        return BadRequest(new { status = 0, message = "Phê duyệt trước đó chưa được phê duyệt" });
                    }
                }
               
                TrainingRegistrationApproved current = _trainingRegistrationApprovedRepo.GetAll(x => x.TrainingRegistrationID == model.TrainingRegistrationID &&
                                                                            (x.IsDeleted == false || x.IsDeleted==null) &&
                                                                            x.TrainingRegistrationApprovedFlowID == model.TrainingRegistrationApprovedFlowID).FirstOrDefault();
                //current.EmployeeApprovedID = model.EmployeeApprovedID;
                current.EmployeeApprovedActualID = model.EmployeeApprovedActualID;
                current.DateApproved = model.DateApproved;
                current.StatusApproved = model.StatusApproved;
                current.UnapprovedReason = model.UnapprovedReason;
                current.Note = model.Note;
                _trainingRegistrationApprovedRepo.UpdateFieldsByID(current.ID, current);
                return Ok(new { status = 1, data=current });


            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }


    }
}
