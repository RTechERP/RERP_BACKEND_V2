using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderFileRepo : GenericRepo<PaymentOrderFile>
    {
        public PaymentOrderFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}