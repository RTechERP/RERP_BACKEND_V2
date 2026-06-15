using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class BillExportDetailRepo : GenericRepo<BillExportDetail>
    {
        public BillExportDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        private RTCContext _context = new RTCContext();

        public async Task<int> CreateAsynC(BillExportDetail item)
        {
            await _context.BillExportDetails.AddAsync(item);
            await _context.SaveChangesAsync();
            return item.ID;
        }
    }
}