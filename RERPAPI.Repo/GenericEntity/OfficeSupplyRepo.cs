using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class OfficeSupplyRepo : GenericRepo<OfficeSupply>
    {
        OfficeSupplyRepo _officesupplyRepo = new OfficeSupplyRepo();
        public string GetNextCodeRTC()
        {
            var bug = _officesupplyRepo.GetAll();
         var allCodes = table.Where(x=>x.CodeRTC.StartsWith("VPP")).Select(x=>x.CodeRTC).ToList();
            int maxNumber = 0;
            foreach (var code in allCodes)
            {
                var numberPart = code.Substring(3);
                if (int.TryParse(numberPart, out int num))
                {
                    if (num > maxNumber)
                        maxNumber = num;
                }
            }
            int nextNumber = maxNumber + 1;
            var nextCodeRTC = "VPP" + nextNumber;
            return nextCodeRTC;

        }
    }
}
