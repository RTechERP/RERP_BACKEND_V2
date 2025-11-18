using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetManagementRepo : GenericRepo<TSAssetManagement>
    {
        public TSAssetManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public string GenerateAssetCode(DateTime? assetdate)
        {
            var date = assetdate.Value.Date;

            // Chỉ lọc theo format code, không dùng CreatedDate
            string baseCode = $"TS{date:ddMMyyyy}";

            var latestCode = table
                .Where(x => !string.IsNullOrEmpty(x.TSAssetCode) &&
                            x.TSAssetCode.StartsWith(baseCode))
                .OrderByDescending(x => x.TSAssetCode)
                .Select(x => x.TSAssetCode)
                .FirstOrDefault();

            string numberPart = "00000";

            if (!string.IsNullOrEmpty(latestCode) && latestCode.Length >= baseCode.Length + 5)
            {
                numberPart = latestCode.Substring(latestCode.Length - 5);
            }

            int nextNumber = int.TryParse(numberPart, out int num) ? num + 1 : 1;
            string numberStr = nextNumber.ToString("D5");
            string newCode = $"{baseCode}{numberStr}";

            return newCode;
        }
        public int GetMaxSTT()
        {
            var data = GetAll();
            if (!data.Any())
                return 0;
            return data.Max(x => x.STT) ?? 0;
        }
        public bool Validate(TSAssetManagement item, out string message)
        {
            message = "";
            
            bool exists = GetAll().Any(x => x.TSAssetCode == item.TSAssetCode && x.ID != item.ID && x.IsDeleted != true);
            bool existSeri = GetAll().Any(x => x.Seri == item.Seri && x.ID != item.ID && x.IsDeleted != true);
            bool existCodeNCC = GetAll().Any(x => x.TSCodeNCC == item.TSCodeNCC && x.ID != item.ID && x.IsDeleted != true);
          
            if (existSeri && !string.IsNullOrWhiteSpace(item.Seri))
            {
                message = $"Mã SerialNumber {item.Seri} đã tồn tại";
                return false;
            }
            if (existCodeNCC && !string.IsNullOrWhiteSpace(item.TSCodeNCC))
            {
                message = $"Mã tài sản {item.TSCodeNCC} đã tồn tại";
                return false;
            }
            return true;
        }
    }
}
