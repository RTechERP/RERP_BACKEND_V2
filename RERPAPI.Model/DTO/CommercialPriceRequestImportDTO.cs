using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class CommercialPriceRequestImportDTO : CommercialPriceRequest
    {
        public string? AdminSentAtHour { get; set; }
        public string? AdminSentAtDate { get; set; }

        public string? PurSentAtHour { get; set; }
        public string? PurSentAtDate { get; set; }

        public string? IsSaleQuotedText { get; set; }
        public string? IsPurQuotedText { get; set; }
    }
}