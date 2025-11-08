using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.BBNV
{
    public class HandoverReceiverRepo : GenericRepo<HandoverReceiver>
    {
        public HandoverReceiverRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
