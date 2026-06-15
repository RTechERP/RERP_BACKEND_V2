using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PONCCDetailRequestBuyRepo : GenericRepo<PONCCDetailRequestBuy>
    {
        public PONCCDetailRequestBuyRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}