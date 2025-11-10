using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class BillExportDetailRepo: GenericRepo<BillExportDetail>
    {
        public BillExportDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        RTCContext _context= new RTCContext();
        public async Task<int> CreateAsynC(BillExportDetail item)
        {
            await _context.BillExportDetails.AddAsync(item);
            await _context.SaveChangesAsync();
            return item.ID;
        }
    }
}
