using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetManagementRepo:GenericRepo<TSAssetManagement>
    {
        public string GenerateAssetCode( DateTime? assetdate)
        {
         

            var date = assetdate.Value.Date;
            //var startDate = date;
            //var endDate = date.AddDays(1);

            //var latestCode = await _context.TSAssetManagements
            //    .Where(x => x.CreatedDate.HasValue &&
            //                x.CreatedDate >= startDate &&
            //                x.CreatedDate < endDate &&
            //                !string.IsNullOrEmpty(x.TSAssetCode))
            //    .OrderByDescending(x => x.ID)
            //    .Select(x => x.TSAssetCode)
            //    .FirstOrDefaultAsync();
            var latestCode = table.Where(x=>x.CreatedDate.HasValue &&  x.CreatedDate.Value.Date==date&&
            !string.IsNullOrEmpty(x.TSAssetCode)).OrderByDescending(x=>x.ID).Select(x=>x.TSAssetCode).FirstOrDefault();

            string baseCode = $"TS{date:ddMMyyyy}";
            string code = string.IsNullOrEmpty(latestCode) ? $"{baseCode}00000" : latestCode;

            string numberPart = code.Substring(code.Length - 5);
            int nextNumber = int.TryParse(numberPart, out int num) ? num + 1 : 1;

            string numberStr = nextNumber.ToString("D5");
            string newCode = $"{baseCode}{numberStr}";

            return newCode;
        }
    }
}
