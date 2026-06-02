using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class PaymentOrderDetailDTO : PaymentOrderDetail
    {
        public int _id { get; set; } = -1;

        public List<PaymentOrderDetailUserTeamSale> PaymentOrderDetailUserTeamSales { get; set; } = new List<PaymentOrderDetailUserTeamSale>();
    }
}