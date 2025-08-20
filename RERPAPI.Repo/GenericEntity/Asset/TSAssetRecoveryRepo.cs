using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAssetRecoveryRepo : GenericRepo<TSAssetRecovery>
    {
        public string genCodeRecovery(DateTime? recoveryDate)
        {

            var date = recoveryDate.Value.Date;
            var latestCode = table.Where(x => x.CreatedDate.HasValue && x.CreatedDate.Value.Date == date &&
            !string.IsNullOrEmpty(x.Code)).OrderByDescending(x => x.ID).Select(x => x.Code).FirstOrDefault();

            string baseCode = $"TSTH{date:ddMMyyyy}";
            string code = string.IsNullOrEmpty(latestCode) ? $"{baseCode}00000" : latestCode;

            string numberPart = code.Substring(code.Length - 5);
            int nextNumber = int.TryParse(numberPart, out int num) ? num + 1 : 1;

            string numberStr = nextNumber.ToString("D5");
            string newCode = $"{baseCode}{numberStr}";

            return newCode;
        }
    }
}
