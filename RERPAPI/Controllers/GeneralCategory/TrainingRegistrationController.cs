using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingRegistrationController : ControllerBase
    {
        private TrainingRegistrationRepo _trainingRegistrationRepo = new TrainingRegistrationRepo();
        private TrainingRegistrationApprovedRepo _trainingRegistrationApprovedRepo = new TrainingRegistrationApprovedRepo();
        private TrainingRegistrationApprovedFlowRepo _trainingRegistrationApprovedFlowRepo = new TrainingRegistrationApprovedFlowRepo();
        private TrainingRegistrationFileRepo _trainingRegistrationFileRepo = new TrainingRegistrationFileRepo();
        private TrainingRegistrationDetailRepo _trainingRegistrationDetailRepo = new TrainingRegistrationDetailRepo();
        private EmployeeRepo _employeeRepo = new EmployeeRepo();

        [HttpPost]
        public IActionResult GetAll(TrainingRegistrationParam param)
        {
            try
            {
                var lstAll = SQLHelper<dynamic>.ProcedureToList(
                    "spGetTrainingRegistration",
                    new string[] { "@DateStart", "@DateEnd", "@DepartmentID", "@TrainingCategoryID" },
                    new object[] { param.DateStart, param.DateEnd, param.DepartmentID, param.TrainingCategoryID });
                var a = SQLHelper<dynamic>.GetListData(lstAll, 0);

                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<dynamic>.GetListData(lstAll, 0)
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

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData(TrainingRegistrationDTO model)
        {
            try
            {
                bool success = false;
                TrainingRegistrationCategoryRepo _trainingRegistrationCategoryRepo = new TrainingRegistrationCategoryRepo();

                // Save Training registration data
                if (model.ID <= 0)
                {
                    if (await _trainingRegistrationRepo.CreateAsync(model) > 0)
                        success = true;

                    // Lấy danh sách flow
                    var approvedFlows = _trainingRegistrationApprovedFlowRepo
                        .GetAll(x => x.IsDeleted == false)
                        .OrderBy(x => x.STT)
                        .ToList();

                    var approvedList = new List<TrainingRegistrationApproved>();

                    // Lấy thông tin nhân viên tạo
                    Employee creator = _employeeRepo.GetByID(model.EmployeeID ?? 0);

                    foreach (var flow in approvedFlows)
                    {
                        int employeeApprovedId = 0;
                        int statusApproved = 0;
                        if (flow.STT == 1)
                        {
                            employeeApprovedId = model.EmployeeID ?? 0;
                            statusApproved = employeeApprovedId == 0 ? 0 : 1;
                        }

                        approvedList.Add(new TrainingRegistrationApproved
                        {
                            TrainingRegistrationID = model.ID,
                            TrainingRegistrationApprovedFlowID = flow.ID,
                            StatusApproved = statusApproved,
                            EmployeeApprovedID = employeeApprovedId,
                            EmployeeApprovedActualID = employeeApprovedId
                        });
                    }

                    await _trainingRegistrationApprovedRepo.CreateRangeAsync(approvedList);
                    success = true;
                }
                else
                {
                    if (_trainingRegistrationRepo.Update(model) > 0)
                        success = true;
                }

                // Save Training registration file data
                if (model.LstFile != null && model.LstFile.Count > 0)
                {
                    foreach (var file in model.LstFile)
                    {
                        if (file.ID <= 0)
                        {
                            var trainingFile = new TrainingRegistrationFile
                            {
                                TrainingRegistrationID = model.ID,
                                FileName = file.FileName,
                                OriginName = file.OriginName,
                                ServerPath = file.ServerPath,
                                IsDeleted = file.IsDeleted
                            };
                            if (await _trainingRegistrationFileRepo.CreateAsync(trainingFile) > 0)
                                success = true;
                        }
                        else
                        {
                            if (_trainingRegistrationFileRepo.Update(file) > 0)
                                success = true;
                        }
                    }
                }
                // Save Training registration detail data
                if (model.LstDetail != null && model.LstDetail.Count > 0)
                {
                    foreach (var detail in model.LstDetail)
                    {
                        if (detail.ID <= 0)
                        {
                            detail.TrainingRegistrationID = model.ID;
                            if (await _trainingRegistrationDetailRepo.CreateAsync(detail) > 0)
                                success = true;
                        }
                        else
                        {
                            if (_trainingRegistrationDetailRepo.Update(detail) > 0)
                                success = true;
                        }
                    }
                }

                if (success)
                {
                    return Ok(new
                    {
                        status = 1,
                        data = model,
                    });
                }
                else
                {
                    return BadRequest(new { status = 0, message = "Không có dữ liệu nào được thay đổi hoặc lưu mới." });
                }
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