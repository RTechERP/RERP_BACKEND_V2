using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.HRM;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.Course;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.KHOAHOC
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ExamResultController : ControllerBase
    {
        private readonly ExamResultRepo _examResultRepo;
        private readonly ExamResultDetailRepo _examResultDetailRepo;
        public ExamResultController(
            ExamResultRepo examResultRepo,
            ExamResultDetailRepo examResultDetailRepo)
        {
            _examResultRepo = examResultRepo;
            _examResultDetailRepo = examResultDetailRepo;
        }

        // Lấy kết quả thi theo loại thi, quý, năm
        [HttpGet("get-exam-result")]
        public IActionResult GetExamResult(int yearValue, int season, int testType)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetExamResult",
                                              new string[] { "@YearValue", "@Season", "@TestType" },
                                              new object[] { yearValue, season, testType });

                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(data, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Xóa kết quả thi theo danh sách IDs truyền vào (VD: ids="1,2,3")
        [HttpPost("delete-exam-result")]
        public async Task<IActionResult> DeleteExamResult(string ids)
        {
            try
            {
                if (string.IsNullOrEmpty(ids))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn nhân viên muốn xóa!"));
                }

                List<int> idList = ids.Split(',')
                      .Select(id => int.Parse(id.Trim()))
                      .ToList();
                foreach (var id in idList)
                {
                    await _examResultDetailRepo.DeleteByAttributeAsync("ExamResultID", id);
                    await _examResultRepo.DeleteAsync(id);
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa kết quả thi của các nhân viên đã chọn thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        // Lấy chi tiết kết quả bài thi
        [HttpGet("get-exam-result-detail")]
        public IActionResult GetExamResultDetail(int yearValue, int quarter, int examType, int employeeID)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetExamResultDetail",
                                              new string[] { "@YearValue", "@Quarter", "@ExamType", "@EmployeeID" },
                                              new object[] { yearValue, quarter, examType, employeeID });

                var result = new
                {
                    detail = SQLHelper<object>.GetListData(data, 0),
                    summary = SQLHelper<object>.GetListData(data, 1)
                };

                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
