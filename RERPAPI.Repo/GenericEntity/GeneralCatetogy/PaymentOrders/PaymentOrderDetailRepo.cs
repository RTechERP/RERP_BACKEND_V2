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
        private readonly PaymentOrderDetailUserTeamSaleRepo _paymentOrderDetailUserTeamSaleRepo;
        public PaymentOrderDetailRepo(CurrentUser currentUser, PaymentOrderDetailUserTeamSaleRepo paymentOrderDetailUserTeamSaleRepo) : base(currentUser)
        {
            _paymentOrderDetailUserTeamSaleRepo = paymentOrderDetailUserTeamSaleRepo;
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
                    payment.PaymentOrderDetails.ForEach(x =>
                    {
                        x.PaymentOrderID = payment.ID;
                        x.ParentID = x.ParentID ?? 0;
                    });
                    //return await CreateRangeAsync(payment.PaymentOrderDetails);

                    var parents = payment.PaymentOrderDetails.Where(x => x.ParentID == 0).ToList();
                    foreach (var item in parents)
                    {
                        //item.PaymentOrderID = payment.ID;
                        //item.ParentID = item.ParentID ?? 0;
                        await CreateAsync(item);

                        var childrens = payment.PaymentOrderDetails.Where(x => x.ParentID == item._id).ToList();
                        foreach (var child in childrens)
                        {
                            //child.PaymentOrderID = payment.ID;
                            child.ParentID = item.ID;

                            await CreateAsync(child);
                        }
                    }
                }

                await CreateRangeAsync(details);
                await _paymentOrderDetailUserTeamSaleRepo.Create(payment);

                return 1;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
