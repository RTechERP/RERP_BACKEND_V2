using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderDetailUserTeamSaleRepo : GenericRepo<PaymentOrderDetailUserTeamSale>
    {
        public PaymentOrderDetailUserTeamSaleRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public async Task Create(PaymentOrderDTO payment)
        {
            foreach (var item in payment.PaymentOrderDetails)
            {
                item.PaymentOrderDetailUserTeamSales.ForEach(x =>
                {
                    x.PaymentOrderDetailID = item.ID;
                });


                await CreateRangeAsync(item.PaymentOrderDetailUserTeamSales);
            }
        }
    }
}
