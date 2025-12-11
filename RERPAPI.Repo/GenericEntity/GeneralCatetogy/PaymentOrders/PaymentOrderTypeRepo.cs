using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderTypeRepo : GenericRepo<PaymentOrderType>
    {
        public PaymentOrderTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
