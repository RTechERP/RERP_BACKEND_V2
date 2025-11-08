using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class BillExportDetailRepo : GenericRepo<BillExportDetail>
    {
        public BillExportDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
        //RTCContext _context= new RTCContext();
        //public async Task<int> CreateAsynC(BillExportDetail item)
        //{
        //    await CreateAsync(item);
        //    return item.ID;
        //}
    }
}
