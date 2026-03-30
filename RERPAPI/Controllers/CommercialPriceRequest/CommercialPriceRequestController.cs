using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using NPOI.OpenXmlFormats.Dml;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;
using System.Globalization;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.CommercialPriceRequest
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommercialPriceRequestController : ControllerBase
    {
        private readonly CommercialPriceRequestRepo _commercialPriceRequestRepo;
        public CommercialPriceRequestController(CommercialPriceRequestRepo CommercialPriceRequestRepo)
        {
            _commercialPriceRequestRepo = CommercialPriceRequestRepo;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(string? keyword, int? yearNo, int? pageSize = 50, int? pageNumber = 1)
        {
            try
            {
                var data = SQLHelper<object>.ProcedureToList("spGetCommercialPriceRequest", new string[] { "@Keyword", "@YearNo", "@PageSize", "@PageNumber" },
                    new object[] { keyword, (yearNo.HasValue && yearNo.Value > 0) ? yearNo : null, pageSize, pageNumber }
                    );
                var data1 = await SqlDapper<object>.ProcedureToListTAsync("spGetCommercialPriceRequest", new { Keyword = keyword, YearNo = yearNo, PageSize = pageSize, PageNumber = pageNumber });
                return Ok(new
                {
                    status = 1,
                    data = data1/* SQLHelper<object>.GetListData(data, 0)*/,
                    message = "Cập nhật thành công!"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("post-data-import-excel")]
        public async Task<IActionResult> SaveData([FromBody] List<CommercialPriceRequestImportDTO> lstDTO)
        {
            try
            {

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var entities = lstDTO.Select(item => new Model.Entities.CommercialPriceRequest
                {
                    // ── Identity ──────────────────────────────────────────
                    ID = 0,
                    RfqNo = item.RfqNo,
                    RequestSeq = item.RequestSeq ?? null,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    Specification = item.Specification,
                    Description = item.Description,
                    Qty = item.Qty,
                    Unit = item.Unit,
                    Moq = item.Moq,
                    PicPurID = currentUser.ID,
                    PicPurName = currentUser.Name,
                    AdminSentAt = ParseDateTime(item.AdminSentAtDate, item.AdminSentAtHour),
                    PurSentAt = ParseDateTime(item.PurSentAtDate, item.PurSentAtHour),
                    QuoteDeadline = DateOnly.TryParse(item.QuoteDeadline?.ToString(), out var qd) ? qd : null,
                    UnitPrice = item.UnitPrice,
                    ShippingCost = item.ShippingCost,
                    Leadtime = item.Leadtime,
                    SaleLeadtime = item.SaleLeadtime,
                    MarginRate = item.MarginRate,
                    SaleUnitPrice = item.SaleUnitPrice,
                    Supplier = item.Supplier,
                    IsSaleQuoted = string.IsNullOrWhiteSpace(item.IsSaleQuotedText) ? null
                           : item.IsSaleQuotedText.Trim().ToLower() == "sale báo" ? 1 : 0,
                    IsPurQuoted = string.IsNullOrWhiteSpace(item.IsPurQuotedText) ? null
                           : item.IsPurQuotedText.Trim().ToLower() == "pur báo" ? 1 : 0,
                    RequestNote = item.RequestNote,
                    SaleNote = item.SaleNote,
                    ImportPriceNote = item.ImportPriceNote,
                    IsPO = item.IsPO,
                    OtherCost = item.OtherCost,
                    Vat = item.Vat,
                    PurRepliedAt = item.PurRepliedAt,
                    QuoteRound = item.QuoteRound,
                    SaleDeadline = item.SaleDeadline,
                    SalesPushedAt = item.SalesPushedAt,
                    SaleTotalPrice = item.SaleUnitPrice * item.Qty,
                    NoteReason = item.NoteReason,
                    WeekNo = item.WeekNo,
                    MonthNo = item.MonthNo,
                    YearNo = item.YearNo,
                    CreatedAt = DateTime.Now,
                    CreatedBy = currentUser.Name,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = currentUser.Name,
                }).ToList();

                var countSuccess = await _commercialPriceRequestRepo.CreateRangeAsync(entities);
                return Ok(new
                {
                    status = 1,
                    data = "",
                    countError = 0,
                    message = "Cập nhật thành công!"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        //    private DateTime? ParseDateTime(string? datePart, string? hourPart)
        //    {
        //        if (string.IsNullOrWhiteSpace(datePart) || string.IsNullOrWhiteSpace(hourPart))
        //            return null;

        //        var combined = $"{datePart.Trim()} {hourPart.Trim()}";

        //        var formats = new[]
        //        {
        //    "yyyy-MM-dd H'h'mm",    // "2025-10-31 15h00"
        //    "yyyy-MM-dd H:mm",      // "2025-10-31 15:00"
        //    "yyyy-MM-dd h:mm tt",   // "2025-10-31 3:00 PM"
        //    "yyyy-MM-dd h:mmtt",    // "2025-10-31 3:00PM"
        //};

        //        return DateTime.TryParseExact(
        //            combined,
        //            formats,
        //            CultureInfo.InvariantCulture,
        //            DateTimeStyles.None,
        //            out var result) ? result : null;
        //    }
        private DateTime? ParseDateTime(string? datePart, string? hourPart)
        {
            if (string.IsNullOrWhiteSpace(datePart))
                return null;

            if (!DateTime.TryParseExact(
                    datePart.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(hourPart))
                return date.Date;

            var hourFormats = new[]
            {
        "H'h'mm",
        "H:mm",
        "h:mm tt",
        "h:mmtt"
    };
            if (DateTime.TryParseExact(
                    hourPart.Trim(),
                    hourFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var time))
            {
                return date.Date.Add(time.TimeOfDay);
            }
            return date.Date;
        }
    }
}
