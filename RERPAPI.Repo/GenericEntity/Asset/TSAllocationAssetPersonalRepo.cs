using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Asset
{
    public class TSAllocationAssetPersonalRepo : GenericRepo<TSAllocationAssetPersonal>
    {
        
        public TSAllocationAssetPersonalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        public string generateAllocationPersonalCode(DateTime? allocationDate)
        {
            var date = allocationDate.Value.Date;
            var latestCode = table.Where(x => x.DateAllocation.HasValue && x.DateAllocation.Value.Date == date &&
                                    !string.IsNullOrEmpty(x.Code)).OrderByDescending(x => x.ID).Select(x => x.Code).FirstOrDefault();

            string baseCode = $"TSCN.{date:ddMMyyyy}.";
            string code = string.IsNullOrEmpty(latestCode) ? $"{baseCode}00000" : latestCode;

            string numberPart = code.Substring(code.Length - 5);
            int nextNumber = int.TryParse(numberPart, out int num) ? num + 1 : 1;

            string numberStr = nextNumber.ToString("D5");
            string newCode = $"{baseCode}{numberStr}";

            return newCode;
        }
    }
}
