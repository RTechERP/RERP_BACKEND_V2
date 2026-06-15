using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class PaymentOrderOrderTypeRepo : GenericRepo<PaymentOrderOrderType>
    {
        public PaymentOrderOrderTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task Create(PaymentOrderDTO payment)
        {
            var existing = GetAll(x => x.PaymentOrderID == payment.ID && x.IsDeleted == false);
            if (existing.Count > 0)
            {
                foreach (var item in existing)
                {
                    item.IsDeleted = true;
                    await UpdateAsync(item);
                }
            }

            var newRecords = payment.PaymentOrderTypeIDs
                .Where(x => x.IsDeleted == false)
                .Select(x => new PaymentOrderOrderType
                {
                    PaymentOrderID = payment.ID,
                    PaymentOrderTypeID = x.PaymentOrderTypeID,
                    IsDeleted = false
                })
                .ToList();

            await CreateRangeAsync(newRecords);
        }
    }
}