using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class FiveSRatingRepo : GenericRepo<FiveSRating>
    {
        public FiveSRatingRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<List<FiveSRating>> GetListAsync(int? yearValue, int? monthValue, string? keyword)
        {
            try
            {
                var sql = "EXEC spGetFiveSRatingList @YearValue, @MonthValue, @Keyword";
                var parameters = new[]
                {
                    new SqlParameter("@YearValue", (object)yearValue ?? DBNull.Value),
                    new SqlParameter("@MonthValue", (object)monthValue ?? DBNull.Value),
                    new SqlParameter("@Keyword", (object)keyword ?? DBNull.Value)
                };
                
                return await table.FromSqlRaw(sql, parameters).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetListAsync: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sinh mã Code tự động theo format: Năm_Tháng_STT (VD: 2026_03_001)
        /// STT là số thứ tự tăng dần trong cùng Năm+Tháng
        /// </summary>
        public async Task<string> GenerateCodeAsync(int year, int month)
        {
            // Đếm số bản ghi hiện có trong cùng Năm_Tháng (kể cả đã xóa để tránh trùng mã)
            int count = await table
                .Where(x => x.YearValue == year && x.MonthValue == month)
                .CountAsync();

            int stt = count + 1;
            return $"{year}_{month:D2}_{stt:D3}";
        }
    }
}
