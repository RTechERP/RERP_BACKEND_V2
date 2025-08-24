using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingRegistrationApprovedController : ControllerBase
    {
        private TrainingRegistrationApprovedRepo _trainingRegistrationApprovedRepo = new TrainingRegistrationApprovedRepo();
        private TrainingRegistrationApprovedFlowRepo _trainingRegistrationApprovedFlowRepo = new TrainingRegistrationApprovedFlowRepo();
        private EmployeeApprovedRepo _employeeApprovedRepo = new EmployeeApprovedRepo();

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
                if (model == null || model.TrainingRegistrationID <= 0 || model.TrainingRegistrationApprovedFlowID <= 0)
                {
                    return BadRequest(new { status = 0, message = "dữ liệu không hợp lệ" });
                }
                int flowID = (model.TrainingRegistrationApprovedFlowID ?? 0) - 1;
                TrainingRegistrationApproved current = _trainingRegistrationApprovedRepo.GetAll(x => x.TrainingRegistrationID == model.TrainingRegistrationID &&
                                                                            (x.IsDeleted == false || x.IsDeleted == null) &&
                                                                            x.TrainingRegistrationApprovedFlowID == model.TrainingRegistrationApprovedFlowID).FirstOrDefault();
                if (model.StatusApproved == 1)
                {
                    TrainingRegistrationApproved previosApproved = _trainingRegistrationApprovedRepo.GetAll(x => x.TrainingRegistrationID == model.TrainingRegistrationID &&
                                                                            (x.IsDeleted == false || x.IsDeleted == null) &&
                                                                            x.TrainingRegistrationApprovedFlowID == flowID).FirstOrDefault();
                    if (current.StatusApproved == 1)
                    {
                        return BadRequest(new { status = 0, message = "Phiếu đăng ký đã duyệt trước đó rồi!" });
                    }
                    if (previosApproved == null)
                    {
                        return BadRequest(new { status = 0, message = "Không tìm thấy phê duyệt trước đó" });
                    }
                    if (previosApproved.StatusApproved <= 0)
                    {
                        return BadRequest(new { status = 0, message = "Phê duyệt trước đó chưa được phê duyệt" });
                    }
                    List<EmployeeApprove> employeeApproveds = _employeeApprovedRepo.GetAll();
                    //if (!employeeApproveds.Any(e => e.ID == model.EmployeeApprovedActualID) && model.TrainingRegistrationApprovedFlowID>1   )
                    //{
                    //    return BadRequest(new { status = 0, message = "Bạn không có quyền duyệt" });
                    //}
                }
                if (model.StatusApproved == 2)
                {
                    if (current.StatusApproved == 2)
                    {
                        return BadRequest(new { status = 0, message = "Phiếu đăng ký đã hủy duyệt trước đó!" });
                    }
                    if (current.EmployeeApprovedActualID != model.EmployeeApprovedActualID)
                    {
                        return BadRequest(new { status = 0, message = "Bạn không có quyền hủy phê duyệt này" });
                    }
                }
                //current.EmployeeApprovedID = model.EmployeeApprovedID;
                current.EmployeeApprovedActualID = model.EmployeeApprovedActualID;
                current.DateApproved = model.DateApproved;
                current.StatusApproved = model.StatusApproved;
                current.UnapprovedReason = model.UnapprovedReason;
                current.Note = model.Note;
                _trainingRegistrationApprovedRepo.Update(current);
                return Ok(new { status = 1, data = current });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 0, message = ex.Message, error = ex.ToString() });
            }
        }
    }
}