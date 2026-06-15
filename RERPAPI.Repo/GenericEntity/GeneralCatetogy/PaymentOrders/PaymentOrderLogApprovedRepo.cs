using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderLogApprovedRepo : GenericRepo<PaymentOrderLogApproved>
    {
        public PaymentOrderLogApprovedRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}