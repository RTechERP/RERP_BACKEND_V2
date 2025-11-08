using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSRecoveryAssetPersonalRepo:GenericRepo<TSRecoveryAssetPersonal>
    {
        public TSRecoveryAssetPersonalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public string generateRecoveryPersonalCode(DateTime? recoveryDate)
        {
            var date = recoveryDate.Value.Date;
            var latestCode = table.Where(x => x.DateRecovery.HasValue && x.DateRecovery.Value.Date == date &&
                                    !string.IsNullOrEmpty(x.Code)).OrderByDescending(x => x.ID).Select(x => x.Code).FirstOrDefault();

            string baseCode = $"TSTH.{date:ddMMyyyy}.";
            string code = string.IsNullOrEmpty(latestCode) ? $"{baseCode}00000" : latestCode;

            string numberPart = code.Substring(code.Length - 5);
            int nextNumber = int.TryParse(numberPart, out int num) ? num + 1 : 1;

            string numberStr = nextNumber.ToString("D5");
            string newCode = $"{baseCode}{numberStr}";

            return newCode;
        }
    }
}
