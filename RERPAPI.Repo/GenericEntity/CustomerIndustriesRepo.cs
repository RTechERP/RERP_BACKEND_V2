using Microsoft.EntityFrameworkCore;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class CustomerIndustriesRepo : GenericRepo<CustomerIndustry>
    {
        public CustomerIndustriesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public async Task<int> GetNextSTTAsync()
        {
            var maxSTT = await table
                .Where(x => x.IsDeleted != true)
                .MaxAsync(x => (int?)x.NumberOrder) ?? 0;

            return maxSTT + 1;
        }
    }
}