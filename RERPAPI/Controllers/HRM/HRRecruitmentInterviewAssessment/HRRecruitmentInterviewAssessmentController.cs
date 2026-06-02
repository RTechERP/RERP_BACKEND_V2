using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment;

namespace RERPAPI.Controllers.HRM.HRRecruitmentInterviewAssessment
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRRecruitmentInterviewAssessmentController : ControllerBase
    {
        private HRRecruitmentApplicationFormRepo _hRRecruitmentApplicationFormRepo;
        private HRRecruitmentInterviewAssessmentFormRepo _hRRecruitmentInterviewAssessmentFormRepo;
        private HRRecruitmentCandidateRepo _hRRecruitmentCandidateRepo;
        private HRRecruitmentApproveRepo _hRRecruitmentApproveRepo;
        private PerformanceCriteriaRepo _performanceCriteriaRepo;

        public HRRecruitmentInterviewAssessmentController(
            HRRecruitmentApplicationFormRepo hRRecruitmentApplicationFormRepo,
            HRRecruitmentInterviewAssessmentFormRepo hRRecruitmentInterviewAssessmentFormRepo,
            HRRecruitmentCandidateRepo hRRecruitmentCandidateRepo,
            HRRecruitmentApproveRepo hRRecruitmentApproveRepo,
            PerformanceCriteriaRepo performanceCriteriaRepo
            )
        {
            _performanceCriteriaRepo = performanceCriteriaRepo;
            _hRRecruitmentApplicationFormRepo = hRRecruitmentApplicationFormRepo;
            _hRRecruitmentInterviewAssessmentFormRepo = hRRecruitmentInterviewAssessmentFormRepo;
            _hRRecruitmentCandidateRepo = hRRecruitmentCandidateRepo;
            _hRRecruitmentApproveRepo = hRRecruitmentApproveRepo;
        }

        [HttpGet("get-data-hr-recruitment-application-form")]
        public async Task<IActionResult> GetDataHRRecruitmentApplicationFormAsync(int HRRecruitmentCandidateID)
        {
            try
            {
                var data = await SqlDapper<object>.ProcedureToListTAsync("spGetHRRecruitmentDataInterview", new { HRRecruitmentCandidateID = HRRecruitmentCandidateID });
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

        [HttpGet("get-performance-criteria")]
        public IActionResult GetPerformanceCriteria()
        {
            try
            {
                var data = _performanceCriteriaRepo.GetAll(c => c.IsDeleted != true && c.IsPublish == true);
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

        [HttpGet("get-data-by-hr-recruit-candidate-id")]
        public async Task<IActionResult> GetDataHRRecruitmentApplicationFormByIDAsync(int HRRecruitmentCandidateID)
        {
            try
            {
                var data = _hRRecruitmentInterviewAssessmentFormRepo.GetAll(c => c.HRRecruitmentCandidateID == HRRecruitmentCandidateID).FirstOrDefault() ?? new HRRecruitmentInterviewAssessmentForm();
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

        //[HttpGet("get-data-to-hr-recruit-approve")]
        //public async Task<IActionResult> GetDataToHRRecruitmentApproveAsync(int HRRecruitmentCandidateID)
        //{
        //    try
        //    {
        //        var (Infomation, Experiences) = await SqlDapper<object>.QueryMultipleAsync<dynamic,dynamic>("spGetDataToHRRecruitmentApprove", new { HRRecruitmentCandidateID = HRRecruitmentCandidateID });
        //        return Ok(new
        //        {
        //            status = 1,
        //            infomation = Infomation,
        //            experiences = Experiences,
        //            message = "Lấy dữ liệu thành công!"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}
        //[HttpGet("get-data-hr-recruitment-approve")]
        //public async Task<IActionResult> GetDataHRRecruitmentApproveAsync(int HRRecruitmentApplicationFormID)
        //{
        //    try
        //    {
        //        var data = _hRRecruitmentApproveRepo.GetAll(c=>c.HRRecruitmentApplicationFormID == HRRecruitmentApplicationFormID).FirstOrDefault();
        //        return Ok(new
        //        {
        //            status = 1,
        //            data = data,
        //            message = "Lấy dữ liệu thành công!"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = 0,
        //            message = ex.Message,
        //            error = ex.ToString()
        //        });
        //    }
        //}

        [HttpPost("save-data-after-interview")]
        public async Task<IActionResult> PostSaveData([FromBody] HRRecruitmentInterviewAssessmentForm HRRecruitmentInterviewAssessmentForm)
        {
            try
            {
                if (HRRecruitmentInterviewAssessmentForm.ID <= 0)
                {
                    HRRecruitmentInterviewAssessmentForm.DateSign = DateTime.Now;
                    var result = await _hRRecruitmentInterviewAssessmentFormRepo.CreateAsync(HRRecruitmentInterviewAssessmentForm);
                    if (result != 1)
                    {
                        //return Ok(new
                        //{
                        //    status = 0,
                        //    message = "Thêm mới phiếu đánh giá phỏng vấn thất bại!"
                        //});

                        return Ok(ApiResponseFactory.Fail(null, "Thêm mới phiếu đánh giá phỏng vấn thất bại!"));
                    }
                }
                else
                {
                    var result = await _hRRecruitmentInterviewAssessmentFormRepo.UpdateAsync(HRRecruitmentInterviewAssessmentForm);
                    if (result != 1)
                    {
                        //return Ok(new
                        //{
                        //    status = 0,
                        //    message = "Cập nhật phiếu đánh giá phỏng vấn thất bại!"
                        //});
                        return Ok(ApiResponseFactory.Fail(null, "Cập nhật phiếu đánh giá phỏng vấn thất bại!"));
                    }
                }
                var hRRecruitmentCandidate = await _hRRecruitmentCandidateRepo.GetByIDAsync(HRRecruitmentInterviewAssessmentForm.HRRecruitmentCandidateID ?? 0);
                if (HRRecruitmentInterviewAssessmentForm.ApplicantStatus == 1) // nếu phù hợp thì cập nhật trạng thái tờ khai
                {
                    // status = 5: kết quả đạt
                    hRRecruitmentCandidate.Status = 5;
                }
                else
                {
                    //status = 4: kết quả không đạt
                    hRRecruitmentCandidate.Status = 4;
                }
                var result1 = await _hRRecruitmentCandidateRepo.UpdateAsync(hRRecruitmentCandidate);
                if (result1 != 1)
                {
                    //return Ok(new
                    //{
                    //    status = 0,
                    //    message = "Cập nhật trạng thái hồ sơ ứng viên thất bại!"
                    //});
                    return Ok(ApiResponseFactory.Fail(null, "Cập nhật trạng thái hồ sơ ứng viên thất bại!"));
                }
                //return Ok(new
                //{
                //    status = 1,
                //    message = "Lưu đánh giá kết quả phỏng vấn thành công!"
                //});
                return Ok(ApiResponseFactory.Success(null, $"Lưu đánh giá kết quả phỏng vấn thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(null, ex.Message));
            }
        }
    }
}