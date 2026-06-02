using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class DailyReportMarketingDTO : DailyReportTechnical
    {
        public List<DailyReportMarketingFile>? dailyReportMarketingFiles { get; set; }
        public List<int>? deletedFileID { get; set; }
    }
}