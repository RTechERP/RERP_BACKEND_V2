using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.DTO;
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
        public OfficeSupplyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public string GetNextCodeRTC()
        {
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
        public bool Validate(OfficeSupply item, out string message)
        {
            message = "";

            // Chuẩn hóa code nhập vào
            var newCode = (item.CodeRTC ?? string.Empty).Trim().ToUpper();

            bool exists = GetAll().Any(x =>
                x.IsDeleted != true
                && x.ID != item.ID
                && (x.CodeRTC ?? string.Empty).Trim().ToUpper() == newCode
            );

            if (exists)
            {
                message = "Mã văn phòng phẩm đã tồn tại";
                return false;
            }

            // Gán lại code đã trim để lưu xuống DB luôn cho sạch
            item.CodeRTC = newCode;

            return true;
        }
    }
}
