﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class TSAssetAllocationRepo : GenericRepo<TSAssetAllocation>
    {

        public string GenerateAllocationCode( DateTime? allocationDate)
        {
           

            var date = allocationDate.Value.Date;

            var latestCode = table.Where(x => x.DateAllocation.HasValue && x.DateAllocation.Value.Date == date &&
                                    !string.IsNullOrEmpty(x.Code)).OrderByDescending(x => x.ID).Select(x => x.Code).FirstOrDefault();

            string baseCode = $"TSCP{date:ddMMyyyy}";
            string code = string.IsNullOrEmpty(latestCode) ? $"{baseCode}00000" : latestCode;

            string numberPart = code.Substring(code.Length - 5);
            int nextNumber = int.TryParse(numberPart, out int num) ? num + 1 : 1;

            string numberStr = nextNumber.ToString("D5");
            string newCode = $"{baseCode}{numberStr}";

            return newCode;
        }
        public string test { get; set; }    
        public string test2 { get; set; }
    }
}
