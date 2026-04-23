using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.HRM.HRRecruitment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HRHiringRequestExamController : ControllerBase
    {
        HiringRequestExamRepo _hiringRequestExamRepo;
        HRHiringRequestRepo _hiringRequestRepo;
        HRRecruitmentCandidateRepo _hrRecruitmentCandidateRepo;
        public HRHiringRequestExamController(HiringRequestExamRepo hiringRequestExamRepo, HRHiringRequestRepo hRHiringRequestRepo, HRRecruitmentCandidateRepo hrRecruitmentCandidateRepo)
        {
            _hiringRequestExamRepo = hiringRequestExamRepo;
            _hiringRequestRepo = hRHiringRequestRepo;
            _hrRecruitmentCandidateRepo = hrRecruitmentCandidateRepo;
        }

        //[HttpGet("get-data")]
        //public async Task<IActionResult> GetData()
        //{
        //    try
        //    {
        //        var data = await SqlDapper<object>.ProcedureToListAsync("spGetHRHiringRequestExamDetails", null);
        //        return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
        [HttpGet("get-data-hiring-request")]
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        public async Task<IActionResult> GetDataHiringRequest(DateTime dateStart, DateTime dateEnd, string? keyword) // thêm keyword, tìm kiếm theo ngày tạo 
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var keywordLower = keyword?.Trim().ToLower();
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddDays(1);

                var data = _hiringRequestRepo.GetAll(x =>!x.IsDeleted
                                                       && (string.IsNullOrEmpty(keywordLower)
                                                       || x.HiringRequestCode.ToLower().Contains(keywordLower)
                                                       || x.PositionName.ToLower().Contains(keywordLower))
                                                       && x.CreatedDate >= dateStart
                                                       && x.CreatedDate <= dateEnd
                                                       && (currentUser.DepartmentID == 6
                                                       || currentUser.IsAdmin
                                                       || x.EmployeeRequestID == currentUser.EmployeeID));
                // phòng hr có thể xem tất cả, nhân viên thường chỉ xem được những yêu cầu tuyển dụng do mình tạo ra ( trừ admin
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-exam-by-requestID")]
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        public async Task<IActionResult> GetExamByRequestID(int hiringRequestID)
        {
            try
            {
                var data = await SqlDapper<object>.ProcedureToListAsync("sp_GetRecruitmentExamByHiringRequestID", new { HiringRequestID = hiringRequestID });
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] HRHiringRequestExamDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }
                var a = _hiringRequestRepo.GetByID(model.HiringRequestID);
                a.IsActiveExam = model.IsActiveExam;
                await _hiringRequestRepo.UpdateAsync(a);

                // Kiểm tra nếu cả hai danh sách đều rỗng, không có gì để xử lý( trừ trường hợp active bài thi)
                if ((model.listHiringRequestIDExam == null || model.listHiringRequestIDExam.Count == 0) &&
                    (model.deletedHiringRequestIDExam == null || model.deletedHiringRequestIDExam.Count == 0) && model.IsActiveExam == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có đề thi nào được gửi để thêm/cập nhật hoặc xóa."));
                }

                // Lấy tất cả các đề thi hiện có cho HiringRequestID này, bao gồm cả các bản ghi đã bị xóa mềm
                var existingExamsInDb = _hiringRequestExamRepo.GetAll(x => x.HiringRequestID == model.HiringRequestID).ToList();

                // 1. Xử lý các đề thi cần xóa mềm trước
                if (model.deletedHiringRequestIDExam != null && model.deletedHiringRequestIDExam.Count > 0)
                {
                    foreach (var examIdToDelete in model.deletedHiringRequestIDExam)
                    {
                        var examToDelete = existingExamsInDb.FirstOrDefault(x => x.RecruitmentExamID == examIdToDelete && x.IsDeleted == false);
                        if (examToDelete != null)
                        {
                            examToDelete.IsDeleted = true;  // Đánh dấu đã xóa mềm
                            //examToDelete.IsActive = false;   // Ngừng kích hoạt
                            await _hiringRequestExamRepo.UpdateAsync(examToDelete);
                        }
                    }
                }

                // 2. So sánh và xử lý các đề thi trong listHiringRequestIDExam (thêm mới hoặc cập nhật)
                if (model.listHiringRequestIDExam != null && model.listHiringRequestIDExam.Count > 0)
                {
                    foreach (var examId in model.listHiringRequestIDExam)
                    {
                        var existingExam = existingExamsInDb.FirstOrDefault(x => x.RecruitmentExamID == examId);

                        if (existingExam != null) // Đề thi đã tồn tại trong DB (có thể đã bị xóa mềm hoặc đang hoạt động)
                        {
                            // Nếu đã tồn tại và có sự thay đổi về IsActive hoặc đang bị xóa mềm, thì cập nhật
                            if (existingExam.IsActive != model.IsActiveExam || existingExam.IsDeleted == true)
                            {
                                //existingExam.IsActive = model.IsActiveExam;
                                existingExam.IsDeleted = false; // Đảm bảo không bị đánh dấu xóa nếu được gửi lại
                                await _hiringRequestExamRepo.UpdateAsync(existingExam);
                            }
                        }
                        else // Đề thi chưa tồn tại cho HiringRequestID này, thêm mới
                        {
                            var newExam = new HRHiringRequestExam
                            {
                                HiringRequestID = model.HiringRequestID,
                                RecruitmentExamID = examId,
                                //IsActive = model.IsActiveExam,
                                IsDeleted = false,
                            };
                            await _hiringRequestExamRepo.CreateAsync(newExam);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(model, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết hơn trong môi trường thực tế
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpPost("delete-data")]
        public async Task<IActionResult> DeleteData(long id)
        {
            try
            {
                // Placeholder logic
                return Ok(ApiResponseFactory.Success(id, "Xóa dữ liệu thành công (Mock)"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lấy danh sách ưng viên tham gia kỳ tuyển dụng 
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpPost("get-candidates")]
        public async Task<IActionResult> GetCandidates(long hiringRequestId)
        {
            try
            {
                // trạng thái ứng viên được xem như là đã tham gia kỳ tuyển dụng, và phải chưa bị xóa mềm
                var data = _hrRecruitmentCandidateRepo.GetAll(x => x.HrHiringRequestID == hiringRequestId && x.IsDeleted == false).ToList();
                return Ok(ApiResponseFactory.Success(data, "Lấy danh sách ứng viên thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //cập nhật khóa bài thi ứng viên có đang hoạt động hay không
        [RequiresPermission("N1,N2,N32,N33,N38,N51,N52,N56,N61,N79,N81,N86")]
        [HttpPost("update-active-exam-candidate")]
        public async Task<IActionResult> UpdateActiveExamCandidate([FromQuery] List<int> ListCandidateId, [FromQuery] bool isActive)
        {
            try
            {
                var candidate = new HRRecruitmentCandidate();
                foreach (var candidateId in ListCandidateId)
                {
                    candidate = _hrRecruitmentCandidateRepo.GetByID(candidateId);
                    candidate.IsActiveExam = isActive;
                    candidate.UpdatedDate = DateTime.Now;
                    await _hrRecruitmentCandidateRepo.UpdateAsync(candidate);
                }
                return Ok(ApiResponseFactory.Success(candidate, $"Cập nhật trạng thái bài thi cho ứng viên thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
