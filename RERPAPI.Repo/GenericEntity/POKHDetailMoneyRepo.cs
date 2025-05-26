using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class POKHDetailMoneyRepo: GenericRepo<POKHDetailMoney>
    {
        RTCContext _context = new RTCContext();
        public async Task<int> DeleteByPOKHID(int id)
        {
            try
            {
                var deleteItem = _context.POKHDetailMoneys.Find(id);
                _context.POKHDetailMoneys.RemoveRange(deleteItem);
                return _context.SaveChanges();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
