using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;

namespace RERPAPI.Controllers
{
    /// <summary>
    /// Link xem công khai: mở một trang ở chế độ chỉ đọc mà không cần đăng nhập.
    ///
    /// Luồng:
    ///   1. Người đã đăng nhập gọi POST sign  -> nhận token đã ký HMAC.
    ///   2. Người nhận link gọi GET  data?t=  -> server xác thực chữ ký rồi trả dữ liệu.
    ///
    /// Vì sao không nhận thẳng projectID ở endpoint ẩn danh: sẽ cho phép bất kỳ ai
    /// lặp ID để lấy toàn bộ dữ liệu. Token ký HMAC không đoán được.
    ///
    /// Danh sách trang được phép xem công khai chính là các nhánh trong switch của
    /// GetData — muốn mở thêm trang nào thì thêm nhánh cho trang đó.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PublicLinkController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ProjectHistoryProblemRepo _historyProblemRepo;
        private readonly ProjectHistoryProblemFileRepo _historyProblemFileRepo;

        public PublicLinkController(
            IConfiguration configuration,
            ProjectHistoryProblemRepo historyProblemRepo,
            ProjectHistoryProblemFileRepo historyProblemFileRepo)
        {
            _configuration = configuration;
            _historyProblemRepo = historyProblemRepo;
            _historyProblemFileRepo = historyProblemFileRepo;
        }

        private string Secret => _configuration["PublicLinkSettings:SecretKey"] ?? "";

        private int DefaultExpireDays =>
            int.TryParse(_configuration["PublicLinkSettings:ExpireDays"], out int days) ? days : 0;

        /// <summary>Tạo token cho link công khai. Bắt buộc đăng nhập.</summary>
        [HttpPost("sign")]
        [Authorize]
        public IActionResult Sign([FromBody] PublicLinkSignParam param)
        {
            try
            {
                if (param == null || string.IsNullOrWhiteSpace(param.Route))
                    return BadRequest(ApiResponseFactory.Fail(null, "Thiếu thông tin trang cần chia sẻ."));

                int expireDays = param.ExpireDays ?? DefaultExpireDays;

                var payload = new PublicLinkPayload
                {
                    Route = param.Route.Trim().ToLowerInvariant(),
                    Filters = param.Filters ?? new Dictionary<string, string>(),
                    Exp = expireDays > 0
                        ? DateTimeOffset.UtcNow.AddDays(expireDays).ToUnixTimeSeconds()
                        : null
                };

                string token = PublicLinkSigner.Sign(payload, Secret);

                return Ok(ApiResponseFactory.Success(new { token, exp = payload.Exp }, "Tạo link thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>Đọc dữ liệu theo token. Không cần đăng nhập, chỉ đọc.</summary>
        [HttpGet("data")]
        [AllowAnonymous]
        public IActionResult GetData(string t)
        {
            try
            {
                if (!PublicLinkSigner.TryVerify(t, Secret, out PublicLinkPayload? payload) || payload == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Link không hợp lệ hoặc đã hết hạn."));

                switch (payload.Route)
                {
                    case "issuelog":
                    case "issue-log":
                    case "project-history-problem-new":
                        return GetIssueLogData(payload);

                    default:
                        return BadRequest(ApiResponseFactory.Fail(null, "Trang này không hỗ trợ xem công khai."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Đọc dữ liệu chi tiết của một dòng (file đính kèm + các bảng liên kết).
        /// Không cần đăng nhập, chỉ đọc.
        ///
        /// `id` được kiểm tra phải thuộc đúng dự án ghi trong token — nếu không,
        /// một token hợp lệ của dự án A sẽ trở thành chìa khoá đọc chi tiết mọi
        /// dòng của mọi dự án khác.
        /// </summary>
        [HttpGet("detail")]
        [AllowAnonymous]
        public IActionResult GetDetail(string t, int id)
        {
            try
            {
                if (!PublicLinkSigner.TryVerify(t, Secret, out PublicLinkPayload? payload) || payload == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Link không hợp lệ hoặc đã hết hạn."));

                switch (payload.Route)
                {
                    case "issuelog":
                    case "issue-log":
                    case "project-history-problem-new":
                        return GetIssueLogDetail(payload, id);

                    default:
                        return BadRequest(ApiResponseFactory.Fail(null, "Trang này không hỗ trợ xem công khai."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // -------------------------------------------------------------------
        // Các trang được phép xem công khai
        // -------------------------------------------------------------------

        /// <summary>Lịch sử phát sinh theo dự án — dùng đúng SP của endpoint có đăng nhập.</summary>
        private IActionResult GetIssueLogData(PublicLinkPayload payload)
        {
            int projectID = GetInt(payload.Filters, "projectId");

            if (projectID <= 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Link thiếu thông tin dự án."));

            var data = SQLHelper<object>.ProcedureToList(
                "spGetProjectHistoryProblemDetail_New",
                new string[] { "@ProjectID", "@EmployeeID" },
                new object[] { projectID, 0 });

            var dtDetail = SQLHelper<object>.GetListData(data, 0);
            var dtMaster = SQLHelper<object>.GetListData(data, 2);

            return Ok(ApiResponseFactory.Success(new
            {
                route = payload.Route,
                filters = payload.Filters,
                dtDetail,
                dtMaster
            }, "Lấy dữ liệu thành công"));
        }

        /// <summary>Chi tiết một dòng lịch sử phát sinh: file đính kèm + các bảng liên kết.</summary>
        private IActionResult GetIssueLogDetail(PublicLinkPayload payload, int id)
        {
            int projectID = GetInt(payload.Filters, "projectId");

            if (projectID <= 0 || id <= 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Thiếu thông tin để lấy chi tiết."));

            // Chốt chặn quan trọng: dòng phải thuộc đúng dự án mà token cho phép.
            bool belongsToProject = _historyProblemRepo
                .GetAll(x => x.ID == id && x.ProjectID == projectID && x.IsDeleted != true)
                .Any();

            if (!belongsToProject)
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy dữ liệu."));

            var files = _historyProblemFileRepo
                .GetAll(x => x.ProjectHistoryProblemID == id && x.IsDeleted == false)
                .Select(x => new
                {
                    x.ID,
                    x.FileName,
                    x.ServerPath,
                    x.OriginPath,
                    x.ProjectHistoryProblemID,
                    x.FileType
                })
                .ToList();

            var data = SQLHelper<object>.ProcedureToList(
                "spGetProjectHistoryProblemLinkedData",
                new string[] { "@ProjectHistoryProblemID" },
                new object[] { id });

            var dtProjectItemLink = SQLHelper<object>.GetListData(data, 0);
            var dtWorkerVersionLink = SQLHelper<object>.GetListData(data, 1);
            var dtPartlistVersionLink = SQLHelper<object>.GetListData(data, 2);

            return Ok(ApiResponseFactory.Success(new
            {
                files,
                dtProjectItemLink,
                dtWorkerVersionLink,
                dtPartlistVersionLink
            }, "Lấy dữ liệu thành công"));
        }

        private static int GetInt(Dictionary<string, string> filters, string key)
        {
            if (filters == null) return 0;

            foreach (var pair in filters)
            {
                if (!string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase)) continue;
                return int.TryParse(pair.Value, out int value) ? value : 0;
            }

            return 0;
        }
    }
}
