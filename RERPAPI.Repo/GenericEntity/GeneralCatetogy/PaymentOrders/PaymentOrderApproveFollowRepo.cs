using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderApproveFollowRepo : GenericRepo<PaymentOrderApproveFollow>
    {
        public PaymentOrderApproveFollowRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}