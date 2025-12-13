using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity.GeneralCatetogy.PaymentOrders
{
    public class PaymentOrderDetailRepo : GenericRepo<PaymentOrderDetail>
    {
        public PaymentOrderDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<int> Create(PaymentOrderDTO payment)
        {
            try
            {
                var details = GetAll(x => x.PaymentOrderID == payment.ID && x.IsDeleted != true);

                //Xóa hết ds chi tiết
                foreach (var item in details)
                {
                    item.IsDeleted = true;
                    await UpdateAsync(item);
                }
                details.Clear();
                //Insert lại 1 list mới
                if (payment.IsDelete == false)
                {
                    return await CreateRangeAsync(details);
                }

                return await CreateRangeAsync(payment.PaymentOrderDetails);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
