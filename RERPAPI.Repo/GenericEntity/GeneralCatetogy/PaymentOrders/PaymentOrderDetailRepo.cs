using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

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
                        x.ID = 0;
                        x.PaymentOrderID = payment.ID;
                        x.ParentID = x.ParentID ?? 0;
                        x._id = payment.IsSpecialOrder == true ? -1 : x._id;
                    });

                    var parents = payment.PaymentOrderDetails.Where(x => x.ParentID == 0).ToList();
                    foreach (var item in parents)
                    {
                        await CreateAsync(item);

                        var childrens = payment.PaymentOrderDetails.Where(x => x.ParentID == item._id).ToList();
                        foreach (var child in childrens)
                        {
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