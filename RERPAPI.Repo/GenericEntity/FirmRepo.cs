using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class FirmRepo : GenericRepo<Firm>
    {
        public bool CheckFirmCodeExists(string firmCode, int? id = null)
        {
            try
            {
                var query = table.Where(f => f.FirmCode == firmCode && f.IsDelete != true);
                
                if (id.HasValue)
                {
                    query = query.Where(f => f.ID != id.Value);
                }
                
                return query.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã hãng: {ex.Message}", ex);
            }
        }
    }
}
