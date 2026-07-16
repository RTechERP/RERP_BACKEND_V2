using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverRepo : GenericRepo<Handover>
    {
        public HandoverRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        /// <summary>
        /// Sinh mã biên bản tự động theo format: MB_RTC_yyyyMMdd.XXX
        /// </summary>
        public async Task<string> GenerateCodeAsync()
        {
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            string prefix = $"MB_RTC_{dateStr}.";

            // Lấy mã lớn nhất trong ngày
            var lastCode = GetAll(x => x.Code != null && x.Code.StartsWith(prefix))
                .OrderByDescending(x => x.Code)
                .Select(x => x.Code).FirstOrDefault()
                ;

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                var parts = lastCode.Split('.');
                if (parts.Length > 1 && int.TryParse(parts[1], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D3}";
        }
    }
}