using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RERPAPI.Controllers.GeneralCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiveSRatingDetailController : ControllerBase
    {
        private readonly FiveSRatingDetailRepo _fiveSRatingDetailRepo;
        private CurrentUser _currentUser;
        private readonly FiveSBonusMinusRepo _fiveSBonusMinusRepo;
        private readonly FiveSDepartmentRepo _departmentRepo;
        private readonly FiveSRatingTicketRepo _ticketRepo;
        private readonly FiveSErrorRepo _errorRepo;

        public FiveSRatingDetailController(
            FiveSRatingDetailRepo fiveSRatingDetailRepo, 
            CurrentUser currentUser, 
            FiveSBonusMinusRepo fiveSBonusMinusRepo,
            FiveSDepartmentRepo departmentRepo,
            FiveSRatingTicketRepo ticketRepo,
            FiveSErrorRepo errorRepo)
        {
            _fiveSRatingDetailRepo = fiveSRatingDetailRepo;
            _currentUser = currentUser;
            _fiveSBonusMinusRepo = fiveSBonusMinusRepo;
            _departmentRepo = departmentRepo;
            _ticketRepo = ticketRepo;
            _errorRepo = errorRepo;
        }

        /// <summary>
        /// Lấy dữ liệu ma trận chấm điểm cho một phiếu đánh giá (Ticket)
        /// </summary>
        [HttpGet("get-matrix")]
        [Authorize]
        public IActionResult GetMatrix(int ticketId)
        {
            try
            {
                var arrParamName = new string[] { "@TicketID" };
                var arrParamValue = new object[] { ticketId };

                var dataSet = SQLHelper<object>.ProcedureToList("spGetFiveSRatingDetailMatrix", arrParamName, arrParamValue);

                var result = new
                {
                    Details = SQLHelper<object>.GetListData(dataSet, 0),
                    Errors = SQLHelper<object>.GetListData(dataSet, 1),
                    Ticket = SQLHelper<object>.GetListData(dataSet, 2)
                };

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy dữ liệu tổng hợp ma trận chấm điểm cho toàn bộ đợt chấm (Session)
        /// </summary>
        [HttpGet("get-matrix-session")]
        [Authorize]
        public IActionResult GetMatrixSession(int sessionId)
        {
            try
            {
                var arrParamName = new string[] { "@SessionID" };
                var arrParamValue = new object[] { sessionId };

                var dataSet = SQLHelper<object>.ProcedureToList("spGetFiveSRatingSessionMatrix", arrParamName, arrParamValue);

                var result = new
                {
                    Details = SQLHelper<object>.GetListData(dataSet, 0),
                    Errors = SQLHelper<object>.GetListData(dataSet, 1),
                    Session = SQLHelper<object>.GetListData(dataSet, 2),
                    Tickets = SQLHelper<object>.GetListData(dataSet, 3)
                };

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu tổng hợp thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lấy dữ liệu tổng hợp tháng
        /// </summary>
        [HttpGet("get-matrix-monthly-summary")]
        [Authorize]
        public IActionResult GetMatrixMonthlySummary(int year, int month)
        {
            try
            {
                var arrParamName = new string[] { "@Year", "@Month" };
                var arrParamValue = new object[] { year, month };

                var dataSet = SQLHelper<object>.ProcedureToList("spGetFiveSRatingMonthlySummary", arrParamName, arrParamValue);

                var result = new
                {
                    Sessions = SQLHelper<object>.GetListData(dataSet, 0),
                    Departments = SQLHelper<object>.GetListData(dataSet, 1),
                    Points = SQLHelper<object>.GetListData(dataSet, 2),
                    Notes = SQLHelper<object>.GetListData(dataSet, 3)
                };

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu tổng hợp tháng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu dữ liệu chấm điểm hàng loạt từ ma trận
        /// Một lỗi chấm cho nhiều phòng ban sẽ lưu nhiều dòng
        /// </summary>
        [Authorize]
        [HttpPost("save-matrix")]
        public async Task<IActionResult> SaveMatrix([FromBody] List<FiveSRatingDetail> items)
        {
            try
            {
                if (items == null || items.Count == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu lưu"));

                int successCount = 0;
                foreach (var item in items)
                {
                    if (item.ID > 0)
                    {
                        successCount += await _fiveSRatingDetailRepo.UpdateAsync(item);
                    }
                    else
                    {
                        successCount += await _fiveSRatingDetailRepo.CreateAsync(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, $"Lưu thành công {successCount} bản ghi chi tiết"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-minus-points")]
        [Authorize]
        public IActionResult GetMinusPoints(int ticketId = 0, int departmentId = 0)
        {
            try
            {
                var arrParamName = new string[] { "@TicketID", "@DepartmentID" };
                var arrParamValue = new object[] { ticketId, departmentId };

                var result = SQLHelper<FiveSMinusDetailDto>.ProcedureToListModel("spGetFiveSMinusPoints", arrParamName, arrParamValue);

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //model lưu tiền phạt
        public class SaveMinusPointRequest
        {
            public FiveSBonusMinu? fiveSBonusMinus { get; set; }
            public int DepartmentID { get; set; }
            public int FiveSRatingTicketID { get; set; }
            public int FiveSErrorID { get; set; }
        }
        [Authorize]
        [HttpPost("save-minus-point")]
        public async Task<IActionResult> SaveMinusPoint([FromBody] SaveMinusPointRequest items)
        {
            try
            {
                if (items.fiveSBonusMinus == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để lưu"));
                var ratingDetail = _fiveSRatingDetailRepo.GetAll(x => x.FiveSDepartmentID == items.DepartmentID && x.FiveSRatingTicketID == items.FiveSRatingTicketID && x.FiveSErrorID == items.FiveSErrorID).FirstOrDefault();
                int successCount = 0;
                if (items.fiveSBonusMinus.ID > 0)
                {
                    items.fiveSBonusMinus.FiveSRatingDetailID = ratingDetail.ID;
                    successCount = await _fiveSBonusMinusRepo.UpdateAsync(items.fiveSBonusMinus);
                }
                else
                {
                    items.fiveSBonusMinus.FiveSRatingDetailID = ratingDetail.ID;
                    successCount = await _fiveSBonusMinusRepo.CreateAsync(items.fiveSBonusMinus);
                }
                return Ok(ApiResponseFactory.Success(null, $"Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
