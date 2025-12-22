using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.DTO
{
   public  class PaymentOrderDetailDTO: PaymentOrderDetail
    {
        public int _id { get; set; } = -1;

        public List<PaymentOrderDetailUserTeamSale> PaymentOrderDetailUserTeamSales { get; set; } = new List<PaymentOrderDetailUserTeamSale>();
    }
}
