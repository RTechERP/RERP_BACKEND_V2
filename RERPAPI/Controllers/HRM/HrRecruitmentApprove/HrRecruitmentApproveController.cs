using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment;

namespace RERPAPI.Controllers.HRM.HrRecruitmentApprove
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrRecruitmentApproveController : ControllerBase
    {
        HRRecruitmentApproveRepo _hRRecruitmentApproveRepo;
        HRRecruitmentCandidateRepo _hRRecruitmentCandidateRepo;
        HRRecruitmentApplicationFormRepo _hRRecruitmentApplicationFormRepo;

        public HrRecruitmentApproveController(HRRecruitmentApproveRepo hRRecruitmentApproveRepo, HRRecruitmentCandidateRepo hRRecruitmentCandidateRepo, HRRecruitmentApplicationFormRepo hRRecruitmentApplicationFormRepo)
        {
            _hRRecruitmentApproveRepo = hRRecruitmentApproveRepo;
            _hRRecruitmentCandidateRepo = hRRecruitmentCandidateRepo;
            _hRRecruitmentApplicationFormRepo = hRRecruitmentApplicationFormRepo;
        }
        [HttpGet("get-data-to-hr-recruit-approve")]
        public async Task<IActionResult> GetDataToHRRecruitmentApproveAsync(int HRRecruitmentCandidateID)
        {
            try
            {
                var (Infomation, Experiences) = await SqlDapper<object>.QueryMultipleAsync<dynamic, dynamic>("spGetDataToHRRecruitmentApprove", new { HRRecruitmentCandidateID = HRRecruitmentCandidateID });
                return Ok(new
                {
                    status = 1,
                    infomation = Infomation,
                    experiences = Experiences,
                    message = "Lấy dữ liệu thành công!"
                });
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));

            }
        }

        [HttpGet("get-hr-recruitment-candidate-by-id-form")]
        public async Task<IActionResult> GetListHRRecruitmetApprove(int HRRecruitmentApplicationFormID)
        {
            try
            {
                int HRRecruitmentCandidateID = 0;
                var data = await _hRRecruitmentApplicationFormRepo.GetByIDAsync(HRRecruitmentApplicationFormID);
                if (data != null)
                {
                    HRRecruitmentCandidateID = data.HRRecruitmentCandidateID ?? 0;
                }
                //return Ok(new
                //{
                //    status = 1,
                //    data = HRRecruitmentCandidateID,
                //    message = "Lấy dữ liệu thành công!"
                //});
                return Ok(ApiResponseFactory.Success(HRRecruitmentCandidateID, $"Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }
        [HttpGet("get-list-hr-recruitment-approve")]
        public async Task<IActionResult> GetListHRRecruitmetApprove(string? Keyword, int? DepartmentID, int EmployeeID, DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(Keyword))
                {
                    Keyword = Keyword.Trim();
                }
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetHRRecruitmentApprove", new { Keyword = Keyword, DepartmentID = DepartmentID, EmployeeID = EmployeeID, StartDate = StartDate, EndDate = EndDate });
                //return Ok(new
                //{
                //    status = 1,
                //    data = data,
                //    message = "Lấy dữ liệu thành công!"
                //});
                return Ok(ApiResponseFactory.Success(data, $"Lấy dữ liệu thành công!"));

            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

        [HttpGet("get-data-hr-recruitment-approve")]
        public async Task<IActionResult> GetDataHRRecruitmentApproveAsync(int HRRecruitmentApplicationFormID)
        {
            try
            {
                var data = _hRRecruitmentApproveRepo.GetAll(c => c.HRRecruitmentApplicationFormID == HRRecruitmentApplicationFormID).FirstOrDefault();
                //return Ok(new
                //{
                //    status = 1,
                //    data = data,
                //    message = "Lấy dữ liệu thành công!"
                //});
                return Ok(ApiResponseFactory.Success(data, $"Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }
        bool IsApproved(int? approverId, string approverName)
        {
            return (approverId.HasValue && approverId > 0)
                || !string.IsNullOrEmpty(approverName);
        }

        [HttpPost("approve-hr-recruitment")]
        public async Task<IActionResult> ApproveHRRecruitment(
       [FromBody] List<int> lstID,
       [FromQuery] string type)
        {
            try
            {
                if (lstID == null || !lstID.Any())
                {
                    return Ok(ApiResponseFactory.Fail(null, "Danh sách ID không hợp lệ"));
                }

                if (string.IsNullOrEmpty(type))
                {
                    return Ok(ApiResponseFactory.Fail(null, "Thiếu loại duyệt"));
                }

                type = type.ToUpper().Trim();

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = _hRRecruitmentApproveRepo
                                .GetAll(c => lstID.Contains(c.ID))
                                .ToList();
                List<int?> lstIDNextStep = new List<int?>();
                
                foreach (var item in data)
                {
                    switch (type)
                    {
                        case "TBP":
                            // TBP duyệt nếu chưa duyệt
                            if (IsApproved(item.TBPApprover, item.TBPApproverName)) continue;
                            item.TBPApprover = currentUser.EmployeeID;
                            item.TBPApproverName = currentUser.FullName;
                            break;

                        case "HCNS":
                            // phải có TBP trước
                            if (!IsApproved(item.TBPApprover, item.TBPApproverName)) continue;
                            if (IsApproved(item.HCNSApprove, item.HCNSApproveName)) continue;

                            if (item.HCNSApprove > 0) continue;
                            item.HCNSApprove = currentUser.EmployeeID;
                            item.HCNSApproveName = currentUser.FullName;
                            break;

                        case "BGD":
                            // phải có HCNS trước
                            if (!IsApproved(item.HCNSApprove, item.HCNSApproveName)) continue;
                            if (IsApproved(item.BGDApprover, item.BGDApproverName)) continue;


                            item.BGDApprover = currentUser.EmployeeID;
                            item.BGDApproverName = currentUser.FullName;
                            // nhảy sang bước tiếp theo: Thư mời nhận việc
                            lstIDNextStep.Add(item.HRRecruitmentApplicationFormID);
                            break;

                        default:
                            return Ok(ApiResponseFactory.Fail(null, "Loại duyệt không hợp lệ"));
                    }
                }

                if ((await _hRRecruitmentApproveRepo.UpdateRangeAsync(data))<=0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra, vui lòng thử lại sau!"));
                }

                if (lstIDNextStep.Count()>0)
                {
                    // lấy id cần next step sang thư mời nhận việc
                    var applicationForms = _hRRecruitmentApplicationFormRepo
                        .GetAll(c => lstIDNextStep.Contains(c.ID))
                        .ToList();

                    var candidateIDs = applicationForms
                        .Select(x => x.HRRecruitmentCandidateID)
                        .ToList();

                    var candidates = _hRRecruitmentCandidateRepo
                        .GetAll(c => candidateIDs.Contains(c.ID))
                        .ToList();
                    foreach (var item in candidates)
                    {
                        item.Status = 7;
                    }

                    if ((await _hRRecruitmentCandidateRepo.UpdateRangeAsync(candidates)) <= 0)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra, vui lòng thử lại sau!"));
                    }
                }
               
                return Ok(ApiResponseFactory.Success(null, $"Duyệt thành công!"));
              
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null,ex.Message));

            }
        }

        [HttpPost("save-hr-recruitment-approve")]
        public async Task<IActionResult> PostSaveData([FromBody] HRRecruitmentApprove HRRecruitmentApprove)
        {
            try
            {
                if (HRRecruitmentApprove.ID <= 0)
                {
                    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                    var currentUser = ObjectMapper.GetCurrentUser(claims);
                    HRRecruitmentApprove.EmployeeApprover = currentUser.EmployeeID;
                    HRRecruitmentApprove.EmployeeApproverName = currentUser.FullName;
                    HRRecruitmentApprove.ProbationarySalary = HRRecruitmentApprove.BasicSalary * 90 / 100;

                    var result = await _hRRecruitmentApproveRepo.CreateAsync(HRRecruitmentApprove);
                    if (result != 1)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Thêm mới tờ trình thất bại!"));
                    }
                    //var HRRecruitmentApplicationForm = await _hRRecruitmentApplicationFormRepo.GetByIDAsync(HRRecruitmentApprove.HRRecruitmentApplicationFormID??0);
                    //var hRRecruitmentCandidate = await _hRRecruitmentCandidateRepo.GetByIDAsync(HRRecruitmentApplicationForm.HRRecruitmentCandidateID??0);
                    //hRRecruitmentCandidate.Status = 6;

                    //if ((await _hRRecruitmentCandidateRepo.UpdateAsync(hRRecruitmentCandidate)) !=1)
                    //{
                    //    return Ok(new
                    //    {
                    //        status = 0,
                    //        message = "Cập nhật trạng thái đơn tuyển dụng thất bại!"
                    //    });
                    //}
                }
                else
                {
                    var result = await _hRRecruitmentApproveRepo.UpdateAsync(HRRecruitmentApprove);
                    if (result != 1)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Cập nhật tờ trình thất bại!"));
                    }
                }
                // cập nhật trạng thái 
                var HRRecruitmentApplicationForm = await _hRRecruitmentApplicationFormRepo.GetByIDAsync(HRRecruitmentApprove.HRRecruitmentApplicationFormID ?? 0);
                var hRRecruitmentCandidate = await _hRRecruitmentCandidateRepo.GetByIDAsync(HRRecruitmentApplicationForm.HRRecruitmentCandidateID ?? 0);
                hRRecruitmentCandidate.Status = 6;

                if ((await _hRRecruitmentCandidateRepo.UpdateAsync(hRRecruitmentCandidate)) != 1)
                {

                    return Ok(ApiResponseFactory.Fail(null, "Cập nhật trạng thái đơn tuyển dụng thất bại!"));
                }

                return Ok(ApiResponseFactory.Success(null, $"Lưu tờ trình thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }

    }
}
