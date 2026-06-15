using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderFileBankSlipRepo : GenericRepo<PaymentOrderFileBankSlip>
    {
        public PaymentOrderFileBankSlipRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}