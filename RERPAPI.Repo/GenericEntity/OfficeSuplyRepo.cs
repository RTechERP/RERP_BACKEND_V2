using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class OfficeSuplyRepo : GenericRepo<OfficeSupply>
    {
        public string GetNextCodeRTC()
        {
    //        var allCodes = await db.OfficeSupplies
    //.Where(x => x.CodeRTC.StartsWith("VPP"))
    //.Select(x => x.CodeRTC)
    //.ToListAsync();
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
