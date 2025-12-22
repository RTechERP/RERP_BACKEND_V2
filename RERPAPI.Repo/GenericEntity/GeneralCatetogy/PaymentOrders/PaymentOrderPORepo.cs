using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderPORepo : GenericRepo<PaymentOrderPO>
    {
        public PaymentOrderPORepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public async Task Create(PaymentOrderDTO payment)
        {
            var pos = GetAll(x => x.PaymentOrderID == payment.ID);
            await  DeleteRangeAsync(pos);


            payment.PaymentOrderPOs.ForEach(x =>
            {
                x.PaymentOrderID = payment.ID;
            });

            await CreateRangeAsync(payment.PaymentOrderPOs);
        }
    }
}
