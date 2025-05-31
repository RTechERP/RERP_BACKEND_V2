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
        //RTCContext _context = new RTCContext();
        //public async Task<int> DeleteByID(int id)
        //{
        //    try
        //    {
        //        var deleteItem = _context.POKHDetailMoneys.Find(id);
        //        if (deleteItem != null)
        //        {
        //            _context.POKHDetailMoneys.Remove(deleteItem);
        //            return _context.SaveChanges();
        //        }
        //        return 0; 
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}
    }
}
