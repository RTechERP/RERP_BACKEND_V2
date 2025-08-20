using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetTransferRepo :GenericRepo<TSTranferAsset>
    {
        public string GenTransferCode(DateTime? transferDate)
        {
            var date = transferDate ?? DateTime.Now;
            var formattedDate = date.ToString("yyyyMMdd");
            var latestCode = table
                .Where(x => x.CreatedDate.HasValue && x.CreatedDate.Value.Date == date.Date &&
                            !string.IsNullOrEmpty(x.CodeReport) && x.CodeReport.StartsWith("SBB" + formattedDate))
                .OrderByDescending(x => x.ID)
                .Select(x => x.CodeReport)
                .FirstOrDefault();

            string baseCode = $"SBB{formattedDate}";
            string numberPart = latestCode?.Substring(baseCode.Length, 6) ?? "000000";
            int nextNumber = int.TryParse(numberPart, out int num) ? num + 1 : 1;

            string numberStr = nextNumber.ToString("D6");
            string newCode = $"{baseCode}{numberStr}";

            return newCode;
        }

    }
}
