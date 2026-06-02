using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderTypeRepo : GenericRepo<PaymentOrderType>
    {
        public PaymentOrderTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}